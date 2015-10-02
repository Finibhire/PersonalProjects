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
            string userName = null;
            int userId = 0;
            this.GetUserData(out userId, out userName);

            ResourceMarketIndexView model = new ResourceMarketIndexView();
            PopulateModelDisplayData(
                model,
                WorkingCurrencyTypeId,
                WorkingResourceTypeId,
                userName,
                userId);

            return View("Index", model);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddPurchaseOrder([Bind(Prefix = "AddPurchaseOrder")] AddOrder model)
        {
            string userName = null;
            int userId = 0;
            this.GetUserData(out userId, out userName);

            if (ModelState.IsValid)
            {
                try
                {
                    db.AddPurchaseOrder(
                        userId, 
                        model.WorkingResourceTypeId, 
                        model.ResourceAmount, 
                        (byte)model.WorkingCurrencyTypeId, 
                        model.CurrencyPerResource);
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    string message;
                    if (ex.InnerException != null &&
                        ex.InnerException.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                        ((System.Data.SqlClient.SqlException)ex.InnerException).Number == 547)
                    {
                        message = 
                            "Currency Per Resource must be non-negative and have 7 or less significant figures.  (DB Constraint Error)";
                        ModelState.AddModelError("AddPOCurrencyPerResource", message);
                    }
                    else
                    {
                        message =
                            "System.Data.Entity.Core.EntityCommandExecutionException: " + Environment.NewLine +
                            ex.InnerException.Message;
                        ModelState.AddModelError("DatabaseError", message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(
                        "DatabaseError", 
                        ex.GetType().ToString() + Environment.NewLine + ex.Message);
                }
            }

            ResourceMarketIndexView viewModel = new ResourceMarketIndexView() { AddPurchaseOrder = model };

            PopulateModelDisplayData(
                viewModel,
                model.WorkingCurrencyTypeId,
                model.WorkingResourceTypeId,
                userName,
                userId);

            return View("Index", viewModel);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddSellOrder([Bind(Prefix = "AddSellOrder")] AddOrder model)
        {
            string userName = null;
            int userId = 0;
            this.GetUserData(out userId, out userName);

            if (ModelState.IsValid)
            {
                try
                {
                    db.AddSellOrder(
                        userId,
                        model.WorkingResourceTypeId,
                        model.ResourceAmount,
                        (byte)model.WorkingCurrencyTypeId,
                        model.CurrencyPerResource);
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    string message;
                    if (ex.InnerException != null &&
                        ex.InnerException.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                        ((System.Data.SqlClient.SqlException)ex.InnerException).Number == 547)
                    {
                        message = "Currency Per Resource must be non-negative and have 7 or less significant figures.  (DB Constraint Error)";
                        ModelState.AddModelError("AddSOCurrencyPerResource", message);
                    }
                    message = 
                        "System.Data.Entity.Core.EntityCommandExecutionException: " + Environment.NewLine +
                        ex.InnerException.Message;
                    ModelState.AddModelError("DatabaseError", message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(
                        "DatabaseError",
                        ex.GetType().ToString() + Environment.NewLine + ex.Message);
                }
            }

            ResourceMarketIndexView viewModel = new ResourceMarketIndexView() { AddSellOrder = model };

            PopulateModelDisplayData(
                viewModel, 
                model.WorkingCurrencyTypeId, 
                model.WorkingResourceTypeId,
                userName,
                userId);

            return View("Index", viewModel);
        }

        private void PopulateModelDisplayData(
                        ResourceMarketIndexView model, 
                        int? WorkingCurrencyTypeId, 
                        int? WorkingResourceTypeId,
                        string userName,
                        int userId)
        {
            string workingCurrencyName, workingResourceName;
            int workingResourceTypeId;
            byte workingCurrencyTypeId;

            //get the requested working currencies and working resources or provide the defaults
            workingCurrencyName =
                db.CurrencyTypes
                .Where(x => x.Id == WorkingCurrencyTypeId)
                .Select(x => x.Name)
                .FirstOrDefault();
            if (workingCurrencyName == null)
            {
                var workingCurrencyInfo =
                    db.CurrencyTypes
                    .Select(x => new { x.Name, x.Id })
                    .OrderBy(x => x.Id)
                    .First();
                workingCurrencyName = workingCurrencyInfo.Name;
                workingCurrencyTypeId = workingCurrencyInfo.Id;
            }
            else
            {
                workingCurrencyTypeId = (byte)WorkingCurrencyTypeId;
            }
            model.WorkingCurrencyName = workingCurrencyName;
            model.WorkingCurrencyTypeId = workingCurrencyTypeId;
            model.AddPurchaseOrder.WorkingCurrencyTypeId = workingCurrencyTypeId;
            model.AddSellOrder.WorkingCurrencyTypeId = workingCurrencyTypeId;

            workingResourceName =
                db.ResourceTypes
                .Where(x => x.Id == WorkingResourceTypeId)
                .Select(x => x.Name)
                .FirstOrDefault();
            if (workingResourceName == null)
            {
                var workingResourceInfo =
                    db.ResourceTypes
                    .Select(x => new { x.Name, x.Id })
                    .OrderBy(x => x.Id)
                    .First();
                workingResourceName = workingResourceInfo.Name;
                workingResourceTypeId = workingResourceInfo.Id;
            }
            else
            {
                workingResourceTypeId = (int)WorkingResourceTypeId;
            }
            model.WorkingResourceName = workingResourceName;
            model.WorkingResourceTypeId = workingResourceTypeId;
            model.AddPurchaseOrder.WorkingResourceTypeId = workingResourceTypeId;
            model.AddSellOrder.WorkingResourceTypeId = workingResourceTypeId;

            model.Currencies =
                db.CurrencyTypes
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = x.Id == workingCurrencyTypeId
                });
            //model.CurrenciesSelectList = new SelectList(model.Currencies, "Id", "Name", (byte)workingCurrencyTypeId);
            model.Resources =
                db.ResourceTypes
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = x.Id == workingResourceTypeId
                });


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
                    CurrencyPerResource = (double)x.TotalCurrencyCost / (double)x.ResourcesSoldAmount,
                    TotalCostAmount = x.TotalCurrencyCost,
                    ClientParticipation = x.BuyerUserId == userId || x.SellerUserId == userId,
                    ClientIsBuyingResources = x.BuyerUserId == userId
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
                    CurrencyPerResource = (double)x.TotalCurrencyCost / (double)x.ResourcesSoldAmount,
                    TotalCostAmount = x.TotalCurrencyCost,
                    ResourceName = x.ResourceType.Name,
                    ClientIsBuyingResources = (x.BuyerUserId == userId)
                });

            model.AllPurchaseOrders =
                db.GetCondensedAndConvertedPurchaseOrders(workingCurrencyTypeId, workingResourceTypeId)
                .Select(x => new CondensedAndConvertedOrdersView()
                {
                    FillableResourceAmount = x.FillableResourceAmount,
                    OriginalCurrencyName = x.OriginalCurrencyName,
                    ExchangeRate = x.SourceMultiplier,
                    CurrencyPerResource = x.ConvertedCurrencyPerResource,
                    TotalCost = (decimal)(x.ConvertedCurrencyPerResource * x.FillableResourceAmount)
                });

            model.AllSellOrders =
                db.GetCondensedAndConvertedSellOrders(workingCurrencyTypeId, workingResourceTypeId)
                .Select(x => new CondensedAndConvertedOrdersView()
                {
                    FillableResourceAmount = x.FillableResourceAmount,
                    OriginalCurrencyName = x.OriginalCurrencyName,
                    ExchangeRate = x.SourceMultiplier,
                    CurrencyPerResource = x.ConvertedCurrencyPerResource,
                    TotalCost = (decimal)x.ConvertedCurrencyPerResource * (decimal)x.FillableResourceAmount
                });

            model.ClientPurchaseOrders =
                from po in db.PurchaseOrders
                where po.UserId == userId
                where po.ResourceTypeId == workingResourceTypeId
                orderby po.Id
                select new MarketOrderView()
                {
                    Id = po.Id,
                    ResourceOrderAmount = po.ResourceRequestAmount,
                    ResourceFilledAmount = po.ResourceFilledAmount,
                    OriginalCurrencyName = po.ResourceType.Name,
                    CurrencyPerResource = po.CurrencyPerResource
                };

            model.ClientSellOrders =
                from so in db.SellOrders
                where so.UserId == userId
                where so.ResourceTypeId == workingResourceTypeId
                orderby so.Id
                select new MarketOrderView()
                {
                    Id = so.Id,
                    ResourceOrderAmount = so.ResourceSellAmount,
                    ResourceFilledAmount = so.ResourceFilledAmount,
                    OriginalCurrencyName = so.ResourceType.Name,
                    CurrencyPerResource = so.CurrencyPerResource
                };

        }
    }
}