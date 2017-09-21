using Newtonsoft.Json;
using SSO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSO.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public string Login(string returnUrl)
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    #region 判断是否已经登录,登录过的有效的重定向，无效的继续。
                    {
                        string sessionid = "";
                        try
                        {
                            sessionid = Request.Cookies["session"].Values["sid"];
                        }
                        catch (Exception)
                        {

                        }
                        var session = ctx.Sessions.Where(m => m.SessionId == sessionid).FirstOrDefault();
                        //有效的已登录信息
                        if (session != null && session.ExpireTime.CompareTo(DateTime.Now) > 0)
                        {
                            return JsonConvert.SerializeObject(new
                            {
                                res = "OK",
                                msg = "已验证用户"
                            });
                        }
                        //无效的删除数据库记录
                        if (session != null && session.ExpireTime.CompareTo(DateTime.Now) <= 0)
                        {
                            ctx.Sessions.Remove(session);
                            ctx.SaveChanges();
                        }
                    }
                    #endregion
                    #region 重新登录进行身份验证
                    var username = Request.Form["username"];
                    var password = Request.Form["password"];
                    var user = ctx.Users.Where(m => m.LoginName == username).FirstOrDefault();
                    if (user != null && user.Password == password)
                    {
                        var session = new Session()
                        {
                            SessionId = Guid.NewGuid().ToString(),
                            UserId = user.UserId,
                            CreateTime = DateTime.Now,
                            ExpireTime = DateTime.Now.AddHours(1)
                        };
                        //新增前删除所有相关信息
                        ctx.Sessions.RemoveRange(ctx.Sessions.Where(m => m.UserId == user.UserId));
                        ctx.Sessions.Add(session);
                        ctx.SaveChanges();
                        HttpCookie cookie = new HttpCookie("session");
                        cookie.Expires = session.ExpireTime;
                        cookie.Values.Add("sid", session.SessionId);
                        cookie.Values.Add("uid", session.UserId);
                        cookie.Values.Add("ctime", session.CreateTime.ToString());
                        cookie.Values.Add("etime", session.ExpireTime.ToString());
                        Response.SetCookie(cookie);
                        return JsonConvert.SerializeObject(new
                        {
                            res = "OK",
                            msg = "登录成功"
                        });
                    }
                    else
                    {
                        throw new Exception("请检查用户名和密码");
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "FAILED",
                    msg = e.Message
                });
            }
        }
    }
}