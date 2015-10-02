using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceMarketDemo.DBModels;
using System.ComponentModel.DataAnnotations;

namespace ResourceMarketDemo.Models
{
    public class AdminUserSubmitData
    {
        //public string CreateNewUser { get; set; }
        public int? LoginAs { get; set; }
        public UserExpandedView NewUser { get; set; }
    }

    public class AdminUserViewData
    {
        public IEnumerable<UserExpandedView> Users { get; set; }
        public AdminUserSubmitData SubData { get; set; }
    }

    public class UserExpandedView
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string UserName { get; set; }
        public decimal Gold { get; set; }
        public decimal DragonPoints { get; set; }
        public decimal HyperCoin { get; set; }
        public decimal GoldPieceCoin { get; set; }
        public decimal HTML5Coin { get; set; }
        public decimal FLAPCoin { get; set; }
        public long Wood { get; set; }
        public long Fish { get; set; }
        public long Stone { get; set; }
        public long Iron { get; set; }
    }
}