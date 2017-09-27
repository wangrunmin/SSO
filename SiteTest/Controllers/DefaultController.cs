using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using True_SSO.Models;

namespace SiteTest.Controllers
{
    public class DefaultController : Controller
    {
        public ViewResult Index()
        {
            new SSOHelper().Valid(Request, Response);
            return View();
        }
    }
}