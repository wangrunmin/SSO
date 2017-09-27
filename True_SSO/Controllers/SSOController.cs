
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using True_SSO.Models;

namespace True_SSO.Controllers
{
    public class SSOController : Controller
    {
        //检查登录状态
        public void CheckCookie()
        {
            var sso = new SSOHelper();
            //存在cookie说明之前登录过系统
            if (Request.Cookies.AllKeys.Contains("session") &&
                !string.IsNullOrEmpty(Request.Cookies["session"].Values["token"]))
            {
                Response.Redirect(Request.QueryString["returnUrl"] + "?token=" + Request.Cookies["session"].Values["token"]);
            }
            else
            {
                Response.Redirect(sso.AccountLogin + "?returnUrl=" + Request.QueryString["returnUrl"]);
            }
        }
        //用户名，密码验证成功返回token和userid
        public string UserValidate()
        {
            using (var ctx = new SSOContext())
            {
                var sso = new SSOHelper();
                if (Request.QueryString.AllKeys.Contains("username") &&
                    Request.QueryString.AllKeys.Contains("password"))
                {
                    var username = Request.QueryString["username"];
                    var password = Request.QueryString["password"];
                    var userEnt = ctx.Users.Where(m => m.LoginName == username).FirstOrDefault();
                    if (userEnt != null && userEnt.Password == password)
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            res = "OK",
                            userid = userEnt.UserId,
                            token = Guid.NewGuid().ToString()
                        });
                    }
                };
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                });
            }
        }
        //设置cookie
        public void SetCookie()
        {
            using (var ctx = new SSOContext())
            {
                var ent = new Session()
                {
                    Token = Request.QueryString["token"],
                    UserId = Request.QueryString["userid"],
                    CreateTime = DateTime.Now,
                    ExpireTime = DateTime.Now.AddHours(1)
                };
                var oldEnt = ctx.Sessions.Where(m => m.UserId == ent.UserId).FirstOrDefault();
                if (oldEnt != null)
                {
                    oldEnt.Token = ent.Token;
                    oldEnt.CreateTime = ent.CreateTime;
                    oldEnt.ExpireTime = ent.ExpireTime;
                    ctx.SaveChanges();
                }
                else
                {
                    ctx.Sessions.Add(ent);
                    ctx.SaveChanges();
                }
                HttpCookie cookie = new HttpCookie("session");
                cookie.Expires = ent.ExpireTime;
                cookie.Values.Add("token", ent.Token);
                Response.SetCookie(cookie);
                Response.Redirect(Request.QueryString["returnUrl"] + "?token=" + ent.Token);
            }
        }
        public string TokenValidate()
        {
            using (var ctx = new SSOContext())
            {
                var sso = new SSOHelper();
                var token = Request.QueryString["token"];
                var session = ctx.Sessions.Where(m => m.Token == token).FirstOrDefault();
                if (session != null && session.ExpireTime.CompareTo(DateTime.Now) > 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        res = "OK"
                    });
                }
                if (session != null && session.ExpireTime.CompareTo(DateTime.Now) <= 0)
                {
                    ctx.Sessions.Remove(session);
                    ctx.SaveChanges();
                }
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR"
                });
            }
        }
    }
}