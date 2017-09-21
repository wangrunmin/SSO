using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SSO.API.Models;
using System.Text;
using System.Web.Script.Serialization;
using SSO.API.Filter;
using Newtonsoft.Json;

namespace SSO.API.Controllers
{
    [CustomAuth]
    public class UsersController : ApiController
    {
        public HttpResponseMessage PostUser(string sessionid, string data)
        {
            try
            {
                var ent = JsonConvert.DeserializeObject<User>(data);
                using (var ctx = new SSOAPIContext())
                {
                    ent.UserId = Guid.NewGuid().ToString();//自动生成主键
                    ctx.Users.Add(ent);
                    ctx.SaveChanges();
                    return JSONHelper.GetHttpMessage("保存新用户成功。");
                }
            }
            catch (Exception e)
            {
                return JSONHelper.GetHttpMessage(e.Message);
            }
        }
        public HttpResponseMessage GetUsers(string sessionid)
        {
            try
            {
                using (var ctx = new SSOAPIContext())
                {
                    string content = new JavaScriptSerializer().Serialize(ctx.Users);
                    return JSONHelper.GetHttpMessage(content);
                }
            }
            catch (Exception e)
            {
                return JSONHelper.GetHttpMessage(e.Message);
            }
        }
        public HttpResponseMessage GetUser(string sessionid, string id)
        {
            try
            {
                using (var ctx = new SSOAPIContext())
                {
                    string content = new JavaScriptSerializer()
                        .Serialize(ctx.Users.Where(m => m.UserId == id).First());
                    return JSONHelper.GetHttpMessage(content);
                }
            }
            catch (Exception e)
            {
                return JSONHelper.GetHttpMessage(e.Message);

            }
        }
        public HttpResponseMessage PutUser(string sessionid, string id, string data)
        {
            try
            {
                var ent = JsonConvert.DeserializeObject<User>(data);
                if (id != ent.UserId)
                {
                    throw new Exception("更新失败，id不匹配");
                }
                using (var ctx = new SSOAPIContext())
                {
                    ctx.Entry(ent).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
                return JSONHelper.GetHttpMessage("更新成功");
            }
            catch (Exception e)
            {
                return JSONHelper.GetHttpMessage(e.Message);
            }
        }

        public HttpResponseMessage DeleteUser(string sessionid, string id)
        {
            try
            {
                using (var ctx = new SSOAPIContext())
                {
                    var ent = ctx.Users.Where(m => m.UserId == id).First();
                    ctx.Users.Remove(ent);
                    ctx.SaveChanges();
                }
                return JSONHelper.GetHttpMessage("删除成功");
            }
            catch (Exception e)
            {
                return JSONHelper.GetHttpMessage(e.Message);
            }
        }

    }
}