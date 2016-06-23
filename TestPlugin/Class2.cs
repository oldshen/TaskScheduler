using CT.Schedule;
using System.Threading;

namespace TestPlugin
{
    public class Class2 : IPlugins
    {
        protected override ExecuteResult OnExecute()
        {
            logger.Info("正在执行:" + JobID.ToString());

            Thread.Sleep(1000 * 70);
            return new ExecuteResult() { Successed = false, Message = "执行失败" };
        }
    }
}
