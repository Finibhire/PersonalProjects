using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Windows;

namespace AlbionPriceComparer
{
    class DataSchemaLoader
    {
        public static void Load()
        {
            DataSet ds = new DataSet("ReferenceData");
            DataTable dt = new DataTable("BaseItemPower");
            dt.Columns.Add("artefact_type", typeof(ArtefactType));
            dt.Columns.Add("teir", typeof(int));
            dt.Columns.Add("item_power", typeof(int));

            DataRow dr = dt.NewRow();
            dr["artefact_type"] = ArtefactType.Normal;
            dr["teir"] = 4;
            dr["item_power"] = 700;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["artefact_type"] = ArtefactType.Normal;
            dr["teir"] = 5;
            dr["item_power"] = 800;
            dt.Rows.Add(dr);

            ds.Tables.Add(dt);

            dt = new DataTable("EnchantProgression");
            dt.Columns.Add("artefact_type", typeof(ArtefactType));
            dt.Columns.Add("teir", typeof(int));
            dt.Columns.Add("enchant_rune", typeof(int));
            dt.Columns.Add("enchant_soul", typeof(int));
            dt.Columns.Add("enchant_relic", typeof(int));

            dr = dt.NewRow();
            dr["artefact_type"] = ArtefactType.Normal;
            dr["teir"] = 4;
            dr["enchant_rune"] = 100;
            dr["enchant_soul"] = 100;
            dr["enchant_relic"] = 80;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["artefact_type"] = ArtefactType.Normal;
            dr["teir"] = 5;
            dr["enchant_rune"] = 100;
            dr["enchant_soul"] = 80;
            dr["enchant_relic"] = 80;
            dt.Rows.Add(dr);

            ds.Tables.Add(dt);

            ds.WriteXml("xmldoc.xml");
            ds.WriteXmlSchema("xmlschema.xml");
        }

        public static DataSet Read()
        {
            DataSet ds = new DataSet();
            ds.ReadXmlSchema("xmlschema.xml");
            ds.ReadXml("xmldoc.xml");
            return ds;
        }


        [DataContract]
        public class Localization
        {
            [DataMember(Name = "EN-US")]
            public string ENUS { get; set; }
            [DataMember(Name = "DE-DE")]
            public string DEDE { get; set; }
            [DataMember(Name = "FR-FR")]
            public string FRFR { get; set; }
            [DataMember(Name = "RU-RU")]
            public string RURU { get; set; }
            [DataMember(Name = "PL-PL")]
            public string PLPL { get; set; }
            [DataMember(Name = "ES-ES")]
            public string ESES { get; set; }
            [DataMember(Name = "PT-BR")]
            public string PTBR { get; set; }
            [DataMember(Name = "ZH-CN")]
            public string ZHCN { get; set; }
            [DataMember(Name = "KO-KR")]
            public string KOKR { get; set; }
        }

        [DataContract]
        public class ItemJSONLookupRow
        {
            [DataMember]
            public string LocalizationNameVariable { get; set; }
            [DataMember]
            public Localization LocalizedNames { get; set; }
            [DataMember]
            public Localization LocalizedDescriptions { get; set; }
            [DataMember]
            public string Index { get; set; }
            [DataMember]
            public string UniqueName { get; set; }
        }

        public enum ArtefactType
        {
            Unknown = -1, Normal, Rune, Soul, Relic
        }

        [DataContract]
        public class ItemEnchantment
        {
            [DataMember(EmitDefaultValue = true)]
            public int enchantmentLevel { get; set; }
            [DataMember(EmitDefaultValue = true)]
            public int itemPower { get; set; }
        }

        [DataContract]
        public class ItemEnchantmentBase
        {
            [DataMember]
            public ItemEnchantment[] enchantments { get; set; }
        }

        public enum SlotType
        {
            unknown = 0, armor = 1, bag, cape, head, mainhand, offhand, shoes
        }

