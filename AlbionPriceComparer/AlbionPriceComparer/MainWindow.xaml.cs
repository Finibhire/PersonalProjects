using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using Swashbuckle;
using Swashbuckle.Swagger;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Data;
using static AlbionPriceComparer.DataSchemaLoader;
using System.Threading;

namespace AlbionPriceComparer
{
    public enum City
    {
        Unknown = 0,
        Caleron = 1,
        FortSterling = 2,
        Bridgewatch = 3,
        Lymhurst = 4,
        Thetford = 5,
        Martlock = 6
    }

    public static class CityMethods
    {
        public static string DisplayName(this City city)
        {
            switch (city)
            {
                case City.Unknown:
                    return "";
                case City.FortSterling:
                    return "Fort Sterling";
                default:
                    return Enum.GetName(typeof(City), city);
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum ItemQuality
        {
            Unknown = -1,
            Normal = 0,
            Good = 10,
            Outstanding = 20,
            Excelent = 50,
            Masterpeice = 100
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        [DataContract]
        private class MarketListing
        {
            [DataMember]
            public string item_id { get; set; }
            [DataMember]
            public int sell_price_min { get; set; }
            [DataMember]
            public int sell_price_max { get; set; }
            [DataMember]
            public int buy_price_min { get; set; }
            [DataMember]
            public int buy_price_max { get; set; }

            public City city { get; set; }

            [DataMember(Name = "city")]
            public string city_s
            {
                get => city.DisplayName();
                set
                {
                    city = City.Unknown;
                    string cityName = value.Replace(" ", "").ToLower();
                    foreach (City id in (City[])Enum.GetValues(typeof(City)))
                    {
                        if (Enum.GetName(typeof(City), id).ToLower() == cityName)
                        {
                            city = id;
                            break;
                        }
                    }
                }
            }

            public ItemQuality quality { get; set; }

            [DataMember(Name = "quality")]
            private int quality_id
            {
                get => ItemQualityIdLookup(quality);
                set => quality = ItemQualityLookup(value);
            }

            private static ItemQuality ItemQualityLookup(int iq)
            {
                switch (iq)
                {
                    case 1:
                        return ItemQuality.Normal;
                    case 2:
                        return ItemQuality.Good;
                    case 3:
                        return ItemQuality.Outstanding;
                    case 4:
                        return ItemQuality.Excelent;
                    case 5:
                        return ItemQuality.Masterpeice;
                    default:
                        return ItemQuality.Unknown;
                }
            }

            private static int ItemQualityIdLookup(ItemQuality iq)
            {
                switch (iq)
                {
                    case ItemQuality.Normal:
                        return 1;
                    case ItemQuality.Good:
                        return 2;
                    case ItemQuality.Outstanding:
                        return 3;
                    case ItemQuality.Excelent:
                        return 4;
                    case ItemQuality.Masterpeice:
                        return 5;
                    default:
                        return 0;
                }
            }

            public DateTime sell_price_min_date { get; set; }
            public DateTime sell_price_max_date { get; set; }
            public DateTime buy_price_min_date { get; set; }
            public DateTime buy_price_max_date { get; set; }

            [DataMember(Name = "sell_price_min_date")]
            private string sell_price_min_date_s
            {
                get => sell_price_min_date.ToString("yyyy-MM-ddTHH:mm:ss");
                set => sell_price_min_date = Convert.ToDateTime(value);
            }

            [DataMember(Name = "sell_price_max_date")]
            private string sell_price_max_date_s
            {
                get => sell_price_max_date.ToString("yyyy-MM-ddTHH:mm:ss");
                set => sell_price_max_date = Convert.ToDateTime(value);
            }

            [DataMember(Name = "buy_price_min_date")]
            private string buy_price_min_date_s
            {
                get => buy_price_min_date.ToString("yyyy-MM-ddTHH:mm:ss");
                set => buy_price_min_date = Convert.ToDateTime(value);
            }

            [DataMember(Name = "buy_price_max_date")]
            private string buy_price_max_date_s
            {
                get => buy_price_max_date.ToString("yyyy-MM-ddTHH:mm:ss");
                set => buy_price_max_date = Convert.ToDateTime(value);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string stringBuffer;

            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("AlbionPriceComparer.ReferenceData.xml"))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    stringBuffer = sr.ReadToEnd();
                }
            }

            //DataSchemaLoader.Load();
            DataSet ds = DataSchemaLoader.Read();
            //DataSet items = DataSchemaLoader.CompileData();

            WebRequest request;
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            //MarketListing[] listings;
            List<MarketListing> listings = new List<MarketListing>();
            request = HttpWebRequest.Create(@"https://www.albion-online-data.com/api/v2/stats/prices/T4_RUNE?locations=Caerleon,Bridgewatch,Fort%20Sterling&qualities=2");
            using (WebResponse response = request.GetResponse())
            {
                var ser = new DataContractJsonSerializer(typeof(MarketListing[]));
                using (Stream sr = response.GetResponseStream())
                {
                    listings.AddRange((MarketListing[])ser.ReadObject(sr));
                }
            }


            string marker = @"<td>Item Power</td><td>";
            int cutStart = stringBuffer.IndexOf(marker) + marker.Length;
            int cutEnd = stringBuffer.IndexOf(@"</td", cutStart);
            stringBuffer = stringBuffer.Substring(cutStart, cutEnd - cutStart);
            int baseItemPower = int.Parse(stringBuffer);


            //MarketListing[] listings;
            listings = new List<MarketListing>();
            request = HttpWebRequest.Create(@"https://www.albion-online-data.com/api/v2/stats/prices/T4_MAIN_RAPIER_MORGANA?locations=Caerleon,Bridgewatch,Fort%20Sterling&qualities=2");
            using (WebResponse response = request.GetResponse())
            {
                var ser = new DataContractJsonSerializer(typeof(MarketListing[]));
                using (Stream sr = response.GetResponseStream())
                {
                    listings.AddRange((MarketListing[])ser.ReadObject(sr));
                }
            }
        }

        public enum EnchantLevel
        {
            None = 0, Rune, Soul, Relic
        }

        private EnchantPrices enchPrices = new EnchantPrices();

        private class EnchantPrices
        {
            private double[,] enchPrices = new double[5, 3];
            private DateTime[,] enchPriceDates = new DateTime[5, 3];

            public double GetPrice(int teir, EnchantLevel lvl, out DateTime dt)
            {
                teir -= 4;
                int b = (int)lvl - 1;
                dt = enchPriceDates[teir, b];
                return enchPrices[teir, b];
            }
            public void SetPrice(int teir, EnchantLevel lvl, DateTime dt, double val)
            {
                teir -= 4;
                int b = (int)lvl - 1;
                enchPriceDates[teir, b] = dt;
                enchPrices[teir, b] = val;
            }
        }

        private void BtnEnchantPriceLookup_Click(object sender, RoutedEventArgs e)
        {
            string[] enchNames = { "RUNE", "SOUL", "RELIC" };
            EnchantLevel[] enchLvl = { EnchantLevel.Rune, EnchantLevel.Soul, EnchantLevel.Relic };
            string[] teirNames = { "T4_", "T5_", "T6_", "T7_", "T8_" };
            int[] teirs = { 4, 5, 6, 7, 8 };


            for (int i = 0; i < enchLvl.Length; i++)
            {
                for (int j = 0; j < teirs.Length; j++)
                {
                    WebRequest request = HttpWebRequest.Create(@"https://www.albion-online-data.com/api/v2/stats/prices/" + teirNames[j] + enchNames[i] + @"?locations=Caerleon,Fort%20Sterling&qualities=2");
                    using (WebResponse response = request.GetResponse())
                    {
                        var ser = new DataContractJsonSerializer(typeof(MarketListing[]));
                        MarketListing[] ml;

                        using (Stream sr = response.GetResponseStream())
                            ml = (MarketListing[])ser.ReadObject(sr);

                        double count = 0;
                        double sum = 0;
                        TimeSpan timeDis = new TimeSpan(0);
                        foreach (MarketListing m in ml)
                        {
                            if (m.city != City.Unknown)
                            {
                                count++;
                                sum += m.sell_price_min;
                                timeDis += DateTime.UtcNow - m.sell_price_min_date;
                            }
                        }
                        DateTime avgDate = DateTime.UtcNow - new TimeSpan(timeDis.Ticks / (long)count);
                        double avgPrice = sum / count;
                        enchPrices.SetPrice(teirs[j], enchLvl[i], avgDate, avgPrice);
                    }
                }
            }
        }


        Thread worker;
        private void BtnCollateItemData_Click(object sender, RoutedEventArgs e)
        {
            List<ItemData> items;
            worker = new Thread(() => DataSchemaLoader.CompileData(txtOut, out items));
            worker.Start();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            worker.Suspend();
            string content = (string)lblStatus.Content;
            worker.Resume();
        }
    }
}