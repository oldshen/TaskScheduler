using CT.Schedule.Domain;
using System;

namespace CT.Schedule.Core
{
    /// <summary>
    /// 任务队列
    /// </summary>
    public interface IJobsQueue
    {
        /// <summary>
        /// 初始化队列
        /// </summary>
        /// <param name="onAdded"></param>
        /// <param name="onRemoved"></param>
        void Init(Action<ScheduleJobsInfo> onAdded,Action<ScheduleJobsInfo> onRemoved);

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="job"></param>
        void Add(ScheduleJobsInfo job);

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="job"></param>
        void Remove(ScheduleJobsInfo job);
    }
}
