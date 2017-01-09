using System;
using System.Security.AccessControl;
using Windows.TaskSchedule.Extends;

namespace Windows.TaskSchedule.Utility
{
    public class JobObject
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// corn表达式
        /// </summary>
        public string CornExpress { get; set; }

        /// <summary>
        /// 任务实体
        /// </summary>
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
        public int? ExpireSecond { get; set; }

        /// <summary>
        /// 是否运行
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// 是否允许沙盒
        /// </summary>
        public bool RunInSandbox { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 实体类名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 沙盒
        /// </summary>
        public Sandbox Sandbox { get; set; }
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
