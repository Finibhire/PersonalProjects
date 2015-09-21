using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ResourceMarketDemo.Models
{
    /// <summary>
    /// View Formated User
    /// </summary>
    public class VFUser
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public string UserName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F0}")]
        public decimal? DragonPoints { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F0}")]
        public decimal? Gold { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#############}")]
        public decimal? HyperCoin { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#############}")]
        public decimal? GoldPieceCoin { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#############}")]
        public decimal? HTML5Coin { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#############}")]
        public decimal? FLAPCoin { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:D0}")]
        public long? Wood { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:D0}")]
        public long? Iron { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:D0}")]
        public long? Stone { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:D0}")]
        public long? Fish { get; set; }
    }

    public class PurchaseOrder
    {
        public int ID { get; set; }
        
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public string UserName { get; set; }

        [Required]
        [EnumDataType(typeof(ResourceType))]
        public ResourceType ResourceType { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ResourceRequestAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int ResourceFilledAmount { get; set; }

        [Required]
        [EnumDataType(typeof(CurrencyType))]
        public CurrencyType CurrencyType { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal CurrencyPerResource { get; set; }
    }

    public class SaleOrder
    {
        public int ID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public string UserName { get; set; }

        [Required]
        [EnumDataType(typeof(ResourceType))]
        public ResourceType ResourceType { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ResourceSellAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int ResourceFilledAmount { get; set; }

        [Required]
        [EnumDataType(typeof(CurrencyType))]
        public CurrencyType CurrencyType { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal CurrencyPerResource { get; set; }
    }

    public class ResourceSale
    {
        public int ID { get; set; }
        public DateTime SaleTime { get; set; }
        public ResourceType ResourceType { get; set; }
        public string SellerUserName { get; set; }
        public string BuyerUserName { get; set; }
        public int ResourceAmount { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public decimal TotalCostAmount { get; set; }
    }

    public class CurrencyConversion
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public CurrencyType SourceCurrencyType { get; set; }
        public CurrencyType DestinationCurrencyType { get; set; }
        //public decimal ConvertMultiplier { get; set; }
        //public decimal SourceTransactionFee { get; set; }
        public decimal SourceAmount { get; set; }
        public decimal DestinationAmount { get; set; }
    }

    public class CurrencyExchangeRate
    {
        public CurrencyType SourceCurrencyType { get; set; }
        public CurrencyType DestinationCurrencyType { get; set; }
        public decimal ConversionSourceMultiplier { get; set; }
    }
}