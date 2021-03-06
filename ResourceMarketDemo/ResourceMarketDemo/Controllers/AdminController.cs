﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceMarketDemo.Models;
using ResourceMarketDemo.DBModels;
using System.Web.Security;

namespace ResourceMarketDemo.Controllers
{
    public class AdminController : Controller
    {
        private RMDDatabaseEntities db = new RMDDatabaseEntities();


        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            return RedirectToActionPermanent("Users");
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Users(int? LoginAs)
        {
            DBModels.User loginUser = null;


            if (LoginAs != null)
            {
                loginUser = db.Users.Where(x => x.Id == LoginAs).FirstOrDefault();
            }

            if (loginUser == null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    loginUser = db.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    if (loginUser == null)
                    {
                        FormsAuthentication.SignOut();
                    }
                }

                if (db.Users.Take(1).Count() == 0)
                {
                    loginUser = new DBModels.User();
                    loginUser.UserName = "Finibhire";
                    loginUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 1, OnHand = 1000 });
                    loginUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 2, OnHand = 2000 });
                    loginUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 3, OnHand = 3000 });
                    loginUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 4, OnHand = 4000 });
                    loginUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 5, OnHand = 5000 });
                    loginUser.UserResources.Add(new UserResource() { ResourceTypeId = 1, OnHand = 10000 });
                    loginUser.UserResources.Add(new UserResource() { ResourceTypeId = 2, OnHand = 20000 });
                    loginUser.UserResources.Add(new UserResource() { ResourceTypeId = 3, OnHand = 30000 });
                    loginUser.UserResources.Add(new UserResource() { ResourceTypeId = 4, OnHand = 40000 });
                    db.Users.Add(loginUser);
                    db.SaveChanges();
                }
                loginUser = db.Users.First();
            }

            FormsAuthentication.SetAuthCookie(loginUser.UserName, true);

            Session["UserId"] = loginUser.Id;
            Session["UserName"] = loginUser.UserName;

            AdminUserViewData viewData = new AdminUserViewData();
            viewData.Users = db.UserAllPivoteds.OrderBy(x => x.UserName).Select(x =>
                new UserExpandedView() {
                    Id = x.Id,
                    UserName = x.UserName,
                    Gold = x.Gold ?? 0,
                    DragonPoints = x.Dragon_Points ?? 0,
                    HyperCoin = x.HyperCoin ?? 0,
                    GoldPieceCoin = x.Gold_Points ?? 0,
                    HTML5Coin = x.HTML5 ?? 0,
                    FLAPCoin = x.FLAP ?? 0,
                    Wood = x.Wood ?? 0,
                    Fish = x.Fish ?? 0,
                    Stone = x.Stone ?? 0,
                    Iron = x.Iron ?? 0
                });
            viewData.SubData = new AdminUserSubmitData();
            viewData.SubData.NewUser = new UserExpandedView();
            viewData.SubData.NewUser.Gold = 100000;
            viewData.SubData.NewUser.DragonPoints = 200000;
            viewData.SubData.NewUser.HyperCoin = 300000;
            viewData.SubData.NewUser.GoldPieceCoin = 400000;
            viewData.SubData.NewUser.HTML5Coin = 500000;
            viewData.SubData.NewUser.FLAPCoin = 600000;
            return View("Users", viewData);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateNewUser(AdminUserSubmitData subData)
        {
            string userName = null;
            int userId = 0;
            this.GetUserData(out userId, out userName);
            
            DBModels.User newUser = new DBModels.User();
            newUser.UserName = subData.NewUser.UserName;
            newUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 1, OnHand = (decimal)subData.NewUser.Gold });
            newUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 2, OnHand = (decimal)subData.NewUser.DragonPoints });
            newUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 3, OnHand = (decimal)subData.NewUser.HyperCoin });
            newUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 4, OnHand = (decimal)subData.NewUser.GoldPieceCoin });
            newUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 5, OnHand = (decimal)subData.NewUser.HTML5Coin });
            newUser.UserCurrencies.Add(new UserCurrency() { CurrencyTypeId = 6, OnHand = (decimal)subData.NewUser.FLAPCoin });
            newUser.UserResources.Add(new UserResource() { ResourceTypeId = 1, OnHand = 10000 });
            newUser.UserResources.Add(new UserResource() { ResourceTypeId = 2, OnHand = 10000 });
            newUser.UserResources.Add(new UserResource() { ResourceTypeId = 3, OnHand = 10000 });
            newUser.UserResources.Add(new UserResource() { ResourceTypeId = 4, OnHand = 10000 });
            
            db.Users.Add(newUser);
            db.SaveChanges();

            return Users(null);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult EditUser(UserExpandedView subData)
        {
            string userName = null;
            int userId = 0;
            this.GetUserData(out userId, out userName);

            if (subData.Id == null)
            {
                throw new HttpException("UserId not supplied!");
            }

            DBModels.User editUser = db.Users
                .Where(x => x.Id == subData.Id)
                .FirstOrDefault();

            if (editUser == null)
            {
                throw new HttpException("UserId does not exist!");
            }

            if (Request.HttpMethod == "POST" && ModelState.IsValid)
            {
                StringBuilder sb = new StringBuilder();
                EditUserUpdateCurrenciesSQL(sb, editUser.Id, 1, subData.Gold);
                EditUserUpdateCurrenciesSQL(sb, editUser.Id, 2, subData.DragonPoints);
                EditUserUpdateCurrenciesSQL(sb, editUser.Id, 3, subData.HyperCoin);
                EditUserUpdateCurrenciesSQL(sb, editUser.Id, 4, subData.GoldPieceCoin);
                EditUserUpdateCurrenciesSQL(sb, editUser.Id, 5, subData.HTML5Coin);
                EditUserUpdateCurrenciesSQL(sb, editUser.Id, 6, subData.FLAPCoin);
                EditUserUpdateResourcesSQL(sb, editUser.Id, 1, subData.Wood);
                EditUserUpdateResourcesSQL(sb, editUser.Id, 2, subData.Fish);
                EditUserUpdateResourcesSQL(sb, editUser.Id, 3, subData.Stone);
                EditUserUpdateResourcesSQL(sb, editUser.Id, 4, subData.Iron);
                db.Database.ExecuteSqlCommand(sb.ToString());
            }

            subData = db.UserAllPivoteds.Where(x => x.Id == subData.Id).Select(x =>
                new UserExpandedView()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Gold = x.Gold ?? 0,
                    DragonPoints = x.Dragon_Points ?? 0,
                    HyperCoin = x.HyperCoin ?? 0,
                    GoldPieceCoin = x.Gold_Points ?? 0,
                    HTML5Coin = x.HTML5 ?? 0,
                    FLAPCoin = x.FLAP ?? 0,
                    Wood = x.Wood ?? 0,
                    Fish = x.Fish ?? 0,
                    Stone = x.Stone ?? 0,
                    Iron = x.Iron ?? 0
                }).FirstOrDefault();

            return View("EditUser", subData);
        }

        private void EditUserUpdateCurrenciesSQL(StringBuilder sb, int UserId, int CurrencyTypeId, decimal? OnHand)
        {
            const string del = "delete from UserCurrencies where UserId={0} and CurrencyTypeId={1} ";
            const string mer = "merge UserCurrencies as t using(select {0} as UserId, cast({1} as tinyint) as CurrencyTypeId, cast({2} as decimal(38,9)) as OnHand) as s on (t.UserId = s.UserId and t.CurrencyTypeId = s.CurrencyTypeId) when matched then update set t.OnHand = s.OnHand when not matched then insert (UserId, CurrencyTypeId, OnHand) values (s.UserId, s.CurrencyTypeId, s.OnHand);";
            
            if (OnHand == null || OnHand == 0)
                sb.AppendFormat(del, UserId, CurrencyTypeId);
            else
                sb.AppendFormat(mer, UserId, CurrencyTypeId, OnHand);
        }
        private void EditUserUpdateResourcesSQL(StringBuilder sb, int UserId, int ResourceTypeId, long? OnHand)
        {
            const string del = "delete from UserResources where UserId={0} and ResourceTypeId={1} ";
            const string mer = "merge UserResources as t using(select {0} as UserId, cast({1} as tinyint) as ResourceTypeId, cast({2} as decimal(38,9)) as OnHand) as s on (t.UserId = s.UserId and t.ResourceTypeId = s.ResourceTypeId) when matched then update set t.OnHand = s.OnHand when not matched then insert (UserId, ResourceTypeId, OnHand) values (s.UserId, s.ResourceTypeId, s.OnHand);";

            if (OnHand == null || OnHand == 0)
                sb.AppendFormat(del, UserId, ResourceTypeId);
            else
                sb.AppendFormat(mer, UserId, ResourceTypeId, OnHand);
        }
    }
}