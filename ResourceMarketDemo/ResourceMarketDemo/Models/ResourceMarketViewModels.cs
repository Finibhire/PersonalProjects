using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResourceMarketDemo.DBModels;
using System.ComponentModel.DataAnnotations;

namespace ResourceMarketDemo.Models
{
    public class ResourceMarketIndexView
    {
        public string WorkingCurrencyName { get; set; }
        public string WorkingResourceName { get; set; }
        [Required]
        public int WorkingCurrencyTypeId { get; set; }
        [Required]
        public int WorkingResourceTypeId { get; set; }
        //public CurrencyType WorkingCurrency { get; set; }
        //public ResourceType WorkingResource { get; set; }
        public IEnumerable<ResourceSaleView> RecentResourceSales { get; set; }
        public IEnumerable<ClientResourceSaleView> ClientRecentTransactions { get; set; }
        public IEnumerable<ConvertedOrderView> CurrentPurchaseOrders { get; set; }
        public IEnumerable<ConvertedOrderView> CurrentSellOrders { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AddPOResourceAmount { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal AddPOCurrencyPerResource { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AddSOResourceAmount { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal AddSOCurrencyPerResource { get; set; }
    }

    public class ConvertedOrderView
    {
        public int Id { get; set; }
        public bool ClientIsOwner { get; set; }
        public int RemainingResourceAmount { get; set; }
        public string OriginalCurrencyName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public decimal ExchangeRate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public decimal CurrencyPerResource { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public decimal TotalCost { get; set; }
    }

    public class ResourceSaleView
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd HH:mm}")]
        public DateTime SaleTime { get; set; }

        public string CurrencyName { get; set; }

        public int AmountSold { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public decimal TotalCostAmount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public decimal CurrencyPerResource { get; set; }
    }

    public class ClientResourceSaleView : ResourceSaleView
    {
        public string ResourceName { get; set; }
        public bool ClientIsBuyingResources { get; set; }
    }

    //public class NewOrderPostData
    //{
    //    [Required]
    //    [Range(0, int.MaxValue)]
    //    public int ResourceAmount { get; set; }

    //    [Required]
    //    [Range(0, (double)decimal.MaxValue)]
    //    public decimal CurrencyPerResource { get; set; }
    //}

    public static class FormatingExtensionMethods
    {
        public static string GetRMDReadableFormat(this decimal dec)
        {
            if (dec == 0)
                return "0";

            if (dec >= 1 || dec * 100 >= 1)
            {
                decimal whole = Math.Floor(dec);
                dec -= whole;
                return (Math.Ceiling(dec * 1000000000) / 1000000000 + whole).ToString("0.#########");
            }

            int exponent = 0;
            while (dec < 1)
            {
                exponent += 3;
                dec *= 1000;
            }
            
            return dec.ToString("0.######") + "E-" + exponent;
        }
    }
}