using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ResourceMarketIndexView
    {
        public class ConvertedOrder
        {
            public int ID { get; set; }
            public bool UserIsOwner { get; set; }
            public int RemainingResourceAmount { get; set; }
            public CurrencyType OriginalCurrency { get; set; }
            public decimal ExchangeRate { get; set; }
            public decimal CostEach { get; set; }
            public decimal TotalCost { get; set; }
        }
        public CurrencyType WorkingCurrency { get; set; }
        public ResourceType WorkingResource { get; set; }
        public ResourceSale[] RecentResourceSales { get; set; }
        public ResourceSale[] MyRecentTransactions { get; set; }
        public ConvertedOrder[] CurrentPurchaseOrders { get; set; }
        public ConvertedOrder[] CurrentSaleOrders { get; set; }
    }
    
}