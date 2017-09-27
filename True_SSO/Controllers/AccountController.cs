using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using True_SSO.Models;
using True_SSO.Models.EntityHelper;

namespace True_SSO.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];
            var returnUrl = Request.Form["returnUrl"];
            var sso = new SSOHelper();
            //使用接口验证用户身份
            if (!string.IsNullOrEmpty(username))
            {
                var tokenString = HttpHelper.HttpGet(sso.UserValidate, "username=" + username + "&password=" + password);
                var tokenEnt = JsonConvert.DeserializeObject<resTmp>(tokenString);
                if (tokenEnt.res == "OK")
                {
                    Response.Redirect(sso.SetCookie + "?token=" + tokenEnt.token + "&returnUrl=" + returnUrl + "&userid=" + tokenEnt.userid);
                }
                else
                {
                    Response.Redirect(sso.AccountLogin + "?returnUrl=" + returnUrl);
                }
            }
            return View();
        }
    }
}