using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Schedule.Domain
{
    /// <summary>
    /// 任务执行状态
    /// </summary>
    [Flags]
    public enum JobStatus:byte
    {
        [Description()]
        WaitToRun = 0,//待执行

        Running = 1,  //正在执行

        Failed = 2,       //执行失败

        Successed = 3     //执行成功
    }
}
