using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class AdminUserSubmitData
    {
        //public string CreateNewUser { get; set; }
        public string LoginAs { get; set; }
        public User NewUser { get; set; }
    }

    public class AdminUserViewData
    {
        public SortedDictionary<string, User> Users { get; set; }
        public AdminUserSubmitData SubData { get; set; }
    }
}