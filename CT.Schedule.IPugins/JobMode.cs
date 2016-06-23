using System;

namespace CT.Schedule
{
    /// <summary>
    /// 任务模式
    /// </summary>
    [Flags]
    public enum JobMode:byte
    {

        /// <summary>
        /// 无论服务启动多少次，只执行一次(无论成功与否)
        /// </summary>
        OnlyOnce=0,

        /// <summary>
        /// 每次服务器启动，则执行一次(无论成功与否)
        /// </summary>
        OnceWhenServieStart = 1,

        /// <summary>
        /// 定时任务，等待上一次任务完成再执行
        /// </summary>
        OnTimeWaitPreviousCompleted=2,

        /// <summary>
        /// 定时任务，每次都执行，无论上一次任务是否完成
        /// </summary>
        OnTimeEveryTime=3,
    }
}
