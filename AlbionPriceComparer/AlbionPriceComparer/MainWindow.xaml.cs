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
using System.Web;
using static AlbionPriceComparer.GameItemDataSet;

namespace AlbionPriceComparer
{
    public enum City
    {
        Unknown = 0,
        Caerleon = 1,
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


        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly EnchantPrices enchPrices = new EnchantPrices();

        private class EnchantPrices
        {
            private readonly double[,] enchPrices = new double[5, 3];
            private readonly DateTime[,] enchPriceDates = new DateTime[5, 3];

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
                    WebRequest request = HttpWebRequest.Create("https://www.albion-online-data.com/api/v2/stats/prices/" + teirNames[j] + enchNames[i] + "?locations=Caerleon,Fort%20Sterling");
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
            worker = new Thread(() => DataSchemaLoader.CompileData(txtOut));
            worker.Start();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            worker.Suspend();
            string content = txtOut.Text;
            worker.Resume();
        }

        private GameItemDataSet cmbItemSelDS;
        private void BtnUpdateItemSelection_Click(object sender, RoutedEventArgs e)
        {
            if (cmbItemSelDS != null)
                cmbItemSelDS.Dispose();
            cmbItemSelDS = new GameItemDataSet();
            //using (var stream = Application.GetResourceStream(new Uri("/GameItemSchema.xml", UriKind.Relative)).Stream)
            //    ds.ReadXmlSchema(stream);
            using (var stream = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + "\\GameItems.xml", FileMode.Open))
                cmbItemSelDS.ReadXml(stream);
            var dtItems = cmbItemSelDS.GameItems;
            cmbItemSelection.DisplayMemberPath = "Name";
            cmbItemSelection.SelectedValuePath = "Id";
            cmbItemSelection.ItemsSource = dtItems.DefaultView;
        }
        ~MainWindow()
        {
            if (cmbItemSelDS != null)
                cmbItemSelDS.Dispose();
        }

