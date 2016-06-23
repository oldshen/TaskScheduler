using CT.Schedule.Core.PluginManager;
using CT.Schedule.Domain;
using log4net;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Spi;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CT.Schedule.Core.ScheduleRunner
{
    public class AdaptableJobFactory : IJobFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            IJob job=null;
            IJobDetail jobDetail = bundle.JobDetail;
            Type jobType = jobDetail.JobType;
            try
            {
                job = GlobalUnityContainer.Container.Resolve<IJob>();
            }
            catch (Exception ex){
                logger.Error(ex);
            }
            return job;
        }

        public void ReturnJob(IJob job)
        {
           IDisposable disposable=job as IDisposable;
            if (disposable != null) {
                disposable.Dispose();
            }
        }
    }
    /// <summary>
    /// 任务执行器
    /// </summary>
    internal class Runner: IRunner
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static ISchedulerFactory sf = null;
        static IScheduler sched = null;

        IJobsQueue jobsQueue;
        /// <summary>
        /// 初始化执行器
        /// </summary>
        public Runner(JobsQueue jobsQueue)
        {
            this.jobsQueue = jobsQueue;
            sf = new Quartz.Impl.StdSchedulerFactory();
            sched = sf.GetScheduler();
            sched.JobFactory = new AdaptableJobFactory();
        }

        public void Close()
        {
            sched.Shutdown(true);
            LoadPlugins.Instance.UnLoad();
        }

        /// <summary>
        /// 启动任务
        /// </summary>
        public void Start()
        {
            LoadPlugins.Instance.Init(); //初始化插件
            sched.Start();      //初始化定时任务
            new Task(() => { jobsQueue.Init(AddScheduleJob,RemoveScheduleJob); }).Start();
        }
                        
        /// <summary>
        /// 将任务加入计划队列
        /// </summary>
        /// <param name="job"></param>
        private void AddScheduleJob(ScheduleJobsInfo job)
        {
            IJobDetail jobDetail = CreateJobDetail(job);

            if (sched.CheckExists(jobDetail.Key))
            {
                sched.DeleteJob(jobDetail.Key);
               
            }

            ITrigger trigger = CeateTrigger(job, jobDetail.Key.Group);

            DateTimeOffset ft = sched.ScheduleJob(jobDetail, trigger);
        }


        private void RemoveScheduleJob(ScheduleJobsInfo job)
        {
            IJobDetail jobDetail = CreateJobDetail(job);
            if (sched.CheckExists(jobDetail.Key))
            {
                sched.DeleteJob(jobDetail.Key);
            }
        }
        /// <summary>
        /// 创建任务详情
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private static IJobDetail CreateJobDetail(ScheduleJobsInfo job)
        {
            string name = "timer" + job.Id.ToString();
            string group = "timer" + job.ScheduleId.ToString();
            IJobDetail jobDetail = JobBuilder.Create<ScheduleTask>().WithIdentity(name, group).Build();
            jobDetail.JobDataMap["JOBS"] = job.Id;// new JobTaskArgument() { IsRunning = false, JobInfo=job } ;//.JSONSerialize();
            return jobDetail;
        }

        /// <summary>
        /// 创建任务触发器
        /// </summary>
        /// <param name="job"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        private static ITrigger CeateTrigger(ScheduleJobsInfo job, string group)
        {
            string triggerName = "trigger_" + job.Id.ToString();

            TriggerBuilder triggerBuilder = TriggerBuilder.Create().WithIdentity(triggerName, "trigger_" + group);
            if (job.Mode== JobMode.OnlyOnce|| job.Mode== JobMode.OnceWhenServieStart)
            {
                triggerBuilder = triggerBuilder.WithSimpleSchedule(x => x.WithRepeatCount(0)).StartNow();
            }
            else {
                triggerBuilder = triggerBuilder.WithCronSchedule(job.RunPlan);
            }
            ITrigger trigger = triggerBuilder.Build();
            return trigger;
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
