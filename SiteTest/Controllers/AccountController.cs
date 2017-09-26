using Newtonsoft.Json;
using SiteTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static SiteTest.Controllers.DefaultController;

namespace SiteTest.Controllers
{
    public class AccountController : Controller
    {
        public string SSOSiteUrl = "http://localhost:2925/";
        public string siteTestUrl = "http://localhost:13550/";

        public ActionResult Login()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];
            var returnUrl = Request.Form["returnUrl"];
            //使用接口验证用户身份,等下改成post
            if (!string.IsNullOrEmpty(username))
            {
                var user = HttpHelper.HttpGet(SSOSiteUrl + "SSO/UserValidate", "username=" + username + "&password=" + password);
                var userEnt = JsonConvert.DeserializeObject<resTmp>(user);
                //验证成功
                if (userEnt.res == "OK")
                {
                    Response.Redirect(SSOSiteUrl + "SSO/SetCookie?token=" + userEnt.token + "&returnUrl=" + returnUrl);
                }
                else
                {
                    Response.Redirect(siteTestUrl + "Account/Login?returnUrl=" + Request.QueryString["returnUrl"]);
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            return View();
        }
    }
}