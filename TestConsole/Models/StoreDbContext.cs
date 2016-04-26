using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace TestConsole.Models
{
    public class StoreDbContext : DbContext
    {
        public DbSet<UserInfo> UserInfos { get; set; }
    }
}
