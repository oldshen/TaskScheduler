using CT.Schedule;
using System.Threading;

namespace TestPlugin
{
    public class Class1 : IPlugins
    {
        
        protected  override ExecuteResult OnExecute()
        {

          
            logger.Info("正在执行:"+JobID.ToString() + System.Configuration.ConfigurationManager.AppSettings["log4net.Config"]);

            Thread.Sleep(1000 * 25);
            return new ExecuteResult() {  Successed=true, Message="执行失败"};
        }

    }
}