        private void CmbItemSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbItemSelection.SelectedIndex >= 0)
                txtOut.Text = "Selected: " + ((GameItem)((DataRowView)cmbItemSelection.SelectedItem).Row).Id;
        }

        //TODO: make this run in another thread so the screen doesn't lock up as it does the HTTP Web Requests.
        private void BtnAnalyzeMarket_Click(object sender, RoutedEventArgs e)
        {
            if (cmbItemSelection.SelectedIndex < 0)
            {
                txtOut.Text = "Error: select an item first!";
                return;
            }

            StringBuilder sNormal = new StringBuilder();
            StringBuilder sEnchant = new StringBuilder();
            StringBuilder sEnd = new StringBuilder();

            sNormal.Append("https://www.albion-online-data.com/api/v2/stats/prices/T");
            int replaceIndex = sNormal.Length;
            sNormal.Append("4_");
            sNormal.Append(((GameItem)((DataRowView)cmbItemSelection.SelectedItem).Row).Id);
            sEnchant.Append(sNormal.ToString());
            sEnchant.Append("@1");
            int replaceIndexEnchant = sEnchant.Length - 1;
            sEnd.Append("?locations=");
            foreach (City c in Enum.GetValues(typeof(City)))
                if (c != City.Unknown)
                    sEnd.Append(HttpUtility.UrlEncode(c.DisplayName())).Append(",");
            sEnd.Length -= 1;
            string sTemp = sEnd.ToString();
            sNormal.Append(sTemp);
            sEnchant.Append(sTemp);


            var listings = new List<MarketListing>();
            var dsl = new DataSchemaLoader();
            var lookupItem = (GameItem)((DataRowView)cmbItemSelection.SelectedItem).Row;

            BtnEnchantPriceLookup_Click(null, null);

            void AppendMarketListings(StringBuilder url)
            {
                MarketListing[] mls;
                for (char teir = '4'; teir <= '8'; teir++)
                {
                    url[replaceIndex] = teir;
                    string sUrl = url.ToString();
                    txtOut.Text = "Loading URL:  " + sUrl;

                    var request = HttpWebRequest.Create(sUrl);
                    using (WebResponse response = request.GetResponse())
                    {
                        var ser = new DataContractJsonSerializer(typeof(MarketListing[]));
                        using (Stream sr = response.GetResponseStream())
                        {
                            mls = (MarketListing[])ser.ReadObject(sr);
                        }
                    }
                    var mlsFiltered = mls.Where(r => r.sell_price_min > 0 && r.sell_price_min_date != DateTime.Parse("0001-01-01T00:00:00") && r.quality != GameItemQuality.Unknown && r.city != City.Unknown);
                    listings.AddRange(mlsFiltered);
                    txtOut.Text = "Finished Loading URL:  " + sUrl;
                }
            }

            MarketListing SetEnchant(MarketListing ml, EnchantLevel newEl)
            {
                ml.RuneEnchantCost = 0;
                ml.SoulEnchantCost = 0;
                ml.RelicEnchantCost = 0;
                ml.FinalItemPower = dsl.ItemPowerLookup((ArtefactType)lookupItem.ArtefactType, ml.ListingTeir, newEl) + (int)ml.quality;

                if (newEl >= EnchantLevel.Rune && ml.ListingEnchantLevel < EnchantLevel.Rune)
                {
                    ml.RuneEnchantCost = (int)(EnchantRuneCount((SlotType)lookupItem.SlotType, lookupItem.TwoHanded) * enchPrices.GetPrice(ml.ListingTeir, EnchantLevel.Rune, out _));
                }
                if (newEl >= EnchantLevel.Soul && ml.ListingEnchantLevel < EnchantLevel.Soul)
                {
                    ml.SoulEnchantCost = (int)(EnchantRuneCount((SlotType)lookupItem.SlotType, lookupItem.TwoHanded) * enchPrices.GetPrice(ml.ListingTeir, EnchantLevel.Soul, out _));
                }
                if (newEl >= EnchantLevel.Relic && ml.ListingEnchantLevel < EnchantLevel.Relic)
                {
                    ml.RelicEnchantCost = (int)(EnchantRuneCount((SlotType)lookupItem.SlotType, lookupItem.TwoHanded) * enchPrices.GetPrice(ml.ListingTeir, EnchantLevel.Relic, out _));
                }
                return ml;
            }

            AppendMarketListings(sNormal);
            for (int i = listings.Count - 1; i >= 0; i--)
            {
                listings[i].FinalItemPower = dsl.ItemPowerLookup((ArtefactType)lookupItem.ArtefactType, listings[i].ListingTeir, EnchantLevel.Normal) + (int)listings[i].quality;
                listings.Add(SetEnchant(listings[i].DeepCopy(), EnchantLevel.Rune));
                listings.Add(SetEnchant(listings[i].DeepCopy(), EnchantLevel.Soul));
                listings.Add(SetEnchant(listings[i].DeepCopy(), EnchantLevel.Relic));
            }
            char[] enchChar = { '1', '2', '3' };
            EnchantLevel[][] addEnchants = new EnchantLevel[][] 
            { 
                new EnchantLevel[] { EnchantLevel.Soul, EnchantLevel.Relic },
                new EnchantLevel[] { EnchantLevel.Relic },
                new EnchantLevel[] { } 
            };
            for (int i = 0; i < enchChar.Length; i++)
            {
                int listingsStart = listings.Count;
                sEnchant[replaceIndexEnchant] = enchChar[i];
                AppendMarketListings(sEnchant);
                int listingsEnd = listings.Count;

                for (int j = listingsStart; j < listingsEnd; j++)
                {
                    listings[j].FinalItemPower = dsl.ItemPowerLookup((ArtefactType)lookupItem.ArtefactType, listings[j].ListingTeir, listings[j].ListingEnchantLevel) + (int)listings[j].quality;
                    for (int k = 0; k < addEnchants[i].Length; k++)
                        listings.Add(SetEnchant(listings[j].DeepCopy(), addEnchants[i][k]));
                }
            }

            var y = listings.OrderBy(x => x.FinalCostPerItemPower).ToArray();

            dgMarketResults.ItemsSource = y;
        }
    }
}