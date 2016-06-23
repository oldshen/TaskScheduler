using System;

namespace CT.Schedule.Domain
{
    /// <summary>
    /// 任务队列实体类
    /// </summary>
    [Serializable]
    public class ScheduleJobsInfo : IAggregate<Guid>
    {
        #region 构造函数
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ScheduleJobsInfo()
        {
        }
        #endregion



        #region 公开属性

        public string Name { get; set; }

        /// <summary>
        /// 计划Id
        /// </summary>       
        public int ScheduleId
        {
            get;
            set;
        }

        /// <summary>
        /// 0-待执行 1-执行中,2-执行失败，3-执行成功
        /// 程序异常终止以后，需要将在队列中的和执行中状态至为初始状态
        /// </summary>
        public JobStatus Status
        {
            get;
            set;
        }
        /// <summary>
        /// 执行计划
        /// </summary>

        public string RunPlan { get; set; }


        /// <summary>
        /// 任务加入时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 执行模式
        /// </summary>
        public JobMode Mode { get; set; }
        /// <summary>
        /// 执行次数
        /// </summary>
        public int RunTimes
        {
            get
            {
                return SuccessedTimes + FailedTimes;
            }
        }
        /// <summary>
        /// 成功次数
        /// </summary>
        public int SuccessedTimes { get; set; }

        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailedTimes { get; set; }

        #endregion
    }
}
