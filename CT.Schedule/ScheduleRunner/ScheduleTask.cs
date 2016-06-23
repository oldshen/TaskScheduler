using CT.Schedule.Core.PluginManager;
using CT.Schedule.Domain;
using CT.Schedule.IService;
using log4net;
using Quartz;
using System;
using System.Reflection;

namespace CT.Schedule.Core.ScheduleRunner
{
    [Serializable]
    [DisallowConcurrentExecution]
   // [PersistJobDataAfterExecution]
    public class ScheduleTask : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IScheduleJobsService jobsService;
        private ISchedulePluginsService pluginService;

        public ScheduleTask(IScheduleJobsService jobsService, ISchedulePluginsService pluginService)
        {
            this.jobsService = jobsService;
            this.pluginService = pluginService;
        }

        /// <summary>
        /// 删除Quartz任务
        /// </summary>
        /// <param name="context"></param>
        private void RemoveQuartzJob(IJobExecutionContext context)
        {
            if (context == null)
            {
                return;
            }
            context.Scheduler.DeleteJob(context.JobDetail.Key);
        }
        /// <summary>
        /// 插件任务执行完成相关操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void OnPluginTaskCompleted(IPlugins sender, ExecuteResult result)
        {
            if (sender == null)
            {
                logger.Error(new Exception("ExecuteCompletedHandler 事件未指定正确的sender参数"));  //记录错误日志
                return;
            }
            ScheduleJobsInfo job = jobsService.FindOne(sender.JobID);
            OnPluginTaskCompleted(null, job, result.Successed, result.Message);
        }
        /// <summary>
        /// 插件任务执行完成相关操作
        /// </summary>
        private void OnPluginTaskCompleted(IJobExecutionContext context, ScheduleJobsInfo job, bool successed, string message)
        {
            if (job == null) {
                return;
            }
            jobsService.AddRunTimes(job.Id, successed);
            if (context != null)
            {
                if (job.Mode == JobMode.OnlyOnce || job.Mode == JobMode.OnceWhenServieStart)
                {
                    RemoveQuartzJob(context); //对于只执行一次的任务，完成即移除
                }
            }
            if (!successed)
            {
                logger.Error(message ?? "");  //记录错误日志
                if (job.Mode == JobMode.OnlyOnce || job.Mode == JobMode.OnceWhenServieStart)
                {
                    jobsService.UpdateStatus(job.Id, JobStatus.Failed);//任务完成
                }
                return;
            }

            if (job.Mode == JobMode.OnlyOnce || job.Mode == JobMode.OnceWhenServieStart)
            {
                jobsService.UpdateStatus(job.Id, JobStatus.Successed);
            }
            else {
                jobsService.UpdateStatus(job.Id, JobStatus.WaitToRun);//任务完成，等待下一次执行
            }
        }


        public void Execute(IJobExecutionContext context)
        {
            Guid jobID= (Guid)context.JobDetail.JobDataMap["JOBS"];
            ScheduleJobsInfo job = jobsService.FindOne(jobID);
            if (job == null)
            {
                return;
            }
            logger.Info("Execute:" + job.ScheduleId.ToString());
            SchedulePluginsInfo schedule = pluginService.FindOne(job.ScheduleId);
            string assembly = schedule == null ? "" : schedule.AssemblyInfo;
            if (string.IsNullOrEmpty(assembly))
            {
                return;
            }
            IPlugins plugin = LoadPlugins.Instance.CreateInstance(assembly);

            if (plugin == null)
            {
                logger.Warn("实例化插件:" + assembly + "失败");
                //如果插件不存在，//标记未读取,以便下一次插件加载后可以读取任务
                jobsService.UpdateStatus(jobID, JobStatus.WaitToRun);
                RemoveQuartzJob(context);
                return;
            }
            jobsService.UpdateStatus(jobID, JobStatus.Running);//标记任务正在执行

            try
            {
                if (job.Mode == JobMode.OnTimeEveryTime)
                {
                    plugin.Execute(job.Id, schedule.NotifyUrl, job.Mode, null, OnPluginTaskCompleted);//执行任务
                }
                else {
                    TaskExecuteState state = new TaskExecuteState();
                    plugin.Execute(job.Id, schedule.NotifyUrl, job.Mode, state, null);//执行任务
                    state.Wait();
                    OnPluginTaskCompleted(context, job, plugin.State.Successed, plugin.State.OperationException != null ? plugin.State.OperationException.Message : "");
                }
            }
            catch (Exception ex)
            {
                jobsService.UpdateStatus(jobID, JobStatus.Failed);
                logger.Error(ex);
            }
        }
    }
}
