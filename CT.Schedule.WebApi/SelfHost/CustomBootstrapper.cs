using CT.Schedule.Domain;
using CT.Schedule.IRepository;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;

namespace CT.Schedule.WebApi.SelfHost
{
    /// <summary>
    /// 自定义启动器
    /// </summary>
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        CustomRootPathProvider provider;
        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                if (provider == null)
                {
                    provider = new CustomRootPathProvider();
                }
                return provider;
            }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            //IoC注入注册，对于IRepository一定要在此处注册，否则Nancy获取不到相关实例对象
            container.Register(GlobalUnityContainer.Resolve<IRepository<ScheduleJobsInfo, Guid>>());
            container.Register(GlobalUnityContainer.Resolve<IRepository<SchedulePluginsInfo, int>>());

            Nancy.Json.JsonSettings.RetainCasing = true; //JSON 格式保持C#格式 否则 key的首字母会被转换为小写

            //Nancy.Session.CookieBasedSessions.Enable(pipelines); //允许Cookie和Session
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
         {
             base.RequestStartup(container, pipelines, context);
             var formsAuthConfiguration = new FormsAuthenticationConfiguration
             {
                 RedirectUrl = "~/login",
                 UserMapper = container.Resolve<IUserMapper>(),
             };
             FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
         }
    }
}