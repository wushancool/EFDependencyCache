# EF使用SQLSERVER的依赖缓存
Use SqlServer Dependency Cache and EntityFramework

##1、首先打开数据库的Broker
##2、实现ICache接口
  实现键值对的设置和获取的接口，获取如果没有对应键的值返回null，将会自动从数据库中获取并调用设置键值对的方式存到对应的缓存中。
##3、使用查询语句
    context.UserInfos.FindWithCache<UserInfo, Cachable, StoreDbContext>(c => c.UserName == "dave")
  FindWithCache所查询的条件将会被做依赖缓存，所写条件最后生成的sql语句必须满足sqlserver依赖缓存的条件，否则依赖缓存将会不成功。
