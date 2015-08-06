using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Windows.TaskSchedule.Utility;

namespace Windows.TaskSchedule
{
    class Program
    {
        static ILog logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            List<JobObject> jobs = new List<JobObject>();
            try
            {
                jobs = ScheduleFactory.GetJobs();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            while (true)
            {
                foreach (var job in jobs)
                {
                    RunJob(job);
                }
                System.Threading.Thread.Sleep(1);
            }
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="job">要执行的任务</param>
        private static void RunJob(JobObject job)
        {
            try
            {
                if (CornUtility.Trigger(job.CornExpress, DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))))
                {                  
                    if (!job.Running && !job.Triggering)
                    {
                        job.Triggering = true;
                        Task.Factory.StartNew(() =>
                        {
                            job.Running = true;
                            job.Instance.Init();
                            job.Instance.Excute();
                            job.Running = false;
                        });
                    }
                }
                else
                {
                    job.Triggering = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("执行任务:{0}时出错.", job.Name), ex);
            }
        }
    }
}
