using CT.Schedule.Domain;
using CT.Schedule.Infrastructure.Events;

namespace CT.Schedule.Service
{
    /// <summary>
    /// 任务被删除事件
    /// </summary>
    public class ScheduleJobRemovedEvent : IDomainEvent
    {
        public ScheduleJobsInfo JobsInfo { get; private set; }

        public ScheduleJobRemovedEvent(ScheduleJobsInfo jobsInfo)
        {
            this.JobsInfo = jobsInfo;
        }
    }
}
