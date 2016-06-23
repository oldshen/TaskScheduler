#region Version Info
/* ========================================================================
* 【本类功能概述】
*
* 作者：shenjk 时间：2011/12/19 18:34:23
* 文件名：ExecuteCompletedEventArgs
* 版本：V1.0.1
*
* 修改者： 时间：
* 修改说明：
* ========================================================================
*/
#endregion


using System;

namespace CT.Schedule
{
    /// <summary>
    /// 执行完成参数
    /// </summary> 
    [Serializable]
    public class ExecuteCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccessed { get; set; }

 
    }
}
