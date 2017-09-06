using SSO.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SSO.API.Filter
{
    public class CustomAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var sessionid = actionContext.ActionArguments["sessionid"].ToString();
                using (var ctx = new SSOAPIContext())
                {
                    var session = ctx.Sessions.Where(m => m.SessionId == sessionid).First();
                    if (session.ExpireTime.CompareTo(DateTime.Now) < 0)
                    {
                        ctx.Sessions.Remove(session);
                        ctx.SaveChanges();
                        throw new Exception("会话过期，请重新登录。");
                    }
                }
            }
            catch (Exception e)
            {
                actionContext.Response = JSONHelper.GetHttpMessage("身份验证失败，"+e.Message);
            }
        }
    }
}