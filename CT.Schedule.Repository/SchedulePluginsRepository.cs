using CT.Schedule.Domain;
using CT.Schedule.IRepository;
using log4net;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CT.Schedule.Repository
{
    [Serializable]
    public class SchedulePluginsRepository : IRepository<SchedulePluginsInfo, int>
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IQueryable<SchedulePluginsInfo> Find(Expression<Func<SchedulePluginsInfo, bool>> query)
        {
            var context = new ScheduleContext();

            if (query != null)
            {
                return context.Plugins.Where(query).OrderByDescending(c=>c.Id);
            }
            return context.Plugins.OrderByDescending(c => c.Id).AsQueryable();

        }

        public SchedulePluginsInfo FindOne(int id)
        {
            using (var context = new ScheduleContext())
            {
                return context.Plugins.FirstOrDefault(c => c.Id == id);
            }
        }

        public bool Remove(int id)
        {
            using (var context = new ScheduleContext())
            {
                var entity = context.Plugins.FirstOrDefault(c => c.Id == id);
                if (entity != null)
                {
                    context.Plugins.Attach(entity);
                    context.Entry<SchedulePluginsInfo>(entity).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool Save(SchedulePluginsInfo entity)
        {

            using (var context = new ScheduleContext())
            {
                if (entity.Id == 0)
                {
                    context.Plugins.Add(entity);
                    context.Entry<SchedulePluginsInfo>(entity).State = System.Data.Entity.EntityState.Added;
                }
                else {

                    var info = context.Plugins.FirstOrDefault(c => c.Id == entity.Id);
                    if (entity != null)
                    {

                        context.Entry<SchedulePluginsInfo>(entity).State = System.Data.Entity.EntityState.Modified;

                    }
                }
                context.SaveChanges();
            }
            return true;
        }
    }
}
