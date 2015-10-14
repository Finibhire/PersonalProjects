using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceMarketDemo.Models;

namespace ResourceMarketDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectPermanent("~/");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}