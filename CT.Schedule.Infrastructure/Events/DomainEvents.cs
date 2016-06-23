#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/11 16:19:55
* 文件名：DomainEvents
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion


using System;
using System.Collections.Generic;

namespace CT.Schedule.Infrastructure.Events
{
    /// <summary>
    /// 领域事件处理总线类
    /// </summary>
    public static class DomainEvents
    {
        #region 属性
        /// <summary>
        /// 事件处理函数列表
        /// </summary>
        private static List<Delegate> staticActions;
        /// <summary>
        /// 事件处理函数列表
        /// </summary>
        [ThreadStatic]
        private static List<Delegate> actions;
        /// <summary>
        /// 事件处理函数列表
        /// </summary>
        private static List<Delegate> Actions
        {
            get
            {
                if (actions == null)
                    actions = new List<Delegate>();
                return actions;
            }
        }
        /// <summary>
        /// 事件处理函数列表
        /// </summary>
        private static List<Delegate> StaticActions
        {
            get
            {
                if (staticActions == null)
                    staticActions = new List<Delegate>();
                return staticActions;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 注册事件响应函数 自动回收
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件响应函数</param>
        public static IDisposable Register<T>(Action<T> handler)
            where T : IDomainEvent
        {
            if (!Actions.Contains(handler))
                Actions.Add(handler);
            return new DomainEventRegistrationRemover(() => Actions.Remove(handler));
        }
        /// <summary>
        /// 注册事件响应函数(不被自动回收)
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件响应函数</param>
        public static void RegisterStatic<T>(Action<T> handler)
            where T : IDomainEvent
        {
            if (!StaticActions.Contains(handler))
            {
                StaticActions.Add(handler);
            }
            //return new DomainEventRegistrationRemover(() => Actions.Remove(handler));
        }
        /// <summary>
        /// 激活事件处理函数
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="domainEvent">事件响应函数</param>
        public static void Raise<T>(T domainEvent)
            where T : IDomainEvent
        {
            //激活静态全局事件
            IEnumerable<IEventHandler<T>> registerHandlers = GetRegisterHandler<T>();
            foreach (IEventHandler<T> handler in registerHandlers)
            {
                handler.Handle(domainEvent);
            }
            //激活动态事件
            foreach (Delegate action in Actions)
            {
                if (action is Action<T>)
                {
                    ((Action<T>)action)(domainEvent);
                }
            }
            //激活静态事件
            foreach (Delegate action in StaticActions)
            {
                if (action is Action<T>)
                {
                    ((Action<T>)action)(domainEvent);
                }
            }
        }

        /// <summary>
        /// 获取领域事件注册类实例
        /// </summary>
        /// <typeparam name="T">领域事件类型</typeparam>
        /// <returns>事件处理类实例列表</returns>
        private static IEnumerable<IEventHandler<T>> GetRegisterHandler<T>() where T : IDomainEvent
        {
            IEnumerable<IEventHandler<T>> handlerInstants = new List<IEventHandler<T>>();
            return handlerInstants;
        }
        #endregion
    }
}
