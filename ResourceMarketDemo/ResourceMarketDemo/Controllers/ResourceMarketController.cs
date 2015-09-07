using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ResourceMarketController : Controller
    {
        // GET: ResourceMarket
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(CurrencyType? WorkingCurrency, ResourceType? WorkingResource)
        {
            string userName = (string)Session["UserName"];
            ResourceMarketIndexView model = new ResourceMarketIndexView();
            model.WorkingCurrency = WorkingCurrency ?? CurrencyType.Gold;
            model.WorkingResource = WorkingResource ?? ResourceType.Wood;
            model.RecentResourceSales =
                DBSimulation.ResourceSales.Values.OrderByDescending(x => x.SaleTime).Take(10).ToArray();
            model.MyRecentTransactions =
                DBSimulation.ResourceSales.Values
                .Where(x => x.BuyerUserName == userName || x.SellerUserName == userName)
                .Take(10)
                .ToArray();

            model.CurrentPurchaseOrders = (
                from po in DBSimulation.PurchaseOrders.Values
                where po.ResourceRequestAmount - po.ResourceFilledAmount > 0
                where po.ResourceType == model.WorkingResource
                join cer in DBSimulation.CurrencyExchangeRates
                    on new { SourceCurrencyType = po.CurrencyType, DestinationCurrencyType = model.WorkingCurrency }
                    equals new { cer.SourceCurrencyType, cer.DestinationCurrencyType }
                    into intoCer
                from leftCer in intoCer.DefaultIfEmpty()
                where leftCer != null || po.CurrencyType == model.WorkingCurrency
                orderby po.CurrencyPerResource * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier) descending
                select new ResourceMarketIndexView.ConvertedOrder
                {
                    ID = po.ID,
                    OriginalCurrency = po.CurrencyType,
                    CostEach = po.CurrencyPerResource * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier),
                    ExchangeRate = leftCer == null ? 1m : leftCer.ConversionSourceMultiplier,
                    RemainingResourceAmount = po.ResourceRequestAmount - po.ResourceFilledAmount,
                    TotalCost = (po.ResourceRequestAmount - po.ResourceFilledAmount) * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier),
                    UserIsOwner = po.UserName == userName
                }
                ).ToArray();

            model.CurrentSaleOrders = (
                from so in DBSimulation.SaleOrders.Values
                where so.ResourceSellAmount - so.ResourceFilledAmount > 0
                where so.ResourceType == model.WorkingResource
                join cer in DBSimulation.CurrencyExchangeRates
                    on new { SourceCurrencyType = so.CurrencyType, DestinationCurrencyType = model.WorkingCurrency }
                    equals new { cer.SourceCurrencyType, cer.DestinationCurrencyType }
                    into intoCer
                from leftCer in intoCer.DefaultIfEmpty()
                where leftCer != null || so.CurrencyType == model.WorkingCurrency
                orderby so.CurrencyPerResource * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier) ascending
                select new ResourceMarketIndexView.ConvertedOrder
                {
                    ID = so.ID,
                    OriginalCurrency = so.CurrencyType,
                    CostEach = so.CurrencyPerResource * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier),
                    ExchangeRate = leftCer == null ? 1m : leftCer.ConversionSourceMultiplier,
                    RemainingResourceAmount = so.ResourceSellAmount - so.ResourceFilledAmount,
                    TotalCost = (so.ResourceSellAmount - so.ResourceFilledAmount) * (leftCer == null ? 1m : leftCer.ConversionSourceMultiplier),
                    UserIsOwner = so.UserName == userName
                }
                ).ToArray();

            return View("Index", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreatePurchaseOrder([Bind(Exclude = "ID,UserName,ResourceFilledAmount")] PurchaseOrder po)
        {
            string userName = (string)Session["UserName"];
            po.UserName = userName;
            //po.ResourceFilledAmount = 0;

            if (ModelState.IsValid)
            {
                try
                {
                    DBSimulation.ProcAddPurchaseOrder(po);
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(ex.ParamName, ex.Message);
                }
            }

            return Index(po.CurrencyType, po.ResourceType);
        }
    }
}