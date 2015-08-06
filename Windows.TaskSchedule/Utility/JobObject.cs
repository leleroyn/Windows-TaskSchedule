using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule.Utility
{
    public class JobObject
    {
        public string Name { get; set; }
        public string CornExpress { get; set; }
        public IJob Instance { get; set; }
        /// <summary>
        /// 任务正在执行
        /// </summary>
        public bool Running { get; set; }
        /// <summary>
        /// 任务正在执行时间段中
        /// </summary>
        public bool Triggering { get; set; }
    }
}
