using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AlbionPriceComparer.AOEndorsed;

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

    public enum GameItemQuality
    {
        Unknown = -1,
        Normal = 0,
        Good = 10,
        Outstanding = 20,
        Excelent = 50,
        Masterpeice = 100
    }

    [DataContract]
    public class MarketListing
    {
        public MarketListing DeepCopy()
        {
            return new MarketListing
            {
                buy_price_max = buy_price_max,
                buy_price_max_date = buy_price_max_date,
                buy_price_min = buy_price_min,
                buy_price_min_date = buy_price_min_date,
                city = city,
                item_id = item_id,
                quality = quality,
                RelicEnchantCost = RelicEnchantCost,
                RuneEnchantCost = RuneEnchantCost,
                sell_price_max = sell_price_max,
                sell_price_max_date = sell_price_max_date,
                sell_price_min = sell_price_min,
                sell_price_min_date = sell_price_min_date,
                SoulEnchantCost = SoulEnchantCost,
                FinalItemPower = FinalItemPower
            };
        }
        public int RuneEnchantCost { get; set; }
        public int SoulEnchantCost { get; set; }
        public int RelicEnchantCost { get; set; }
        public int FinalItemPower { get; set; }
        public int FinalCost
        {
            get => sell_price_min + RuneEnchantCost + SoulEnchantCost + RelicEnchantCost;
        }

        public double FinalCostPerItemPower
        {
            get
            {
                return Math.Round((double)FinalCost / (double)FinalItemPower, 2);
            }
        }
        public int ListingTeir { get; set; }
        public string ListingTeirEnchant
        {
            get => ListingTeir + "." + ((int)ListingEnchantLevel).ToString();
        }
        public string Id { get; set; }
        public EnchantLevel ListingEnchantLevel { get; private set; }
        public EnchantLevel FinalEnchantLevel
        {
            get
            {
                if (RelicEnchantCost > 0 || ListingEnchantLevel >= EnchantLevel.Relic)
                    return EnchantLevel.Relic;
                if (SoulEnchantCost > 0 || ListingEnchantLevel >= EnchantLevel.Soul)
                    return EnchantLevel.Soul;
                if (RuneEnchantCost > 0 || ListingEnchantLevel >= EnchantLevel.Rune)
                    return EnchantLevel.Rune;
                return EnchantLevel.Normal;
            }
        }

        [DataMember]
        private string item_id
        {
            get
            {
                return "T" + ListingTeir + "_" + Id + (ListingEnchantLevel == EnchantLevel.Normal ? "" : "@" + (int)ListingEnchantLevel);
            }
            set
            {
                ListingTeir = value[1] - '0';
                if (value[value.Length - 2] != '@')
                {
                    ListingEnchantLevel = EnchantLevel.Normal;
                    Id = value.Substring(3, value.Length - 3);
                }
                else
                {
                    ListingEnchantLevel = (EnchantLevel)(value[value.Length - 1] - '0');
                    Id = value.Substring(3, value.Length - 5);
                }
            }
        }
        [DataMember]
        public int sell_price_min { get; set; }
        public string SellPriceAge
        {
            get => Math.Round((DateTime.UtcNow - sell_price_min_date).TotalHours, 1) + " hours";
        }
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

        public GameItemQuality quality { get; set; }

        [DataMember(Name = "quality")]
        private int quality_id
        {
            get => ItemQualityIdLookup(quality);
            set => quality = ItemQualityLookup(value);
        }

        private static GameItemQuality ItemQualityLookup(int iq)
        {
            switch (iq)
            {
                case 1:
                    return GameItemQuality.Normal;
                case 2:
                    return GameItemQuality.Good;
                case 3:
                    return GameItemQuality.Outstanding;
                case 4:
                    return GameItemQuality.Excelent;
                case 5:
                    return GameItemQuality.Masterpeice;
                default:
                    return GameItemQuality.Unknown;
            }
        }

        private static int ItemQualityIdLookup(GameItemQuality iq)
        {
            switch (iq)
            {
                case GameItemQuality.Normal:
                    return 1;
                case GameItemQuality.Good:
                    return 2;
                case GameItemQuality.Outstanding:
                    return 3;
                case GameItemQuality.Excelent:
                    return 4;
                case GameItemQuality.Masterpeice:
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

    [DataContract]
    public class BinItemJSON
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

    public enum EnchantLevel
    {
        Unknown = -1, Normal = 0, Rune, Soul, Relic
    }

    public enum ArtefactType
    {
        Unknown = -1, Normal = 0, Rune, Soul, Relic
    }

    public enum SlotType
    {
        unknown = 0, armor = 1, bag, cape, head, mainhand, offhand, shoes
    }
}
