using System;
using System.Threading;

namespace CT.Schedule
{
    /// <summary>
    /// 任务执行状态
    /// </summary>
    [Serializable]
    public class TaskExecuteState
    {
        private ManualResetEvent wait;
        public ManualResetEvent OperationCompleted
        {
            get { return wait; }
        }
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool Successed { get; private set;}
        /// <summary>
        /// 操作异常信息
        /// </summary>
        public Exception OperationException { get; private set; }
        public TaskExecuteState() {
            wait = new ManualResetEvent(false);
        }

        /// <summary>
        /// 等待执行结果
        /// </summary>
        public void Wait() {
            wait.WaitOne();
        }

        /// <summary>
        /// 设置操作完成
        /// </summary>
        /// <param name="successed"></param>
        /// <param name="exception"></param>
        public void SetCompleted(bool successed, Exception exception) {
            this.Successed = successed;
            this.OperationException = exception;
            wait.Set();
        }


    }
}
