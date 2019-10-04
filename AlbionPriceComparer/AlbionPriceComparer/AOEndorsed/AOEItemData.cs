using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlbionPriceComparer.AOEndorsed
{


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

    [DataContract]
    public class AOEItemData
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

        public AOEItemData()
        {
            teir = 0;
        }
    }
}
