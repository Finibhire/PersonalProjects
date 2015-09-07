using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        public ActionResult ResourceMarketDemo(ResourceMarketSubmission subData)
        {
            ViewBag.Message = "ViewBag.Message";

            if (string.IsNullOrWhiteSpace((string)Session["UserName"]))
            {
                Response.Redirect("~/Home/SelectUser.html");
                return null;
            }

            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SelectUser(SelectUserSubmission subData)
        {
            if (!string.IsNullOrWhiteSpace(subData.LoginAs))
            {
                Session["UserName"] = subData.LoginAs;
            }
            else
            {
                if (Session["UserName"] == null)
                {
                    if (!DBSimulation.Users.ContainsKey("Finibhire"))
                    {
                        User fini = new Models.User();
                        fini.UserName = "Finibhire";
                        fini.DragonPoints = 1337;
                        fini.FLAPCoin = 5555;
                        fini.Gold = 100000;
                        fini.GoldPieceCoin = 4.589034m;
                        fini.HTML5Coin = 900;
                        fini.HyperCoin = 0.069699m;
                        DBSimulation.Users.Add("Finibhire", fini);
                    }
                    Session["UserName"] = "Finibhire";
                }
            }

            ViewBag.Users = DBSimulation.Users;

            return View(subData);
        }
    }
}