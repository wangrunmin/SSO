using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace True_SSO.Controllers
{
    public class SSOController : Controller
    {
        public string CheckCookie()
        {
            try
            {
                //存在cookie说明之前登录过其它系统
                if (Request.Cookies.AllKeys.Contains("session"))
                {
                    //提取cookie中存储的token
                    var token = Request.Cookies["session"].Values["sid"];
                    Response.Redirect(Request.QueryString["returnUrl"] + "?token=" + token);
                }
                else//没有登录过重定向到登录页面
                {
                    Response.Redirect("http://localhost:13550/Account/Login?returnUrl=" + Request.QueryString["returnUrl"]);
                }
            }
            catch (Exception e)
            {
                Response.Redirect("http://localhost:13550/Account/Login?returnUrl=" + Request.QueryString["returnUrl"] + "&exception=" + e.Message);
            }
            return "";
        }
        public string SetCookie()
        {
            HttpCookie cookie = new HttpCookie("session");
            cookie.Values.Add("sid", Request.QueryString["token"]);
            Response.SetCookie(cookie);
            Response.Redirect(Request.QueryString["returnUrl"] + "?token=" + Request.QueryString["token"]);
            return "";
        }

        public string UserValidate()
        {
            var username = Request.QueryString["username"];
            var password = Request.QueryString["password"];
            if (username == "1" && password == "1")
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "OK",
                    token = Guid.NewGuid().ToString(),
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR"
                });
            }
        }
        public string TokenValidate()
        {
            var token = Request.QueryString["token"];
            //token合法性从数据库检查
            if (token != "")
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "OK",
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                });
            }
        }
    }
}