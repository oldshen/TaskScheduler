using CT.Schedule.Domain;
using System;
using System.Collections.Generic;

namespace CT.Schedule.IService
{
    public interface IScheduleJobsService
    {
        List<ScheduleJobsInfo> FindAll();

        bool UpdateStatus(Guid id, JobStatus status);

        ScheduleJobsInfo FindOne(Guid id);

        void AddRunTimes(Guid id,bool successed);


        bool AddJob(ScheduleJobsInfo job);

        bool UpdateJob(ScheduleJobsInfo job);

        bool Remove(Guid id);
    }
}
