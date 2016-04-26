using System;
using System.Linq;
using EFChangeNotify;
using TestConsole.Models;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            CacheProvider<Cachable, StoreDbContext> cache = new CacheProvider<Cachable, StoreDbContext>();

            StoreDbContext context = new StoreDbContext();
            for (int i = 0; i < 100; i++)
            {
                if (i == 50 || i == 90) Console.ReadKey();
                var user = context.UserInfos.FindWithCache<UserInfo, Cachable, StoreDbContext>(c => c.UserName == "dave").FirstOrDefault();
                Console.WriteLine("{0}name:{1}", i, user.UserName);
            }

            Console.ReadKey();
        }
    }
}
