using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace EFChangeNotify
{
    public class CacheProvider<TCache, TDbContext>
        where TCache : ICache, new()
        where TDbContext : DbContext, new()
    {
        public List<TEntity> FindWithCache<TEntity>(Expression<Func<TEntity, bool>> whereLambda)
        where TEntity : class
        {
            ICache cachable = new TCache();

            EntityChangeNotifier<TEntity, TDbContext> notifer = null;
            SafeCountDictionary.IncrementSqlString(GetSql(whereLambda), key =>
            {
                notifer = new EntityChangeNotifier<TEntity, TDbContext>(GetSql(whereLambda));
                notifer.Changed += (sender, e) =>
                {
                    cachable.Set(GetSql(whereLambda), null);
                };
                notifer.Error += (sender, e) =>
                {

                };
            });

            List<TEntity> list = cachable.Get<List<TEntity>>(GetSql<TEntity>(whereLambda));
            if (list == null)
            {
                list = new TDbContext().Set<TEntity>().Where(whereLambda).ToList();
                cachable.Set(GetSql<TEntity>(whereLambda), list);
            }
            return list;

        }

        private string GetSql<TEntity>(Expression<Func<TEntity, bool>> whereLambda)
            where TEntity : class
        {
            var q = GetCurrent(whereLambda);

            return q.ToTraceString();
        }

        private DbQuery<TEntity> GetCurrent<TEntity>(Expression<Func<TEntity, bool>> whereLambda)
            where TEntity : class
        {
            var query = new TDbContext().Set<TEntity>().Where(whereLambda) as DbQuery<TEntity>;

            return query;
        }

    }
}
