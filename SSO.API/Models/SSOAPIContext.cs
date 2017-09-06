using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SSO.API.Models
{
    public class SSOAPIContext : DbContext
    {
        public SSOAPIContext() : base("name=SSOAPI")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
