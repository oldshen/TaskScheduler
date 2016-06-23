using CT.Schedule.Domain;
using CT.Schedule.IRepository;
using log4net;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CT.Schedule.Repository
{
    [Serializable]
    public class ScheduleJobsRepository : IRepository<ScheduleJobsInfo, Guid>
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ScheduleJobsRepository() {
            logger.Info("初始化ScheduleJobsRepository");
        }

        public IQueryable<ScheduleJobsInfo> Find(Expression<Func<ScheduleJobsInfo, bool>> query)
        {           
            var context = new ScheduleContext();
            if (query != null)
            {
                return context.Set<ScheduleJobsInfo>().Where(query);
            }       
            return context.ScheduleJobs.AsQueryable<ScheduleJobsInfo>();
           
        }

        public ScheduleJobsInfo FindOne(Guid id)
        {
            using (var context = new ScheduleContext())
            {
                return context.ScheduleJobs.FirstOrDefault(c => c.Id == id);
            }
        }

        public bool Remove(Guid id)
        {
            using (var context = new ScheduleContext())
            {
                var entity = context.ScheduleJobs.FirstOrDefault(c => c.Id == id);
                if (entity != null)
                {
                    context.ScheduleJobs.Attach(entity);
                    context.Entry<ScheduleJobsInfo>(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool Save(ScheduleJobsInfo entity)
        {
            ScheduleJobsInfo info = FindOne(entity.Id);
            if (info == null)
            {
                using (var context = new ScheduleContext())
                {
                    context.ScheduleJobs.Add(entity);
                    context.Entry<ScheduleJobsInfo>(entity).State = EntityState.Added;
                    context.SaveChanges();
                }
                return true;
            }
            using (var context = new ScheduleContext())
            {
                //context.ScheduleJobs.AddObject(entity);
                context.Entry<ScheduleJobsInfo>(entity).State = EntityState.Modified;
                int r= context.SaveChanges();
            }
            return true;
        }
    }
}
