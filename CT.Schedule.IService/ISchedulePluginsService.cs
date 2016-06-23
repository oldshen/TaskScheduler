using CT.Schedule.Domain;
using System.Collections.Generic;

namespace CT.Schedule.IService
{
    public interface ISchedulePluginsService
    {
        List<SchedulePluginsInfo> FindAll();

        SchedulePluginsInfo FindOne(int id);

        bool Save(SchedulePluginsInfo info);
    }
}
