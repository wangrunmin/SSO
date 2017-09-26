using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SiteTest.Controllers
{
    public class DefaultController : Controller
    {
        public class resTmp
        {
            public string res { get; set; }
            public string url { get; set; }
            public string token { get; set; }
        }
        // GET: Default
        public ViewResult Index()
        {
            //如果访问没有令牌，重定向到SSO服务去检查是否已经登录
            if (!Request.QueryString.AllKeys.Contains("token") && !Request.Headers.AllKeys.Contains("token"))
            {
                Response.Redirect("http://localhost:11274/api/SSO/CheckCookie?returnUrl=" + Request.Url);
            }
            else//访问带有令牌，检查令牌的合法性
            {
                //之后可以改成post
                var token = HttpGet("http://localhost:11274/api/SSO/TokenValidate", "token=" + Request.QueryString["token"]);
                var tokenEnt = JsonConvert.DeserializeObject<resTmp>(token);
                if (tokenEnt.res == "OK")
                {
                    return View();
                }
                else
                {
                    Response.Redirect("http://localhost:11274/api/SSO/CheckCookie?returnUrl=" + Request.Url);
                }
            }
            return View();
        }
        //private string HttpPost(string Url, string postDataStr)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        //    request.Method = "POST";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
        //    request.CookieContainer = cookie;
        //    Stream myRequestStream = request.GetRequestStream();
        //    StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
        //    myStreamWriter.Write(postDataStr);
        //    myStreamWriter.Close();

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    response.Cookies = cookie.GetCookies(response.ResponseUri);
        //    Stream myResponseStream = response.GetResponseStream();
        //    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        //    string retString = myStreamReader.ReadToEnd();
        //    myStreamReader.Close();
        //    myResponseStream.Close();

        //    return retString;
        //}
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
    }
}