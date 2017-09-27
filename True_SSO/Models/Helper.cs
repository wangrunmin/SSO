using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using True_SSO.Models.EntityHelper;

namespace True_SSO.Models
{
    namespace EntityHelper
    {
        public class configJsonEntity
        {
            public string SSOSiteUrl { get; set; }
        }
        public class resTmp
        {
            public string res { get; set; }
            public string url { get; set; }
            public string token { get; set; }
        }
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
        public SSOHelper()
        {
            string str = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var path = Path.Combine(str, "config.json");
            using (var filestream = new FileStream(path, FileMode.Open))
            {
                byte[] b = new byte[filestream.Length];
                filestream.Read(b, 0, b.Length);
                string json = Encoding.UTF8.GetString(b);
                var ent = JsonConvert.DeserializeObject<configJsonEntity>(json);
                SSOSiteUrl = ent.SSOSiteUrl;
                CheckCookie = SSOSiteUrl + "SSO/CheckCookie";
                SetCookie = SSOSiteUrl + "SSO/SetCookie";
                UserValidate = SSOSiteUrl + "SSO/UserValidate";
                TokenValidate = SSOSiteUrl + "SSO/TokenValidate";
                AccountLogin = SSOSiteUrl + "Account/Login";
            }
        }

        public string SSOSiteUrl;
        public string CheckCookie;
        public string SetCookie;
        public string UserValidate;
        public string TokenValidate;
        public string AccountLogin;

        public void Valid(HttpRequestBase request, HttpResponseBase response)
        {
            //如果访问的查询字符串中没有令牌或者令牌为空，重定向到SSO服务去检查是否已经登录
            if (!request.QueryString.AllKeys.Contains("token") || string.IsNullOrEmpty(request.QueryString["token"]))
            {
                response.Redirect(CheckCookie + "?returnUrl=" + request.Url.AbsoluteUri);
            }
            else//访问带有令牌，检查令牌的合法性
            {
                HttpHelper.HttpGet(TokenValidate, "token=" + request.QueryString["token"] + "&returnUrl=" + request.Url.AbsoluteUri);
                //var tokenEnt = JsonConvert.DeserializeObject<resTmp>(token);
                //if (tokenEnt.res != "OK")
                //{
                //    response.Redirect(AccountLogin + "?returnUrl=" + request.Url.AbsoluteUri);
                //}
            }
        }
    }
}