﻿using System;
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
        public int? WorkingCurrencyTypeId { get; set; }
        public int? WorkingResourceTypeId { get; set; }
        public IEnumerable<ResourceSaleView> RecentResourceSales { get; set; }
        public IEnumerable<ClientResourceSaleView> ClientRecentTransactions { get; set; }
        public IEnumerable<CondensedAndConvertedOrdersView> AllPurchaseOrders { get; set; }
        public IEnumerable<CondensedAndConvertedOrdersView> AllSellOrders { get; set; }
        public IEnumerable<MarketOrderView> ClientPurchaseOrders { get; set; }
        public IEnumerable<MarketOrderView> ClientSellOrders { get; set; }

        public AddOrder AddPurchaseOrder { get; set; }
        public AddOrder AddSellOrder { get; set; }

        public ResourceMarketIndexView()
        {
            AddPurchaseOrder = new AddOrder();
            AddSellOrder = new AddOrder();
        }
    }

    public class AddOrder
    {
        [Required]
        public int? WorkingCurrencyTypeId { get; set; }

        [Required]
        public int? WorkingResourceTypeId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int? ResourceAmount { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public double? CurrencyPerResource { get; set; }
    }

    public class MarketOrderView
    {
        public int Id { get; set; }
        public int ResourceOrderAmount { get; set; }
        public int ResourceFilledAmount { get; set; }
        public string OriginalCurrencyName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public double CurrencyPerResource { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        //public decimal RemainingTotalCost { get; set; }
    }

    public class CondensedAndConvertedOrdersView
    {
        public int FillableResourceAmount { get; set; }
        public string OriginalCurrencyName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public double ExchangeRate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.#########}")]
        public double CurrencyPerResource { get; set; }
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
        public double CurrencyPerResource { get; set; }

        public bool ClientParticipation { get; set; }

        public bool ClientIsBuyingResources { get; set; }
    }

    public class ClientResourceSaleView : ResourceSaleView
    {
        public string ResourceName { get; set; }
    }

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

        public static string GetRMDReadableFormat(this double dec)
        {
            if (dec == 0)
                return "0";

            if (dec >= 1 || dec * 100 >= 1)
            {
                double whole = Math.Floor(dec);
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