        // TODO: add twohanded property
        [DataContract]
        public class ItemData
        {
            [DataMember]
            public string itemType { get; set; }
            [DataMember]
            public string uniqueName { get; set; }
            [DataMember]
            public int itemPower { get; set; }
            [DataMember]
            public bool twoHanded { get; set; }

            [DataMember]
            public Localization localizedNames { get; set; }

            [DataMember]
            public ItemEnchantmentBase enchantments { get; set; }


            public SlotType slotType { get; set; }

            [DataMember(Name = "slotType")]
            public string slotType_s
            {
                get
                {
                    return Enum.GetName(typeof(SlotType), slotType);
                }
                set
                {
                    foreach (SlotType slot in Enum.GetValues(typeof(SlotType)))
                    {
                        if (value.Equals(Enum.GetName(typeof(SlotType), slot), StringComparison.OrdinalIgnoreCase))
                        {
                            slotType = slot;
                            break;
                        }
                    }
                }
            }

            public int teir { get; set; }

            [DataMember(Name = "tier")]
            private string tier_s
            {
                get
                {
                    return teir.ToString();
                }
                set
                {
                    teir = Convert.ToInt32(value);
                }
            }

            public ItemData()
            {
                teir = 0;
            }
        }

        public delegate void CompileDataDelegate(Label lbl, out List<ItemData> items);

        public static void CompileData(TextBlock txt, out List<ItemData> items)
        {
            ItemJSONLookupRow[] rawData;

            var ser = new DataContractJsonSerializer(typeof(ItemJSONLookupRow[]));
            using (var stream = Application.GetContentStream(new Uri("/binItems.json", UriKind.Relative)).Stream)
            {
                rawData = (ItemJSONLookupRow[])ser.ReadObject(stream);
            }
            var filteredList = from item in rawData
                               where item.UniqueName.StartsWith("T4")
                               select item;
            
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            items = new List<ItemData>(filteredList.Count());
            var seri = new DataContractSerializer(typeof(ItemData));
            int k = 0;
            foreach (ItemJSONLookupRow item in filteredList)
            {
                txt.Dispatcher.BeginInvoke(new Action(() => txt.Text = "Downloading Item Data #" + k + ": " + item.UniqueName));
                ItemData buffer;
                var request = HttpWebRequest.Create(@"https://gameinfo.albiononline.com/api/gameinfo/items/" + item.UniqueName + @"/data");
                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (var stream = new StreamReader(response.GetResponseStream()))
                        {
                            buffer = (ItemData)serializer.Deserialize(stream, typeof(ItemData));
                        }
                    }
                    items.Add(buffer);
                }
                catch (WebException ex)
                {
                    //break;
                    if (!(ex.Status == WebExceptionStatus.ProtocolError && ex.Message.Contains("(404)")))
                        throw ex;
                }
                k++;
            }
            txt.Dispatcher.BeginInvoke(new Action(() => txt.Text = "Finished downloading" + k + " items #"));

            items = (from i in items
                     where i.itemType == "equipment" || i.itemType == "weapon"
                     select i).GroupBy(i => i.uniqueName + i.teir).Select(i => i.First()).ToList();


            var ds = new DataSet();
            using (var stream = Application.GetResourceStream(new Uri("/ReferenceDataSchema.xml", UriKind.Relative)).Stream)
                ds.ReadXmlSchema(stream);
            using (var stream = Application.GetResourceStream(new Uri("/ReferenceData.xml", UriKind.Relative)).Stream)
                ds.ReadXml(stream);

            var dtBase = ds.Tables["BaseItemPower"];
            var dtEnch = ds.Tables["BaseItemPower"].Copy();
            dtEnch.Clear();

            var une = dtBase.AsEnumerable().Where(r => (int)r["teir"] == 4).Join(
                ds.Tables["EnchantProgression"].AsEnumerable().Where(ep => (int)ep["teir"] == 4),
                r => (ArtefactType)r["artefact_type"],
                ep => (ArtefactType)ep["artefact_type"],
                (r, ep) => new { artefact_type = (ArtefactType)r["artefact_type"], item_power = (int)r["item_power"] + (int)ep["enchant_rune"] });

