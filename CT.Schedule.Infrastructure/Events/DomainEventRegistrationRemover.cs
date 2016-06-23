#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/11 16:19:15
* 文件名：DomainEventRegistrationRemover
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion


using System;

namespace CT.Schedule.Infrastructure.Events
{
    /// <summary>
    /// 领域事件处理方法注销类
    /// </summary>
    public class DomainEventRegistrationRemover : IDisposable
    {
        private Action action;

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">注销方法代码段</param>
        public DomainEventRegistrationRemover(Action action)
        {
            this.action = action;
        }
        #endregion

        #region 回收函数
        /// <summary>
        /// 回收函数
        /// </summary>
        public void Dispose()
        {
            this.action();
        }
        #endregion
    }
}
