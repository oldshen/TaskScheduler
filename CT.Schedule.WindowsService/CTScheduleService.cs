using CT.Schedule.Core;
using System.ServiceProcess;
namespace CT.Schedule.WindowsService
{
    public partial class CTScheduleService : ServiceBase
    {
        Start start;
        public CTScheduleService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (start == null)
            {
                start = new Start();
            }
            start.StartSchedule();
            //Runner.Instance.Start();
        }

        protected override void OnStop()
        {

            try
            {
                //Runner.Instance.Close();
                if (start != null)
                {
                    start.Dispose();
                }
            }
            catch { }
        }
        public new void Dispose()
        {
            try
            {
                if (start != null)
                {
                    start.Dispose();
                }
            }
            catch { }
            base.Dispose();
        }
    }
}