using SSO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSO.Filter
{
    public class CustomAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                string sessionid = "";
                try
                {
                    sessionid = filterContext.HttpContext.Request.Cookies["session"].Values["sid"];
                }
                catch (Exception)
                {
                    throw new Exception("用户尚未登录");
                }
                using (var ctx = new SSOContext())
                {
                    var session = ctx.Sessions.Where(m => m.SessionId == sessionid).FirstOrDefault();
                    if (session == null)
                    {
                        throw new Exception("非法参数");
                    }
                    if (session != null && session.ExpireTime.CompareTo(DateTime.Now) > 0)
                    {
                        session.ExpireTime = DateTime.Now.AddHours(1);
                        HttpCookie cookie = new HttpCookie("session");
                        cookie.Expires = session.ExpireTime;
                        cookie.Values.Add("sid", session.SessionId);
                        cookie.Values.Add("uid", session.UserId);
                        cookie.Values.Add("ctime", session.CreateTime.ToString());
                        cookie.Values.Add("etime", session.ExpireTime.ToString());
                        filterContext.HttpContext.Response.SetCookie(cookie);
                        ctx.SaveChanges();
                    }
                    if (session != null && session.ExpireTime.CompareTo(DateTime.Now) <= 0)
                    {
                        ctx.Sessions.Remove(session);
                        ctx.SaveChanges();
                        throw new Exception("会话过期，请重新登录。");
                    }
                }
            }
            catch (Exception e)
            {
                filterContext.Result = new RedirectResult("/api/Home/ValidFailed?exception=" + e.Message);
            }
        }
    }
}