using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace True_SSO.Models
{
    public class resTmp
    {
        public string res { get; set; }
        public string url { get; set; }
        public string token { get; set; }
    }
    public class HttpHelper
    {
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
    public class SSOHelper
    {
        public static string SSOSiteUrl = "http://localhost:2925/";
        public static string siteTestUrl = "http://localhost:2925/";

        public static string Valid(HttpRequestBase request, HttpResponseBase response)
        {
            //如果访问没有令牌，重定向到SSO服务去检查是否已经登录
            if (!request.QueryString.AllKeys.Contains("token") || string.IsNullOrEmpty(request.QueryString["token"]))
            {
                response.Redirect(SSOSiteUrl + "SSO/CheckCookie?returnUrl=" + request.Url.AbsoluteUri);
                return "";
            }
            else//访问带有令牌，检查令牌的合法性
            {
                //之后可以改成post
                var token = HttpHelper.HttpGet(SSOSiteUrl + "SSO/TokenValidate", "token=" + request.QueryString["token"]);
                var tokenEnt = JsonConvert.DeserializeObject<resTmp>(token);
                if (tokenEnt.res == "OK")
                {
                    return "OK";
                }
                else
                {
                    response.Redirect(siteTestUrl + "Account/Login?returnUrl=" + request.Url.AbsoluteUri);
                    return "";
                }
            }
        }
    }
}