using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace SSO.API
{
    public class JSONHelper
    {
        /// <summary>
        /// 返回http响应，内容utf-8，json格式
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetHttpMessage(string content)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(content, Encoding.GetEncoding("UTF-8"), "application/json")
            };
        }
    }
}