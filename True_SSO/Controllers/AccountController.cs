using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using True_SSO.Models;

namespace True_SSO.Controllers
{
    public class AccountController : Controller
    {
        public string SSOSiteUrl = "http://localhost:2925/";
        public string siteTestUrl = "http://localhost:2925/";

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
    }
}