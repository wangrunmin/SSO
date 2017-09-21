using Newtonsoft.Json;
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
            return JsonConvert.SerializeObject(new
            {
                res = "OK",
                msg = "恭喜，通过验证"
            });
        }
        public string ValidFailed(string exception)
        {
            return JsonConvert.SerializeObject(new
            {
                res = "FAILED",
                msg = exception
            });
        }
    }
}