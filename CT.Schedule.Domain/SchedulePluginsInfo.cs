using System;

namespace CT.Schedule.Domain
{
    /// <summary>
    /// 任务插件管理表实体类
    /// </summary>
    [Serializable]
    public class SchedulePluginsInfo:IAggregate<int>
    {
        #region 构造函数
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public SchedulePluginsInfo()
        {

        }
        #endregion

        #region 公开属性
    
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 触发事件名称
        /// </summary>
        public string EventsName { get; set; }
        /// <summary>
        /// 程序集信息
        /// </summary>
        public string AssemblyInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 结果反馈通知地址
        /// </summary>
        public string NotifyUrl
        {
            get;
            set;
        }
        #endregion
    }
}
