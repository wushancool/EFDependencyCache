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
        /// <summary>
        /// 从缓存中获取数据，如果缓存中没有则从数据库中获取，然后存到缓存中，缓存的键使用sql语句，缓存改变通知依赖SQLSERVER依赖缓存，所以条件不能太复杂，要满足SQLSERVER依赖缓存要求
        /// </summary>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <param name="whereLambda">条件</param>
        /// <param name="insertCacheAction">当缓存内容第一次被插入到缓存时候执行动作</param>
        /// <param name="changeEvent">当缓存内容发生变化的时候，会先清空缓存，然后执行该动作</param>
        /// <returns></returns>
        public List<TEntity> FindWithCache<TEntity>(Expression<Func<TEntity, bool>> whereLambda,
            Action<TEntity> insertCacheAction = null,
            EventHandler<EntityChangeEventArgs<TEntity>> changeEvent = null)
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
                if (changeEvent != null)
                    notifer.Changed += changeEvent;
                notifer.Error += (sender, e) =>
                {

                };
            });

            List<TEntity> list = cachable.Get<List<TEntity>>(GetSql<TEntity>(whereLambda));
            if (list == null)
            {
                list = new TDbContext().Set<TEntity>().Where(whereLambda).ToList();
                if (insertCacheAction != null)
                {
                    list.ForEach(t => insertCacheAction(t));
                }
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
