using CT.Schedule.Core.ScheduleRunner;
using CT.Schedule.Domain;
using CT.Schedule.Infrastructure.Events;
using CT.Schedule.IRepository;
using CT.Schedule.IService;
using CT.Schedule.Repository;
using CT.Schedule.Service;
using Microsoft.Practices.Unity;
using Quartz;
using System;

namespace CT.Schedule.Core
{
    public class Start : IDisposable
    {
        static bool isInitted = false;
        IRunner runner;
     
        public void StartSchedule()
        {
            UnityConfig(GlobalUnityContainer.Container);
            RegisterDomainEvent(GlobalUnityContainer.Container);
            runner = GlobalUnityContainer.Resolve<IRunner>();
            if (runner != null)
            {
                runner.Start();
            }
        }
        /// <summary>
        /// Ioc容器注册
        /// </summary>
        /// <param name="unityContainer"></param>
        public static void UnityConfig(IUnityContainer unityContainer)
        {
            if (isInitted)
            {
                return;
            }
            isInitted = true;
            unityContainer
                .RegisterType<IJobsQueue, JobsQueue>(new ContainerControlledLifetimeManager())
                .RegisterType<IRunner,Runner>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepository<ScheduleJobsInfo, Guid>, ScheduleJobsRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepository<SchedulePluginsInfo, int>, SchedulePluginsRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IScheduleJobsService, ScheduleJobsService>(new ContainerControlledLifetimeManager())
                .RegisterType<ISchedulePluginsService, SchedulePluginsService>(new ContainerControlledLifetimeManager())
                .RegisterType<IJob, ScheduleTask>()
                ;
        }

        /// <summary>
        /// 注册领域事件
        /// </summary>
        /// <param name="unityContainer"></param>
        public static void RegisterDomainEvent(IUnityContainer unityContainer) {
            DomainEvents.RegisterStatic<ScheduleJobSaveSuccessedEvent>(new ScheduleJobSaveSuccessedEventHandler(unityContainer.Resolve<IJobsQueue>()).Handle);
            DomainEvents.RegisterStatic<ScheduleJobRemovedEvent>(new ScheduleJobRemovedEventHandler(unityContainer.Resolve<IJobsQueue>()).Handle);
        }

        public void Dispose()
        {
            if (runner != null) {
                runner.Dispose();
            }
        }
    }
}
