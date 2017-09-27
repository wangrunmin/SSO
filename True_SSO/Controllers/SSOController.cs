
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
        public void CheckCookie()
        {
            try
            {
                var sso = new SSOHelper();
                //存在cookie说明之前登录过其它系统
                if (Request.Cookies.AllKeys.Contains("session"))
                {
                    //提取cookie中存储的token
                    var token = Request.Cookies["session"].Values["token"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        Response.Redirect(Request.QueryString["returnUrl"] + "?token=" + token);
                    }
                    else
                    {
                        Response.Redirect(sso.AccountLogin + "?returnUrl=" + Request.QueryString["returnUrl"]);
                    }
                }
                else//没有登录过重定向到登录页面
                {
                    Response.Redirect(sso.AccountLogin + "?returnUrl=" + Request.QueryString["returnUrl"]);
                }
            }
            catch (Exception)
            {
            }
        }
        public string UserValidate()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    var sso = new SSOHelper();
                    var username = Request.QueryString["username"];
                    var password = Request.QueryString["password"];
                    var userEnt = ctx.Users.Where(m => m.LoginName == username).FirstOrDefault();
                    if (userEnt != null && userEnt.Password == password)
                    {
                        var token = Guid.NewGuid().ToString();
                        HttpCookie cookie = new HttpCookie("session");
                        cookie.Values.Add("token", token);
                        return JsonConvert.SerializeObject(new
                        {
                            res = "OK",
                            token = token
                        });
                    }
                }
            }
            catch (Exception)
            {

            }
            return JsonConvert.SerializeObject(new
            {
                res = "ERROR",
            });
        }
        public void SetCookie()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    var ent = new Session()
                    {
                        Token = Request.QueryString["token"],
                        CreateTime = DateTime.Now,
                        ExpireTime = DateTime.Now.AddHours(1)
                    };
                    if (ctx.Sessions.Where(m => m.Token == ent.Token).FirstOrDefault() == null)
                    {
                        ctx.Sessions.Add(ent);
                        ctx.SaveChanges();
                    }
                    HttpCookie cookie = new HttpCookie("session");
                    cookie.Values.Add("token", ent.Token);
                    Response.SetCookie(cookie);
                    Response.Redirect(Request.QueryString["returnUrl"] + "?token=" + ent.Token);
                }
            }
            catch (Exception)
            {
            }
        }
        public void TokenValidate()
        {
            try
            {
                var sso = new SSOHelper();
                using (var ctx = new SSOContext())
                {
                    var token = Request.QueryString["token"];
                    if (ctx.Sessions.Where(m => m.Token == token).FirstOrDefault() != null)
                    {

                    }
                    else
                    {
                        Response.Redirect(sso.AccountLogin + "?returnUrl=" + Request.QueryString["returnUrl"]);

                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}