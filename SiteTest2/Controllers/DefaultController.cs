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

namespace SiteTest2.Controllers
{
    public class DefaultController : Controller
    {
        //public string SSOSiteUrl = "http://localhost:2925/";
        //public string siteTestUrl = "http://localhost:2925/";

        //public ViewResult Index()
        //{
        //    var request = Request;
        //    var response = Response;
        //    //如果访问没有令牌，重定向到SSO服务去检查是否已经登录
        //    if (!Request.QueryString.AllKeys.Contains("token") || string.IsNullOrEmpty(Request.QueryString["token"]))
        //    {
        //        Response.Redirect(SSOSiteUrl + "SSO/CheckCookie?returnUrl=" + Request.Url.AbsoluteUri);
        //    }
        //    else//访问带有令牌，检查令牌的合法性
        //    {
        //        //之后可以改成post
        //        var token = HttpHelper.HttpGet(SSOSiteUrl + "SSO/TokenValidate", "token=" + Request.QueryString["token"]);
        //        var tokenEnt = JsonConvert.DeserializeObject<resTmp>(token);
        //        if (tokenEnt.res == "OK")
        //        {
        //            return View();
        //        }
        //        else
        //        {
        //            Response.Redirect(siteTestUrl + "Account/Login?returnUrl=" + Request.Url.AbsoluteUri);
        //        }
        //    }
        //    return View();
        //}
        public ViewResult Index()
        {
            SSOHelper.Valid(Request, Response);
            return View();
        }
    }
}