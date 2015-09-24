using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceMarketDemo.Models;
using ResourceMarketDemo.DBModels;

namespace ResourceMarketDemo.Controllers
{
    [Authorize]
    public class ResourceMarketController : Controller
    {
        RMDDatabaseEntities db = new RMDDatabaseEntities();

        // GET: ResourceMarket
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(int? WorkingCurrencyTypeId, int? WorkingResourceTypeId)
        {
            ResourceMarketIndexView model = new ResourceMarketIndexView();
            PopulateModelDisplayData(model, WorkingCurrencyTypeId, WorkingResourceTypeId);

            return View("Index", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddPurchaseOrder(NewOrderPostData data)
        {
            string userName = (string)Session["UserName"];
            int userId = (int)Session["UserId"];

            if (ModelState.IsValid)
            {
                try
                {
                    db.AddPurchaseOrder(userId, data.ResourceTypeId, data.ResourceAmount, (byte)data.CurrencyTypeId, data.CurrencyPerResource);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("DatabaseError", ex);
                }
            }

            ResourceMarketIndexView model = new ResourceMarketIndexView();
            PopulateModelDisplayData(model, data.CurrencyTypeId, data.ResourceTypeId);
            model.AddPurchaseOrderData = data;

            return View("Index", model);
        }

        private void PopulateModelDisplayData(ResourceMarketIndexView model, int? WorkingCurrencyTypeId, int? WorkingResourceTypeId)
        {
            string workingCurrencyName, workingResourceName;
            int workingResourceTypeId;
            byte workingCurrencyTypeId;

            string userName = (string)Session["UserName"];
            int userId = (int)Session["UserId"];

            //get the requested working currencies and working resources or provide the defaults
            model.WorkingCurrencyName =
                db.CurrencyTypes
                .Where(x => x.Id == WorkingCurrencyTypeId)
                .Select(x => x.Name)
                .FirstOrDefault();
            if (model.WorkingCurrencyName == null)
            {
                var workingCurrencyInfo =
                    db.CurrencyTypes
                    .Select(x => new { x.Name, x.Id })
                    .OrderBy(x => x.Id)
                    .First();
                model.WorkingCurrencyName = workingCurrencyInfo.Name;
                model.WorkingCurrencyTypeId = workingCurrencyInfo.Id;
            }
            workingCurrencyTypeId = (byte)model.WorkingCurrencyTypeId;
            workingCurrencyName = model.WorkingCurrencyName;

            model.WorkingResourceName =
                db.ResourceTypes
                .Where(x => x.Id == WorkingResourceTypeId)
                .Select(x => x.Name)
                .FirstOrDefault();
            if (model.WorkingResourceName == null)
            {
                var workingResourceInfo =
                    db.ResourceTypes
                    .Select(x => new { x.Name, x.Id })
                    .OrderBy(x => x.Id)
                    .First();
                model.WorkingResourceName = workingResourceInfo.Name;
                model.WorkingResourceTypeId = workingResourceInfo.Id;
            }
            workingResourceTypeId = model.WorkingResourceTypeId;
            workingResourceName = model.WorkingResourceName;

            //populate what's needed for the html view tables
            model.RecentResourceSales =
                db.MarketSales
                .Where(x => x.ResourceTypeId == workingResourceTypeId)
                .OrderBy(x => x.TimeStamp)
                .Select(x => new ResourceSaleView()
                {
                    SaleTime = x.TimeStamp,
                    CurrencyName = x.CurrencyType.Name,
                    AmountSold = x.ResourcesSoldAmount,
                    CurrencyPerResource = x.TotalCurrencyCost / (decimal)x.ResourcesSoldAmount,
                    TotalCostAmount = x.TotalCurrencyCost
                });

            model.ClientRecentTransactions =
                db.MarketSales
                .Where(x => x.BuyerUserId == userId || x.SellerUserId == userId)
                .OrderBy(x => x.TimeStamp)
                .Select(x => new ClientResourceSaleView()
                {
                    SaleTime = x.TimeStamp,
                    CurrencyName = x.CurrencyType.Name,
                    AmountSold = x.ResourcesSoldAmount,
                    CurrencyPerResource = x.TotalCurrencyCost / (decimal)x.ResourcesSoldAmount,
                    TotalCostAmount = x.TotalCurrencyCost,
                    ResourceName = x.ResourceType.Name,
                    ClientIsBuyingResources = (x.BuyerUserId == userId)
                });

            model.CurrentPurchaseOrders =
                db.GetConvertedPurchaseOrders(workingCurrencyTypeId, workingResourceTypeId)
                .Select(x => new ConvertedOrderView()
                {
                    Id = x.Id,
                    ClientIsOwner = x.UserId == userId,
                    RemainingResourceAmount = x.ToBeFilledAmount,
                    OriginalCurrencyName = x.OriginalCurrencyName,
                    ExchangeRate = x.SourceMultiplier,
                    CurrencyPerResource = (decimal)x.ConvertedCurrencyPerResource,
                    TotalCost = (decimal)(x.ConvertedCurrencyPerResource * x.ToBeFilledAmount)
                });

            model.CurrentSellOrders =
                db.GetConvertedSellOrders(workingCurrencyTypeId, workingResourceTypeId)
                .Select(x => new ConvertedOrderView()
                {
                    Id = x.Id,
                    ClientIsOwner = x.UserId == userId,
                    RemainingResourceAmount = x.ToBeFilledAmount,
                    OriginalCurrencyName = x.OriginalCurrencyName,
                    ExchangeRate = x.SourceMultiplier,
                    CurrencyPerResource = (decimal)x.ConvertedCurrencyPerResource,
                    TotalCost = (decimal)x.ConvertedCurrencyPerResource * (decimal)x.ToBeFilledAmount
                });
        }
    }
}