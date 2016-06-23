#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/17 8:56:38
* 文件名：JobsQueue
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion


using CT.Schedule.Domain;
using CT.Schedule.IService;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;

namespace CT.Schedule.Core.ScheduleRunner
{

    /// <summary>
    /// 任务队列
    /// </summary>
    internal class JobsQueue : ConcurrentDictionary<Guid, ScheduleJobsInfo>, IJobsQueue 
    {

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IScheduleJobsService jobsService;

        private Timer timer;
        private bool isInited;
        public JobsQueue(IScheduleJobsService jobsService)
        {
            this.jobsService = jobsService;
        }
        Action<ScheduleJobsInfo> onAdded;

        Action<ScheduleJobsInfo> onRemoved;
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(Action<ScheduleJobsInfo> onAdded,Action<ScheduleJobsInfo> onRemoved)
        {
            if (isInited)
            {
                return;
            }
            isInited = true;
            this.onAdded = onAdded;
            this.onRemoved = onRemoved;
            List<ScheduleJobsInfo> lst = jobsService.FindAll();
            lst.ForEach(item =>
            {
                this.Add(item);
            });

            timer = new Timer(60000 * 15);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }


        bool isruning;
        //定时执行任务
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isruning)
            {
                isruning = true;
                try
                {
                    jobsService.FindAll().ForEach(item =>
                    {
                        Add(item);
                    });
                }
                catch (Exception ex)
                {
                    logger.Error(ex);//记录错误
                }
                isruning = false;
            }
        }


        public void Remove(ScheduleJobsInfo job)
        {
            if (job == null || job.Id == Guid.Empty)
            {
                return;
            }
            ScheduleJobsInfo item;
            bool successed = this.TryRemove(job.Id, out item);
            if (successed) {
                if (onRemoved!=null) {
                    onRemoved(item);
                }
            }
        }

        public void Add(ScheduleJobsInfo job)
        {
            if (job == null)
            {
                return;
            }



            if (job.Status == JobStatus.Successed && job.Mode == CT.Schedule.JobMode.OnlyOnce)
            {
                return;
            }
            ScheduleJobsInfo item;
            if (TryGetValue(job.Id, out item))
            {
                if (item.Status == JobStatus.Failed)
                {
                    //TODO:                    
                }
                return;
            }
            if (base.TryAdd(job.Id, job))
            {
                if (onAdded != null)
                {
                    job.Status = JobStatus.WaitToRun;
                    onAdded(job);
                }
            }
        }
    }
}
