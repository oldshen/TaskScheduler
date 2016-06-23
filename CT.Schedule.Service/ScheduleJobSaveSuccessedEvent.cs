using CT.Schedule.Domain;
using CT.Schedule.Infrastructure.Events;

namespace CT.Schedule.Service
{
    /// <summary>
    /// 任务保存成功
    /// </summary>
    public class ScheduleJobSaveSuccessedEvent : IDomainEvent
    {
        public ScheduleJobsInfo JobsInfo { get; private set; }

        public ScheduleJobSaveSuccessedEvent(ScheduleJobsInfo jobsInfo) {
            this.JobsInfo = jobsInfo;
        }
    }
}
