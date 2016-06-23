using CT.Schedule.Infrastructure.Events;
using CT.Schedule.Service;

namespace CT.Schedule.Core.ScheduleRunner
{
    public class ScheduleJobRemovedEventHandler : IEventHandler<ScheduleJobRemovedEvent>
    {

        private IJobsQueue jobsQueue;

        public ScheduleJobRemovedEventHandler(IJobsQueue jobsQueue)
        {
            this.jobsQueue = jobsQueue;
        }

        public void Handle(ScheduleJobRemovedEvent domainEvent)
        {
            if (domainEvent == null || domainEvent.JobsInfo == null)
            {
                return;
            }
            if (jobsQueue != null)
            {
                this.jobsQueue.Remove(domainEvent.JobsInfo);
            }

        }
    }
}
