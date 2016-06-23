using CT.Schedule.Domain;
using System;

namespace CT.Schedule.WebApi.Model
{
    public class JobModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int PluginID { get; set; }

        public int RunMode { get; set; }


        public string RunPlan { get; set; }

        public int RunplanMode { get; set; }

        public ScheduleJobsInfo ToScheduleJobsInfo()
        {
            ScheduleJobsInfo info = new ScheduleJobsInfo()
            {
                Id=this.Id,
                Name = this.Name,
                CreateTime = DateTime.Now,
                Mode = (JobMode)(this.RunMode == 2 ? (this.RunMode + this.RunplanMode) : this.RunMode),
                RunPlan = this.RunPlan,
                Status = JobStatus.WaitToRun,
                ScheduleId=this.PluginID
            };
            return info;
        }
    }
}