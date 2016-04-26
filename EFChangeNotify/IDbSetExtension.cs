using EFChangeNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.Entity
{
    public static class IDbSetExtension
    {
        public static List<TEntity> FindWithCache<TEntity, TCache, TDbContext>(this IDbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> whereLambda)
            where TEntity : class
            where TCache : ICache, new()
            where TDbContext : DbContext, new()
        {
            CacheProvider<TCache, TDbContext> cacheProvider = new CacheProvider<TCache, TDbContext>();
            return cacheProvider.FindWithCache<TEntity>(whereLambda);
        }
    }
}
