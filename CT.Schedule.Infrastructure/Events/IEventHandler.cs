#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/11 16:18:39
* 文件名：IEventHandler
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion



namespace CT.Schedule.Infrastructure.Events
{
    /// <summary>
    /// 领域事件处理类
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    public interface IEventHandler<T> where T : IDomainEvent
    {
        /// <summary>
        /// 事件处理函数
        /// </summary>
        /// <param name="domainEvent"></param>
        void Handle(T domainEvent);

    }
}
