using Newtonsoft.Json;
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
        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        public ActionResult Login()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];
            //使用接口验证用户身份,等下改成post
            var user = HttpGet("http://localhost:11274/api/SSO/UserValidate", "username=" + username + "&password=" + password);
            var userEnt = JsonConvert.DeserializeObject<resTmp>(user);
            //验证成功
            if (userEnt.res == "OK")
            {
                Response.Redirect("http://localhost:11274/api/SSO/SetCookie?token=" + userEnt.token + "&returnUrl=" + Request.QueryString["returnUrl"]);
            }
            return View();
        }
        public ActionResult Logout()
        {
            return View();
        }
    }
}