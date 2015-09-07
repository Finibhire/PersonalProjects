using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ManualEntryController : Controller
    {
        // GET: ManualEntry
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreatePurchaseOrder(PurchaseOrder po)
        {
            if (!DBSimulation.PurchaseOrders.ContainsKey(po.ID))
            {
                DBSimulation.PurchaseOrders[po.ID] = po;
            }

            po = new PurchaseOrder();
            po.UserName = (string)Session["UserName"];
            po.ID = (new Random()).Next();
            po.CurrencyType = CurrencyType.Gold;
            po.ResourceType = ResourceType.Wood;

            return View("CreatePurchaseOrder", po);
        }
    }
}