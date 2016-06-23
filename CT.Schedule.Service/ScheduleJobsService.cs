using CT.Schedule.Domain;
using CT.Schedule.Infrastructure.Events;
using CT.Schedule.IRepository;
using CT.Schedule.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CT.Schedule.Service
{

    [Serializable]
    public class ScheduleJobsService : IScheduleJobsService
    {
        private IRepository<ScheduleJobsInfo, Guid> repository;

        public ScheduleJobsService(IRepository<ScheduleJobsInfo, Guid> repository)
        {
            this.repository = repository;
            //this.repository = GlobalUnityContainer.Resolve<IRepository<ScheduleJobsInfo, Guid>>();
            
        }

        public bool AddJob(ScheduleJobsInfo job)
        {
            if (job == null) {
                return false;
            }
            if (job.Id == Guid.Empty) {
                job.Id = Guid.NewGuid();
            }
            bool successed= repository.Save(job);
            if (successed) {
                DomainEvents.Raise<ScheduleJobSaveSuccessedEvent>(new ScheduleJobSaveSuccessedEvent(job));
            }
            return successed;
        }

        public void AddRunTimes(Guid id, bool successed)
        {
            var job = repository.FindOne(id);
            if (job == null)
            {
                return;
            }
            if (successed)
            {
                job.SuccessedTimes++;
            }
            else {
                job.FailedTimes++;
            }
            repository.Save(job);
        }


        public List<ScheduleJobsInfo> FindAll()
        {
            return repository.Find(null).ToList();
        }

        public ScheduleJobsInfo FindOne(Guid id)
        {
            return repository.FindOne(id);
        }

        public bool Remove(Guid id)
        {
            bool successed = repository.Remove(id);
            if (successed) {
                DomainEvents.Raise<ScheduleJobRemovedEvent>(new ScheduleJobRemovedEvent(new ScheduleJobsInfo() { Id = id }));
            }
            return successed;
        }

        public bool UpdateJob(ScheduleJobsInfo job)
        {
            ScheduleJobsInfo info = repository.FindOne(job.Id);
            if (info == null) {
                return false;
            }
            info.Mode = job.Mode;
            info.RunPlan = job.RunPlan;

            bool successed= repository.Save(info);
            if (successed)
            {
                DomainEvents.Raise<ScheduleJobSaveSuccessedEvent>(new ScheduleJobSaveSuccessedEvent(job));
            }
            return successed;
        }

        public bool UpdateStatus(Guid id, JobStatus status)
        {
            var job = repository.FindOne(id);
            if (job == null)
            {
                return false;
            }
            
            job.Status = status;
            return repository.Save(job);
        }
    }
}
