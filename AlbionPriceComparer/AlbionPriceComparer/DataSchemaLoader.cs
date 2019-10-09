using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows;
using AlbionPriceComparer.AOEndorsed;

namespace AlbionPriceComparer
{
    class DataSchemaLoader
    {

        public static void CompileData(TextBlock txt)
        {
            BinItemJSON[] rawData;

            var ser = new DataContractJsonSerializer(typeof(BinItemJSON[]));
            using (var stream = Application.GetContentStream(new Uri("/binItems.json", UriKind.Relative)).Stream)
            {
                rawData = (BinItemJSON[])ser.ReadObject(stream);
            }
            var filteredList = from item in rawData
                               where item.UniqueName.StartsWith("T4") 
                               where item.UniqueName[item.UniqueName.Length - 2] != '@'
                               select item;

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var items = new List<AOEItemData>(filteredList.Count());
            var seri = new DataContractSerializer(typeof(AOEItemData));
            int k = 0;
            foreach (BinItemJSON item in filteredList)
            {
                txt.Dispatcher.BeginInvoke(new Action(() => txt.Text = "Downloading Item Data #" + k + ": " + item.UniqueName));
                AOEItemData buffer;
                var request = HttpWebRequest.Create("https://gameinfo.albiononline.com/api/gameinfo/items/" + item.UniqueName + "/data");
                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (var stream = new StreamReader(response.GetResponseStream()))
                        {
                            buffer = (AOEItemData)serializer.Deserialize(stream, typeof(AOEItemData));
                        }
                    }
                    items.Add(buffer);
                }
                catch (WebException ex)
                {
                    //break;
                    if (!(ex.Status == WebExceptionStatus.ProtocolError && ex.Message.Contains("(404)")))
                        throw;
                }
                k++;
            }
            txt.Dispatcher.BeginInvoke(new Action(() => txt.Text = "Finished downloading " + k + " game items"));

            items = (from i in items
                     where i.itemType == "equipment" || i.itemType == "weapon"
                     select i).GroupBy(i => i.uniqueName + i.teir).Select(i => i.First()).ToList();


            ReferenceDataSet ds = new ReferenceDataSet();
            //using (var stream = Application.GetResourceStream(new Uri("/ReferenceDataSchema.xml", UriKind.Relative)).Stream)
            //    ds.ReadXmlSchema(stream);
            using (var stream = Application.GetResourceStream(new Uri("/ReferenceData.xml", UriKind.Relative)).Stream)
                ds.ReadXml(stream);
            
            var dtBase = ds.BaseItemPowerTable;
            var dtEnch = (ReferenceDataSet.BaseItemPowerTableDataTable)dtBase.Clone();
            dtEnch.Clear();

            var une = dtBase
                .AsEnumerable()
                .Where(r => r.Teir == 4)
                .Join(
                    ds.EnchantProgressionTable.AsEnumerable().Where(ep => ep.Teir == 4),
                    r => (ArtefactType)r.ArtefactType,
                    ep => (ArtefactType)ep.ArtefactType,
                    (r, ep) => new { ArtefactType = r.ArtefactType, ItemPower = r.ItemPower + ep.EnchantRune });

            foreach (var a in une)
            {
                var r = dtEnch.NewBaseItemPower();
                r.Teir = 4;
                r.ArtefactType = a.ArtefactType;
                r.ItemPower = a.ItemPower;
                dtEnch.AddBaseItemPower(r);
            }

            GameItemDataSet dsOut = new GameItemDataSet();
            //using (var stream = Application.GetResourceStream(new Uri("/ItemDataSchema.xml", UriKind.Relative)).Stream)
            //    dsOut.ReadXmlSchema(stream);
            var dtItems = dsOut.GameItems;


            int ArtefactTypeLookup(ReferenceDataSet.BaseItemPowerTableDataTable dt, int teir, int itemPower)
            {
                return dt
                    .AsEnumerable()
                    .Where(row => row.Teir == teir && row.ItemPower == itemPower)
                    .First()
                    .ArtefactType;
            }


            foreach (AOEItemData i in items)
            {
                var row = dtItems.NewGameItem();
                row.Id = i.uniqueName.Replace("T4_", "");
                row.Name = i.localizedNames.ENUS.Replace("Adept's ", "");
                row.TwoHanded = i.twoHanded;
                row.SlotType = (int)i.slotType;
                if (i.itemPower >= 500 || (i.itemPower < 500 && i.enchantments != null))
                {
                    if (i.itemPower < 500)
                    {
                        int ip = i.itemPower;
                        ip = i.enchantments.enchantments.Where(x => x.enchantmentLevel == 1).First().itemPower;
                        row.ArtefactType = ArtefactTypeLookup(dtEnch, 4, ip);
                    }
                    else
                    {
                        row.ArtefactType = ArtefactTypeLookup(dtBase, 4, i.itemPower);
                    }
                    dtItems.Rows.Add(row);
                }
                //else
                //    throw new Exception("Error looking up artefact_type for: " + i.uniqueName);
            }

            dsOut.WriteXml("GameItems.xml");

            ds.Dispose();
            dsOut.Dispose();
        }

        public static int EnchantRuneCount(SlotType slot, bool twoHanded)
        {

            switch (slot)
            {
                case SlotType.armor:
                case SlotType.bag:
                    return 48;
                case SlotType.head:
                case SlotType.offhand:
                case SlotType.shoes:
                case SlotType.cape:
                    return 24;
                case SlotType.mainhand:
                    if (twoHanded)
                        return 96;
                    return 72;
                default:
                    throw new Exception("Unknown Rune Count to Enchant Item");
            }
        }

        private readonly ReferenceDataSet refData;
        private readonly ReferenceDataSet.BaseItemPowerTableDataTable baseItemPower;
        private readonly ReferenceDataSet.EnchantProgressionTableDataTable enchantProgression;

        public DataSchemaLoader()
        {
            refData = new ReferenceDataSet();
            //using (var stream = Application.GetResourceStream(new Uri("/ReferenceDataSchema.xml", UriKind.Relative)).Stream)
            //    refData.ReadXmlSchema(stream);
            using (var stream = Application.GetResourceStream(new Uri("/ReferenceData.xml", UriKind.Relative)).Stream)
                refData.ReadXml(stream);
            baseItemPower = refData.BaseItemPowerTable;
            enchantProgression = refData.EnchantProgressionTable;
        }

        public int ItemPowerLookup(ArtefactType artType, int teir, EnchantLevel enchLevel)
        {
            var baseLookup = baseItemPower.AsEnumerable().Where(r => (ArtefactType)r.ArtefactType == artType && r.Teir == teir);
            if (baseLookup.Count() <= 0)
                throw new Exception("Unable to find base Item Power");
            if (baseLookup.Count() > 1)
                throw new Exception("Too many results!");

            int ip = baseLookup.First().ItemPower;
            var epLookup = enchantProgression.AsEnumerable().Where(r => (ArtefactType)r.ArtefactType == artType && r.Teir == teir);
            if (epLookup.Count() <= 0)
                throw new Exception("Unable to find base Item Power");
            if (epLookup.Count() > 1)
                throw new Exception("Too many results!");

            var epRow = epLookup.First();
            if (enchLevel >= EnchantLevel.Rune)
            {
                ip += epRow.EnchantRune;
                if (enchLevel >= EnchantLevel.Soul)
                {
                    ip += epRow.EnchantSoul;
                    if (enchLevel >= EnchantLevel.Relic)
                    {
                        ip += epRow.EnchantRelic;
                    }
                }
            }
            return ip;
        }
    }
}
