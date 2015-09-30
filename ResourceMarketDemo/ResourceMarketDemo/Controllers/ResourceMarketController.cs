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
                model.WorkingCurrencyTypeId,
                model.WorkingResourceTypeId,
                userName,
                userId);

            return View("Index", model);
        }

        private const string apoWhiteList =
            "WorkingCurrencyTypeId," +
            "WorkingResourceTypeId," +
            "AddPOResourceAmount," +
            "AddPOCurrencyPerResource";
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddPurchaseOrder([Bind(Include = apoWhiteList)] ResourceMarketIndexView model)
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
                        model.AddPOResourceAmount, 
                        (byte)model.WorkingCurrencyTypeId, 
                        model.AddPOCurrencyPerResource);
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
                            "System.Data.Entity.Core.EntityCommandExecutionException" + Environment.NewLine +
                            ex.Message;
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

            PopulateModelDisplayData(
                model,
                model.WorkingCurrencyTypeId,
                model.WorkingResourceTypeId,
                userName,
                userId);

            return View("Index", model);
        }

        private const string asoWhiteList =
            "WorkingCurrencyTypeId," +
            "WorkingResourceTypeId," +
            "AddSellOrderData.ResourceAmount," +
            "AddSellOrderData.CurrencyPerResource";
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddSellOrder([Bind(Include = asoWhiteList)] ResourceMarketIndexView model)
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
                        model.AddSOResourceAmount,
                        (byte)model.WorkingCurrencyTypeId,
                        model.AddSOCurrencyPerResource);
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
                        "System.Data.Entity.Core.EntityCommandExecutionException" + Environment.NewLine +
                        ex.Message;
                    ModelState.AddModelError("DatabaseError", message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(
                        "DatabaseError",
                        ex.GetType().ToString() + Environment.NewLine + ex.Message);
                }
            }

            PopulateModelDisplayData(
                model, 
                model.WorkingCurrencyTypeId, 
                model.WorkingResourceTypeId,
                userName,
                userId);

            return View("Index", model);
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