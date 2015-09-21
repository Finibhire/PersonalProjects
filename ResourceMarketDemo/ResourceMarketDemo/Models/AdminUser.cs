using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceMarketDemo.DBModels;

namespace ResourceMarketDemo.Models
{
    public class AdminUserSubmitData
    {
        //public string CreateNewUser { get; set; }
        public int? LoginAs { get; set; }
        public VFUser NewUser { get; set; }
    }

    public class AdminUserViewData
    {
        public IEnumerable<VFUser> Users { get; set; }
        public AdminUserSubmitData SubData { get; set; }
    }
}