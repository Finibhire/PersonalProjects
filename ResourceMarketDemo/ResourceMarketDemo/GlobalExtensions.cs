using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ResourceMarketDemo.DBModels;

namespace ResourceMarketDemo
{
    public static class GlobalExtensions
    {
        private static RMDDatabaseEntities db = new RMDDatabaseEntities();

        public static void GetUserData(this Controller controller, out int UserId, out string UserName)
        {
            string userName = (string)controller.Session["UserName"];
            int? userId = (int?)controller.Session["UserId"];

            if (userName == null && controller.User.Identity.IsAuthenticated)
            {
                userName = controller.User.Identity.Name;
                controller.Session["UserName"] = userName;
            }
            if (userName == null)
            {
                FormsAuthentication.SignOut();
                //controller.Session["UserName"] = null;
                controller.Session["UserId"] = null;
                throw new Exception("Null User Error");
            }
            UserName = userName;

            if (userId == null)
            {
                var tmp = db.Users
                    .Where(x => x.UserName == userName)
                    .Select(x => new { Id = x.Id })
                    .SingleOrDefault();
                if (tmp == null)
                {
                    FormsAuthentication.SignOut();
                    controller.Session["UserName"] = null;
                    //controller.Session["UserId"] = null;
                    throw new Exception("Logged-In User Does Not Exist in the Database");
                }
                UserId = tmp.Id;
            }
            else
            {
                UserId = (int)userId;
            }
        }

        public static int GetUserId(this Controller controller)
        {
            string userName = null;
            int userId = 0;

            GetUserData(controller, out userId, out userName);

            return userId;
        }

        public static string GetUserName(this Controller controller)
        {
            string userName = null;
            int userId = 0;

            GetUserData(controller, out userId, out userName);

            return userName;
        }


        // In an attempt to make a better DropDownList I made this and learned that it was already done
        // but I had not understood the way it was built.  This was a good learning experience but the
        // method isn't needed any more.
        public static IHtmlString DropDownListForCustom<ModelType, KeyType>(
            this HtmlHelper<ModelType> helper, 
            System.Linq.Expressions.Expression<Func<ModelType, KeyType>> exp,
            IEnumerable<KeyValuePair<KeyType, string>> list)
        {
            EqualityComparer<KeyType> comparer = EqualityComparer<KeyType>.Default;
            Func<ModelType, KeyType> del = exp.Compile();
            KeyType selectedValue = del.Invoke(helper.ViewData.Model);

            string name = del.Method.Name;

            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"");
            sb.Append(name);
            sb.Append("\" name=\"");
            sb.Append(name);
            sb.Append("\">");
            foreach (KeyValuePair<KeyType, string> item in list)
            {
                sb.Append("<option value=\"");
                sb.Append(item.Key);
                if (comparer.Equals(item.Key, selectedValue))
                {
                    sb.Append("\" selected=\"selected\">");
                }
                else
                {
                    sb.Append("\">");
                }
                sb.Append(item.Value);
                sb.Append("</option>");
            }
            sb.Append("</select>");

            return new HtmlString(sb.ToString());
        }
    }
}
