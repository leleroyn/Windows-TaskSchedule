using System;
using Windows.TaskSchedule.Extends;

namespace Windows.TaskSchedule.Utility
{   
    public class JobObject
    {
        public string Name { get; set; }
        public string CornExpress { get; set; }
        public IJob Instance { get; set; }        
        /// <summary>
        /// 任务正在执行时间段中
        /// </summary>
        public bool Triggering { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public JobTypeEnum JobType { get; set; }
        /// <summary>
        /// 可执行程序所处的位置
        /// </summary>
        public string ExePath { get; set; }
        /// <summary>
        /// 可执行程序运行时可用的参数
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// 最长运行时间
        /// </summary>
        public int? ExpireSecond
        {
            get;
            set;
        }
        public bool Running
        {
            get;
            set;
        }       
    }

    /// <summary>
    /// 任务类型
    /// </summary>
    public enum JobTypeEnum
    {
        Assembly,
        Exe
    }
}
