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

namespace AlbionPriceComparer
{
    class DataSchemaLoader
    {
        public static void Load()
        {
            DataSet ds = new DataSet("ReferenceData");
            DataTable dt = new DataTable("BaseItemPower");
            dt.Columns.Add("artefact_type", typeof(string));
            dt.Columns.Add("teir", typeof(int));
            dt.Columns.Add("item_power", typeof(int));

            DataRow dr = dt.NewRow();
            dr["artefact_type"] = "normal";
            dr["teir"] = 4;
            dr["item_power"] = 700;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["artefact_type"] = "normal";
            dr["teir"] = 5;
            dr["item_power"] = 800;
            dt.Rows.Add(dr);

            ds.Tables.Add(dt);

            dt = new DataTable("EnchantProgression");
            dt.Columns.Add("artefact_type", typeof(string));
            dt.Columns.Add("teir", typeof(int));
            dt.Columns.Add("enchant_rune", typeof(int));
            dt.Columns.Add("enchant_soul", typeof(int));
            dt.Columns.Add("enchant_relic", typeof(int));

            dr = dt.NewRow();
            dr["artefact_type"] = "normal";
            dr["teir"] = 4;
            dr["enchant_rune"] = 100;
            dr["enchant_soul"] = 100;
            dr["enchant_relic"] = 80;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["artefact_type"] = "normal";
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
            Unknown = 0, Normal, Rune, Soul, Relic
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
            //List<ItemJSONLookupRow> filteredList = new List<ItemJSONLookupRow>();
            ItemJSONLookupRow[] rawData;

            var ser = new DataContractJsonSerializer(typeof(ItemJSONLookupRow[]));
            using (FileStream fs = new FileStream("items.json", FileMode.Open))
            {
                rawData = (ItemJSONLookupRow[])ser.ReadObject(fs);
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
                txt.Dispatcher.BeginInvoke(new Action(() => txt.Text = "Processing Item #" + k + ": " + item.UniqueName));
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
            //items.GroupBy(x => x.itemType).Select(g => g.First().itemType).ToList();
            items = (from i in items
                     where i.itemType == "equipment" || i.itemType == "weapon"
                     select i).GroupBy(i => i.uniqueName + i.teir).Select(i => i.First()).ToList();

            var ds = new DataSet();
            using (Stream stream = typeof(AlbionPriceComparer.MainWindow).Assembly.GetManifestResourceStream("AlbionPriceComparer.ReferenceDataSchema.xml"))
                ds.ReadXmlSchema(stream);
            using (Stream stream = typeof(AlbionPriceComparer.MainWindow).Assembly.GetManifestResourceStream("AlbionPriceComparer.ReferenceData.xml"))
                ds.ReadXml(stream);

            var dtItems = ds.Tables["Items"];
            var dtBase = ds.Tables["BaseItemPower"];

            var dtEnch = ds.Tables["BaseItemPower"].Copy();

            var une = dtBase.AsEnumerable().Where(r => (int)r["teir"] == 4).Join(
                ds.Tables["EnchantProgression"].AsEnumerable().Where(ep => (int)ep["teir"] == 4),
                r => (string)((DataRow)r)["artefact_type"],
                ep => (string)((DataRow)ep)["artefact_type"],
                (r, ep) => new { artefact_type = (string)r["artefact_type"], item_power = (int)r["item_power"] + (int)ep["enchant_rune"] });
            dtEnch.Clear();
            foreach (var a in une)
            {
                var r = dtEnch.NewRow();
                r["teir"] = 4;
                r["artefact_type"] = a.artefact_type;
                r["item_power"] = a.item_power;
                dtEnch.Rows.Add(r);
            }

            foreach (ItemData i in items)
            {
                var row = dtItems.NewRow();
                row["id"] = i.uniqueName.Replace("T4_", "");
                row["name"] = i.localizedNames.ENUS.Replace("Adept's ", "");
                row["two_handed"] = i.twoHanded;
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
            }
        }

        private static ArtefactType ArtefactTypeLookup(DataTable dt, int teir, int itemPower)
        {
            var x = (string)dt.AsEnumerable()
                .Where(row => (int)row["teir"] == teir && (int)row["item_power"] == itemPower)
                .First()["artefact_type"];
            
            foreach (ArtefactType a in Enum.GetValues(typeof(ArtefactType)))
            {
                if (Enum.GetName(typeof(ArtefactType), a).Equals(x, StringComparison.OrdinalIgnoreCase))
                    return a;
            }
            return ArtefactType.Unknown;
        }
    }
}
