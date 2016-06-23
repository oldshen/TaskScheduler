using CT.Schedule.Domain;
using log4net;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Text;

namespace CT.Schedule.Repository
{
    public class ScheduleContext : DbContext
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static string connstr;
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        protected static string Connstring
        {
            get
            {
                if (connstr == null)
                {
                    connstr = System.Configuration.ConfigurationManager.ConnectionStrings["CTSchedule.ConnString"].ConnectionString;
                }
                return connstr;
            }
        }

        public ScheduleContext() : base(Connstring)
        {
            Database.SetInitializer<ScheduleContext>(null);
        }
        public DbSet<ScheduleJobsInfo> ScheduleJobs { get; set; }
        public DbSet<SchedulePluginsInfo> Plugins { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduleJobsInfo>().ToTable("t_ScheduleJobs");
            modelBuilder.Entity<ScheduleJobsInfo>().HasKey(c => c.Id);
            modelBuilder.Entity<ScheduleJobsInfo>().Ignore(c => c.RunTimes);
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.ScheduleId).IsRequired().HasColumnName("ScheduleId");
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.RunPlan).HasColumnName("RunPlan");
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.Mode).HasColumnName("Mode");
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.Status).HasColumnName("Status");
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.CreateTime).HasColumnName("CreateTime");
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.Name).HasColumnName("Name");
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.SuccessedTimes).HasColumnName("SuccessedTimes");
            modelBuilder.Entity<ScheduleJobsInfo>().Property(c => c.FailedTimes).HasColumnName("FailedTimes");

            modelBuilder.Entity<SchedulePluginsInfo>().ToTable("t_SchedulePlugins");
            modelBuilder.Entity<SchedulePluginsInfo>().HasKey(c => c.Id);
            modelBuilder.Entity<SchedulePluginsInfo>().Property(c => c.Name).HasColumnName("Name");
            modelBuilder.Entity<SchedulePluginsInfo>().Property(c => c.AssemblyInfo).HasColumnName("AssemblyInfo");
            modelBuilder.Entity<SchedulePluginsInfo>().Property(c => c.NotifyUrl).HasColumnName("NotifyUrl");
            modelBuilder.Entity<SchedulePluginsInfo>().Property(c => c.EventsName).HasColumnName("EventsName");

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                foreach (var error in ex.EntityValidationErrors)
                {
                    foreach (var item in error.ValidationErrors)
                    {
                        sb.AppendLine(item.PropertyName + ": " + item.ErrorMessage);
                    }
                }
                logger.Error("SaveChanges.DbEntityValidation:\t" + ex.Message + sb.ToString());
                throw;
            }
        }
    }
}
