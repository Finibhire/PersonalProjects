using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Users(AdminUserSubmitData subData)
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
                        User fini = new User();
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

            AdminUserViewData viewData = new AdminUserViewData();
            viewData.Users = DBSimulation.Users;
            viewData.SubData = new AdminUserSubmitData();
            viewData.SubData.NewUser = new User();
            viewData.SubData.NewUser.Gold = 50000;
            viewData.SubData.NewUser.DragonPoints = 1337;
            viewData.SubData.NewUser.HyperCoin = 0.119191443m;
            viewData.SubData.NewUser.GoldPieceCoin = 6.38m;
            viewData.SubData.NewUser.HTML5Coin = 80;
            viewData.SubData.NewUser.FLAPCoin = 0;
            return View("Users", viewData);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateNewUser(AdminUserSubmitData subData)
        {
            if (subData.NewUser.UserName != null && !DBSimulation.Users.ContainsKey(subData.NewUser.UserName))
            {
                DBSimulation.Users.Add(subData.NewUser.UserName, subData.NewUser);
            }
            return Users(new AdminUserSubmitData());
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult EditUser(User subData)
        {
            if (subData.UserName == null || !DBSimulation.Users.ContainsKey(subData.UserName))
            {
                return RedirectToAction("Users", new AdminUserSubmitData());
            }
            if (Request.HttpMethod == "GET")
            {
                subData = DBSimulation.Users[subData.UserName];
            }

            return View("EditUser", subData);
        }
    }
}