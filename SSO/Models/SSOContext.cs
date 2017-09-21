using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SSO.Models
{
    public class SSOContext : DbContext
    {
        public SSOContext() : base("SSO")
        {
        }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}