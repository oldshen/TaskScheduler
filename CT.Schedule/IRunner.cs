using System;

namespace CT.Schedule.Core
{
    /// <summary>
    /// 任务执行器
    /// </summary>
    public interface IRunner:IDisposable
    {
        /// <summary>
        /// 启动调度器
        /// </summary>
        void Start();
    }
}
