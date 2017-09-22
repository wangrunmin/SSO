using Newtonsoft.Json;
using SSO.Filter;
using SSO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSO.Controllers
{
    public class UserController : Controller
    {
        //登录C
        [HttpPost]
        public string Login()
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
                        #region cookie设置
                        HttpCookie cookie = new HttpCookie("session");
                        cookie.Expires = session.ExpireTime;
                        cookie.Values.Add("sid", session.SessionId);
                        cookie.Values.Add("uid", session.UserId);
                        cookie.Values.Add("ctime", session.CreateTime.ToString());
                        cookie.Values.Add("etime", session.ExpireTime.ToString());
                        Response.SetCookie(cookie);
                        #endregion
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
                    res = "ERROR",
                    msg = e.Message
                });
            }
        }
        #region 对于session资源，用户操作应该只能create和delete（即登录和登出），不能update和retrieve
        //查询当前在线用户，以及会话是否过期等R
        [CustomAuth]
        private string GetAllSession()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    return JsonConvert.SerializeObject(new
                    {
                        res = "OK",
                        msg = ctx.Sessions.AsQueryable()
                    });
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                    msg = e.Message
                });
            }
        }
        //更新用户是否在线状态U
        [CustomAuth]
        private string UpdateSession()
        {
            return "未使用方法";
        }
        #endregion
        //登出D[Filter]
        [CustomAuth, HttpPost]
        public string Logout()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    #region 登录过的，删除数据库和客户端cookie。
                    string sessionid = "";
                    try
                    {
                        sessionid = Request.Cookies["session"].Values["sid"];
                    }
                    catch (Exception)
                    {

                    }
                    var session = ctx.Sessions.Where(m => m.SessionId == sessionid).FirstOrDefault();
                    if (session != null)
                    {
                        ctx.Sessions.Remove(session);
                        ctx.SaveChanges();
                    }
                    #region cookie重置
                    HttpCookie cookie = new HttpCookie("session");
                    cookie.Expires = DateTime.Now;
                    cookie.Values.Add("sid", "0");
                    cookie.Values.Add("uid", "0");
                    cookie.Values.Add("ctime", DateTime.Now.ToString());
                    cookie.Values.Add("etime", DateTime.Now.ToString());
                    Response.SetCookie(cookie);
                    #endregion
                    #endregion
                    return JsonConvert.SerializeObject(new
                    {
                        res = "OK",
                        msg = "成功退出当前账号"
                    });
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                    msg = e.Message
                });
            }
        }

        //注册C
        [HttpPost]
        public string Register()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    var ent = new User()
                    {
                        LoginName = Request.Form["LoginName"],
                        Password = Request.Form["Password"],
                        NickName = Request.Form["NickName"],
                        Profile = Request.Form["Profile"],
                        Gender = Request.Form["Gender"],
                        Birthday = DateTime.Parse(Request.Form["Birthday"]).ToString("yyyy-MM-dd"),
                        Location = Request.Form["Location"],
                        Phone = Request.Form["Phone"],
                        Email = Request.Form["Email"],
                    };
                    ctx.Users.Add(ent);
                    ctx.SaveChanges();
                    return JsonConvert.SerializeObject(new
                    {
                        res = "OK",
                        msg = "注册成功"
                    });
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                    msg = e.Message
                });
            }
        }
        //查看R[Filter]
        [HttpPost, CustomAuth]
        public string UserInfo()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    var sessionid = Request.Cookies["session"].Values["sid"];
                    var session = ctx.Sessions.Where(m => m.SessionId == sessionid).FirstOrDefault();
                    if (session != null)
                    {
                        var user = ctx.Users.Where(m => m.UserId == session.UserId).Select(m => new
                        {
                            UserId = m.UserId,
                            LoginName = m.LoginName,
                            NickName = m.NickName,
                            Profile = m.Profile,
                            Gender = m.Gender,
                            Birthday = m.Birthday,
                            Location = m.Location,
                            Phone = m.Phone,
                            Email = m.Email,
                        }).FirstOrDefault();
                        if (user != null)
                        {
                            return JsonConvert.SerializeObject(new
                            {
                                res = "OK",
                                msg = JsonConvert.SerializeObject(user)
                            });
                        }
                    }
                    return JsonConvert.SerializeObject(new
                    {
                        res = "ERROR",
                        msg = "登录信息过期或找不到当前用户"
                    });
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                    msg = e.Message
                });
            }
        }
        //修改U[Filter]
        [HttpPost, CustomAuth]
        public string UpdateUserInfo()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    var sessionid = Request.Cookies["session"].Values["sid"];
                    var session = ctx.Sessions.Where(m => m.SessionId == sessionid).FirstOrDefault();
                    if (session != null)
                    {
                        var user = ctx.Users.Where(m => m.UserId == session.UserId).FirstOrDefault();

                        if (user != null)
                        {
                            //用户id，登录名，密码不在此处修改
                            user.NickName = Request.Form["NickName"];
                            user.Profile = Request.Form["Profile"];
                            user.Gender = Request.Form["Gender"];
                            user.Birthday = DateTime.Parse(Request.Form["Birthday"]).ToString("yyyy-MM-dd");
                            user.Location = Request.Form["Location"];
                            user.Phone = Request.Form["Phone"];
                            user.Email = Request.Form["Email"];
                            ctx.SaveChanges();
                            return JsonConvert.SerializeObject(new
                            {
                                res = "OK",
                                msg = "用户信息修改成功"
                            });
                        }
                    }
                    return JsonConvert.SerializeObject(new
                    {
                        res = "ERROR",
                        msg = "登录信息过期或找不到当前用户"
                    });
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                    msg = e.Message
                });
            }
        }
        //销号D[Filter]
        [HttpPost, CustomAuth]
        public string DelUser()
        {
            try
            {
                using (var ctx = new SSOContext())
                {
                    var sessionid = Request.Cookies["session"].Values["sid"];
                    var session = ctx.Sessions.Where(m => m.SessionId == sessionid).FirstOrDefault();
                    if (session != null)
                    {
                        var user = ctx.Users.Where(m => m.UserId == session.UserId).FirstOrDefault();
                        if (user != null)
                        {
                            ctx.Users.Remove(user);
                            ctx.SaveChanges();
                            return JsonConvert.SerializeObject(new
                            {
                                res = "OK",
                                msg = "当前用户已销号"
                            });
                        }
                    }
                    return JsonConvert.SerializeObject(new
                    {
                        res = "ERROR",
                        msg = "登录信息过期或找不到当前用户"
                    });
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    res = "ERROR",
                    msg = e.Message
                });
            }
        }
    }
}