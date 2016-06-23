using CT.Schedule.Infrastructure.Events;
using CT.Schedule.Service;

namespace CT.Schedule.Core.ScheduleRunner
{
    /// <summary>
    /// 任务保存成功触发事件
    /// </summary>
    public class ScheduleJobSaveSuccessedEventHandler : IEventHandler<ScheduleJobSaveSuccessedEvent>
    {

        private IJobsQueue jobsQueue;

        public ScheduleJobSaveSuccessedEventHandler(IJobsQueue jobsQueue) {
            this.jobsQueue = jobsQueue;
        }

        public void Handle(ScheduleJobSaveSuccessedEvent domainEvent)
        {
            if (domainEvent == null || domainEvent.JobsInfo==null) {
                return;
            }
            if (jobsQueue != null) {
                this.jobsQueue.Remove(domainEvent.JobsInfo);
                this.jobsQueue.Add(domainEvent.JobsInfo);
            }

        }
    }
}
