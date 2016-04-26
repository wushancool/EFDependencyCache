using System;

namespace EFChangeNotify
{
    public interface ICache
    {
        /// <summary>
        /// 有则返回值，没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(String key) where T :class;
        /// <summary>
        /// 将键值对存入缓存中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set(String key, Object value);
    }
}
