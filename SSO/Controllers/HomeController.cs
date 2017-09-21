using SSO.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSO.Controllers
{
    public class HomeController : Controller
    {
        [CustomAuth]
        public string Index()
        {
            return "通过验证";
        }
        public string ValidFailed()
        {
            return "未通过验证";
        }
    }
}