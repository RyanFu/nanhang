﻿using Autofac;
using Autofac.Integration.Mvc;
using System.Data.Entity;
using System.Reflection;
using System.Web.Mvc;
using ZHXY.Domain;

namespace ZHXY.Application
{
    public class DIHelper
    {
        public static void SetMvcDependencyResolver()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ZhxyDbContext>().As<DbContext>().InstancePerRequest();

            // 注册单元工作
            builder.RegisterType(typeof(UnitWork)).As(typeof(IUnitWork)).InstancePerRequest();

            // 注册app层
            builder.RegisterAssemblyTypes(typeof(AppService).Assembly).Where(p => p.BaseType.Equals(typeof(AppService)) && !p.IsAbstract).AsSelf().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(AppService).Assembly).Where(p => p.BaseType.Equals(typeof(AppService)) && !p.IsAbstract).AsImplementedInterfaces().InstancePerRequest();

            // 注册控制器
            builder.RegisterControllers(Assembly.GetCallingAssembly());
            builder.RegisterModelBinders(Assembly.GetCallingAssembly());
            builder.RegisterModelBinderProvider();

            // 启用视图模型自动注入
            builder.RegisterSource(new ViewRegistrationSource());

            // 注册所有的Attribute
            builder.RegisterFilterProvider();

            // 设置容器
            var container = builder.Build();

            // 设置 Mvc 依赖解析 
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}