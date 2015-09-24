using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceMarketDemo.Models
{
    public static class DBSimulation
    {
        //static SortedDictionary<string, User> _Users = new SortedDictionary<string, User>();
        static SortedDictionary<int, PurchaseOrder> _PurchaseOrders = new SortedDictionary<int, PurchaseOrder>();
        static SortedDictionary<int, SaleOrder> _SaleOrders = new SortedDictionary<int, SaleOrder>();
        static SortedDictionary<int, ResourceSale> _ResourceSales = new SortedDictionary<int, ResourceSale>();
        static SortedDictionary<int, CurrencyConversion> _CurrencyConversions = new SortedDictionary<int, CurrencyConversion>();
        static CurrencyExchangeRate[] _CurrencyExchangeRates;


        public const decimal DPtoGold = 10m;
        public const decimal DPtoHyperCoin = 0.00008894m;
        public const decimal DPtoGoldPiecesCoin = 0.00024041m;
        public const decimal DPtoHTML5Coin = 8.22637000m;
        public const decimal DPtoFLAPCoin = 6.37544000m;
        public const decimal ConvertTaxRate = 0.85m; //used when converting from DP to a crypto-currency

        //public static SortedDictionary<string, User> Users
        //{
        //    get
        //    {
        //        return _Users;
        //    }
        //}
        public static SortedDictionary<int, PurchaseOrder> PurchaseOrders
        {
            get
            {
                return _PurchaseOrders;
            }
        }
        public static SortedDictionary<int, SaleOrder> SaleOrders
        {
            get
            {
                return _SaleOrders;
            }
        }
        public static SortedDictionary<int, ResourceSale> ResourceSales
        {
            get
            {
                return _ResourceSales;
            }
        }
        public static SortedDictionary<int, CurrencyConversion> CurrencyConversions
        {
            get
            {
                return _CurrencyConversions;
            }
        }
        public static CurrencyExchangeRate[] CurrencyExchangeRates
        {
            get
            {
                if (_CurrencyExchangeRates != null)
                {
                    return _CurrencyExchangeRates;
                }
                _CurrencyExchangeRates = new CurrencyExchangeRate[25];
                //Note there is no conversion for gold to other currencies by design
                _CurrencyExchangeRates[0] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.DragonPoints,
                    DestinationCurrencyType = CurrencyType.Gold,
                    ConversionSourceMultiplier = DPtoGold
                };
                _CurrencyExchangeRates[1] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.DragonPoints,
                    DestinationCurrencyType = CurrencyType.HyperCoin,
                    ConversionSourceMultiplier = DPtoHyperCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[2] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.DragonPoints,
                    DestinationCurrencyType = CurrencyType.GoldPiecesCoin,
                    ConversionSourceMultiplier = DPtoGoldPiecesCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[3] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.DragonPoints,
                    DestinationCurrencyType = CurrencyType.HTML5Coin,
                    ConversionSourceMultiplier = DPtoHTML5Coin * ConvertTaxRate
                };
                _CurrencyExchangeRates[4] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.DragonPoints,
                    DestinationCurrencyType = CurrencyType.FLAP,
                    ConversionSourceMultiplier = DPtoFLAPCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[5] = new CurrencyExchangeRate()
                {// hyper/dptohyper = dp; dp*dptogold = gold; >>> hyper/dptohyper*dptogold = gold
                    SourceCurrencyType = CurrencyType.HyperCoin,
                    DestinationCurrencyType = CurrencyType.Gold,
                    ConversionSourceMultiplier = DPtoGold / DPtoHyperCoin
                };
                _CurrencyExchangeRates[6] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HyperCoin,
                    DestinationCurrencyType = CurrencyType.DragonPoints,
                    ConversionSourceMultiplier = 1m / DPtoHyperCoin
                };
                _CurrencyExchangeRates[7] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HyperCoin,
                    DestinationCurrencyType = CurrencyType.GoldPiecesCoin,
                    ConversionSourceMultiplier = DPtoGoldPiecesCoin / DPtoHyperCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[8] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HyperCoin,
                    DestinationCurrencyType = CurrencyType.HTML5Coin,
                    ConversionSourceMultiplier = DPtoHTML5Coin / DPtoHyperCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[9] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HyperCoin,
                    DestinationCurrencyType = CurrencyType.FLAP,
                    ConversionSourceMultiplier = DPtoFLAPCoin / DPtoHyperCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[10] = new CurrencyExchangeRate()
                {// hyper/dptohyper = dp; dp*dptogold = gold; >>> hyper/dptohyper*dptogold = gold
                    SourceCurrencyType = CurrencyType.GoldPiecesCoin,
                    DestinationCurrencyType = CurrencyType.Gold,
                    ConversionSourceMultiplier = DPtoGold / DPtoGoldPiecesCoin
                };
                _CurrencyExchangeRates[11] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.GoldPiecesCoin,
                    DestinationCurrencyType = CurrencyType.DragonPoints,
                    ConversionSourceMultiplier = 1m / DPtoGoldPiecesCoin
                };
                _CurrencyExchangeRates[12] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.GoldPiecesCoin,
                    DestinationCurrencyType = CurrencyType.GoldPiecesCoin,
                    ConversionSourceMultiplier = DPtoGoldPiecesCoin / DPtoGoldPiecesCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[13] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.GoldPiecesCoin,
                    DestinationCurrencyType = CurrencyType.HTML5Coin,
                    ConversionSourceMultiplier = DPtoHTML5Coin / DPtoGoldPiecesCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[14] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.GoldPiecesCoin,
                    DestinationCurrencyType = CurrencyType.FLAP,
                    ConversionSourceMultiplier = DPtoFLAPCoin / DPtoGoldPiecesCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[15] = new CurrencyExchangeRate()
                {// hyper/dptohyper = dp; dp*dptogold = gold; >>> hyper/dptohyper*dptogold = gold
                    SourceCurrencyType = CurrencyType.HTML5Coin,
                    DestinationCurrencyType = CurrencyType.Gold,
                    ConversionSourceMultiplier = DPtoGold / DPtoHTML5Coin
                };
                _CurrencyExchangeRates[16] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HTML5Coin,
                    DestinationCurrencyType = CurrencyType.DragonPoints,
                    ConversionSourceMultiplier = 1m / DPtoHTML5Coin
                };
                _CurrencyExchangeRates[17] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HTML5Coin,
                    DestinationCurrencyType = CurrencyType.GoldPiecesCoin,
                    ConversionSourceMultiplier = DPtoGoldPiecesCoin / DPtoHTML5Coin * ConvertTaxRate
                };
                _CurrencyExchangeRates[18] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HTML5Coin,
                    DestinationCurrencyType = CurrencyType.HTML5Coin,
                    ConversionSourceMultiplier = DPtoHTML5Coin / DPtoHTML5Coin * ConvertTaxRate
                };
                _CurrencyExchangeRates[19] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.HTML5Coin,
                    DestinationCurrencyType = CurrencyType.FLAP,
                    ConversionSourceMultiplier = DPtoFLAPCoin / DPtoHTML5Coin * ConvertTaxRate
                };
                _CurrencyExchangeRates[20] = new CurrencyExchangeRate()
                {// hyper/dptohyper = dp; dp*dptogold = gold; >>> hyper/dptohyper*dptogold = gold
                    SourceCurrencyType = CurrencyType.FLAP,
                    DestinationCurrencyType = CurrencyType.Gold,
                    ConversionSourceMultiplier = DPtoGold / DPtoFLAPCoin
                };
                _CurrencyExchangeRates[21] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.FLAP,
                    DestinationCurrencyType = CurrencyType.DragonPoints,
                    ConversionSourceMultiplier = 1m / DPtoFLAPCoin
                };
                _CurrencyExchangeRates[22] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.FLAP,
                    DestinationCurrencyType = CurrencyType.GoldPiecesCoin,
                    ConversionSourceMultiplier = DPtoGoldPiecesCoin / DPtoFLAPCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[23] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.FLAP,
                    DestinationCurrencyType = CurrencyType.HTML5Coin,
                    ConversionSourceMultiplier = DPtoHTML5Coin / DPtoFLAPCoin * ConvertTaxRate
                };
                _CurrencyExchangeRates[24] = new CurrencyExchangeRate()
                {
                    SourceCurrencyType = CurrencyType.FLAP,
                    DestinationCurrencyType = CurrencyType.FLAP,
                    ConversionSourceMultiplier = DPtoFLAPCoin / DPtoFLAPCoin * ConvertTaxRate
                };
                return _CurrencyExchangeRates;
            }
        }

        /// <summary>
        /// Retrives a list of the best sales orders that in order of the highest paying sales orders
        /// considering the currency conversion rate.
        /// </summary>
        /// <returns></returns>
        //public static IEnumerable<ResourceMarketIndexView.ConvertedOrder> OrderedBestSaleOrders(ResourceType WorkingResource, CurrencyType WorkingCurrency)
        //{
        //    IEnumerable<ResourceMarketIndexView.ConvertedOrder> saleOrders =
        //        from so in DBSimulation.SaleOrders.Values
        //        where so.ResourceSellAmount - so.ResourceFilledAmount > 0
        //        where so.ResourceType == WorkingResource
        //        join cer in DBSimulation.CurrencyExchangeRates
        //            on new { SourceCurrencyType = so.CurrencyType, DestinationCurrencyType = WorkingCurrency }
        //            equals new { cer.SourceCurrencyType, cer.DestinationCurrencyType }
        //            into intoCer
        //        from leftCer in intoCer.DefaultIfEmpty()
        //        where leftCer != null || so.CurrencyType == WorkingCurrency
        //        orderby so.CurrencyPerResource * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier) ascending
        //        select new ResourceMarketIndexView.ConvertedOrder
        //        {
        //            ID = so.ID,
        //            OriginalCurrency = so.CurrencyType,
        //            CostEach = so.CurrencyPerResource * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier),
        //            ExchangeRate = leftCer == null ? 1m : leftCer.ConversionSourceMultiplier,
        //            RemainingResourceAmount = so.ResourceSellAmount - so.ResourceFilledAmount,
        //            TotalCost = (so.ResourceSellAmount - so.ResourceFilledAmount) * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier)
        //        };

        //    return saleOrders;
        //}

        ///// <summary>
        ///// when currencyPerResource is less than zero it is ignored and the maximum is bought at any price
        ///// </summary>
        ///// <param name="po"></param>
        //public static void ProcAddPurchaseOrder(PurchaseOrder po)
        //{
        //    po.ID = (new Random()).Next();

        //    if (po.UserName == null)
        //        throw new ArgumentNullException("UserName");
        //    //if (!_Users.ContainsKey(po.UserName))
        //    //    throw new ArgumentException("UserName does not exist in the database", "UserName");
        //    //User user = _Users[po.UserName];

        //    var qualifyingSaleOrders = OrderedBestSaleOrders(po.ResourceType, po.CurrencyType);
        //    if (po.CurrencyPerResource >= 0)
        //        qualifyingSaleOrders = qualifyingSaleOrders.Where(x => x.CostEach >= po.CurrencyPerResource);

        //    List<CurrencyConversion> curConTrans = new List<CurrencyConversion>();
        //    List<SaleOrder> saleOrderTrans = new List<SaleOrder>();


        //    bool filled = false;
        //    po.ResourceFilledAmount = 0;
        //    foreach (var order in qualifyingSaleOrders)
        //    {
        //        int resourceBuyAmount = order.RemainingResourceAmount;
        //        if (po.ResourceRequestAmount - po.ResourceFilledAmount <= resourceBuyAmount)
        //        {
        //            resourceBuyAmount = po.ResourceRequestAmount - po.ResourceFilledAmount;
        //            filled = true;
        //        }

        //    }
            

        //    decimal totalCost = po.CurrencyPerResource * (decimal)po.ResourceRequestAmount;
        //    bool enoughCurrency = false;
        //    switch (po.CurrencyType)
        //    {
        //        case CurrencyType.DragonPoints:
        //            if (user.DragonPoints > totalCost)
        //            {
        //                enoughCurrency = true;
        //                user.DragonPoints -= totalCost;
        //            }
        //            break;
        //        case CurrencyType.FLAP:
        //            if (user.FLAPCoin > totalCost)
        //            {
        //                enoughCurrency = true;
        //                user.FLAPCoin -= totalCost;
        //            }
        //            break;
        //        case CurrencyType.Gold:
        //            if (user.Gold > totalCost)
        //            {
        //                enoughCurrency = true;
        //                user.Gold -= totalCost;
        //            }
        //            break;
        //        case CurrencyType.GoldPiecesCoin:
        //            if (user.GoldPieceCoin > totalCost)
        //            {
        //                enoughCurrency = true;
        //                user.GoldPieceCoin -= totalCost;
        //            }
        //            break;
        //        case CurrencyType.HTML5Coin:
        //            if (user.HTML5Coin > totalCost)
        //            {
        //                enoughCurrency = true;
        //                user.HTML5Coin -= totalCost;
        //            }
        //            break;
        //        case CurrencyType.HyperCoin:
        //            if (user.HyperCoin > totalCost)
        //            {
        //                enoughCurrency = true;
        //                user.HyperCoin -= totalCost;
        //            }
        //            break;
        //    }
        //    if (!enoughCurrency)
        //        throw new ArgumentException(po.UserName + " does not have enough " + po.CurrencyType + " to create this Purchase Order.", "CurrencyPerResource");

        //    _PurchaseOrders.Add(po.ID, po);
        //}
    }
}