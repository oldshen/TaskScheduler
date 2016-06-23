using CT.Schedule.Domain;
using CT.Schedule.IRepository;
using CT.Schedule.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CT.Schedule.Service
{
    [Serializable]
    public class SchedulePluginsService : ISchedulePluginsService
    {
        private IRepository<SchedulePluginsInfo, int> repository;

        public SchedulePluginsService(IRepository<SchedulePluginsInfo, int> repository) {
            this.repository = repository;
        }
        public List<SchedulePluginsInfo> FindAll()
        {
            return repository.Find(null).ToList();
        }

        public SchedulePluginsInfo FindOne(int id)
        {
            return repository.FindOne(id);
        }

        public bool Save(SchedulePluginsInfo info)
        {
            return repository.Save(info);
        }
    }
}
