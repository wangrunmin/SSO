using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SiteTest2.Controllers
{
    public class DefaultController : Controller
    {
        public class resTmp
        {
            public string res { get; set; }
            public string url { get; set; }
            public string token { get; set; }
        }
        public class HttpHelper
        {

            public static string HttpGet(string Url, string postDataStr)
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
        }

        public string SSOSiteUrl = "http://localhost:2925/";
        public string siteTestUrl = "http://localhost:13550/";

        public ViewResult Index()
        {
            //如果访问没有令牌，重定向到SSO服务去检查是否已经登录
            if (!Request.QueryString.AllKeys.Contains("token") || string.IsNullOrEmpty(Request.QueryString["token"]))
            {
                Response.Redirect(SSOSiteUrl + "SSO/CheckCookie?returnUrl=" + Request.Url.AbsoluteUri);
            }
            else//访问带有令牌，检查令牌的合法性
            {
                //之后可以改成post
                var token = HttpHelper.HttpGet(SSOSiteUrl + "SSO/TokenValidate", "token=" + Request.QueryString["token"]);
                var tokenEnt = JsonConvert.DeserializeObject<resTmp>(token);
                if (tokenEnt.res == "OK")
                {
                    return View();
                }
                else
                {
                    Response.Redirect(siteTestUrl + "Account/Login?returnUrl=" + Request.Url.AbsoluteUri);
                }
            }
            return View();
        }
    }
}