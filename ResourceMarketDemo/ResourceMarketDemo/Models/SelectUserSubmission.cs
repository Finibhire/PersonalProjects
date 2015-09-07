using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class SelectUserSubmission
    {
        public string LoginAs { get; set; }
        public User NewUser { get; set; }
    }
}