            foreach (var a in une)
            {
                var r = dtEnch.NewRow();
                r["teir"] = 4;
                r["artefact_type"] = a.artefact_type;
                r["item_power"] = a.item_power;
                dtEnch.Rows.Add(r);
            }

            DataSet dsOut = new DataSet();
            using (var stream = Application.GetResourceStream(new Uri("/ItemDataSchema.xml", UriKind.Relative)).Stream)
                dsOut.ReadXmlSchema(stream);
            var dtItems = dsOut.Tables["Items"];


            ArtefactType ArtefactTypeLookup(DataTable dt, int teir, int itemPower)
            {
                return (ArtefactType)dt.AsEnumerable()
                    .Where(row => (int)row["teir"] == teir && (int)row["item_power"] == itemPower)
                    .First()["artefact_type"];
            }


            foreach (ItemData i in items)
            {
                var row = dtItems.NewRow();
                row["id"] = i.uniqueName.Replace("T4_", "");
                row["name"] = i.localizedNames.ENUS.Replace("Adept's ", "");
                row["two_handed"] = i.twoHanded;
                row["slot_type"] = i.slotType;
                if (i.itemPower >= 500 || (i.itemPower < 500 && i.enchantments != null))
                {
                    if (i.itemPower < 500)
                    {
                        int ip = i.itemPower;
                        ip = i.enchantments.enchantments.Where(x => x.enchantmentLevel == 1).First().itemPower;
                        row["artefact_type"] = ArtefactTypeLookup(dtEnch, 4, ip);
                    }
                    else
                    {
                        row["artefact_type"] = ArtefactTypeLookup(dtBase, 4, i.itemPower);
                    }
                    dtItems.Rows.Add(row);
                }
                //else
                //    throw new Exception("Error looking up artefact_type for: " + i.uniqueName);
            }
            dsOut.WriteXml("ItemData.xml");
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
                    return 24;
                case SlotType.mainhand:
                    if (twoHanded)
                        return 96;
                    return 72;
                default:
                    throw new Exception("Unknown Rune Count to Enchant Item");
            }
        }

        private readonly DataSet refData;
        private readonly DataTable baseItemPower;
        private readonly DataTable enchantProgression;

        public DataSchemaLoader()
        {
            refData = new DataSet();
            using (var stream = Application.GetResourceStream(new Uri("/ReferenceDataSchema.xml", UriKind.Relative)).Stream)
                refData.ReadXmlSchema(stream);
            using (var stream = Application.GetResourceStream(new Uri("/ReferenceData.xml", UriKind.Relative)).Stream)
                refData.ReadXml(stream);
            baseItemPower = refData.Tables["BaseItemPower"];
            enchantProgression = refData.Tables["EnchantProgression"];
        }

        public int ItemPowerLookup(ArtefactType artType, int teir, ArtefactType enchLevel)
        {
            var results = baseItemPower.AsEnumerable().Where(r => (ArtefactType)r["artefact_type"] == artType && (int)r["teir"] == teir);
            if (results.Count() <= 0)
                throw new Exception("Unable to find base Item Power");
            if (results.Count() > 1)
                throw new Exception("Too many results!");

            int ip = (int)results.First()["item_power"];
            results = enchantProgression.AsEnumerable().Where(r => (ArtefactType)r["artefact_type"] == artType && (int)r["teir"] == teir);
            if (results.Count() <= 0)
                throw new Exception("Unable to find base Item Power");
            if (results.Count() > 1)
                throw new Exception("Too many results!");

            DataRow dr = results.First();
            if (enchLevel >= ArtefactType.Rune)
            {
                ip += (int)dr["enchant_rune"];
                if (enchLevel >= ArtefactType.Soul)
                {
                    ip += (int)dr["enchant_soul"];
                    if (enchLevel >= ArtefactType.Relic)
                    {
                        ip += (int)dr["enchant_relic"];
                    }
                }
            }
            return ip;
        }
    }
}
