using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

namespace Windows.TaskSchedule.Utility
{
    public class ScheduleFactory
    {
        static ILog logger = LogManager.GetLogger("Topshelf.Error");
        static readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Jobs.config");
        static XDocument doc = XDocument.Load(configPath);
        public readonly static string ServerName = doc.Element("Jobs").Attribute("serverName").Value;
        public readonly static string Description = doc.Element("Jobs").Attribute("description").Value;
        public readonly static string DisplayName = doc.Element("Jobs").Attribute("displayName").Value;
        
        public void Start()
        {          
            List<JobObject> jobs = new List<JobObject>();
            try
            {
                jobs = GetJobs();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    foreach (var job in jobs)
                    {
                        RunJob(job);
                    }
                    System.Threading.Thread.Sleep(1);
                }
            });

        }

        public void Stop() { }

        #region Private Method
        /// <summary>
        /// 获取配置文件中所有的任务
        /// </summary>
        /// <returns></returns>
        private List<JobObject> GetJobs()
        {
            List<JobObject> result = new List<JobObject>();
            var jobs = doc.Element("Jobs").Elements("Job");
            foreach (var p in jobs)
            {
                JobObject job = new JobObject();
                if (p.Attributes().Any(o => o.Name.ToString() == "type") && p.Attributes().Any(o => o.Name.ToString() == "exePath"))
                {
                    throw new Exception("job中不能同时配制“type”与“exePath”");
                }
                if (p.Attributes().Any(o => o.Name.ToString() == "type"))
                {
                    job.JobType = JobTypeEnum.Assembly;
                    string assembly = p.Attribute("type").Value.Split(',')[1];
                    string className = p.Attribute("type").Value.Split(',')[0];
                    job.Instance = Assembly.Load(assembly).CreateInstance(className) as IJob;
                }
                else if (p.Attributes().Any(o => o.Name.ToString() == "exePath"))
                {
                    job.JobType = JobTypeEnum.Exe;
                    job.ExePath = p.Attribute("exePath").Value;
                    if (p.Attributes().Any(o => o.Name.ToString() == "arguments"))
                    {
                        job.Arguments = p.Attribute("arguments").Value;
                    }
                }

                job.Name = p.Attribute("name").Value;
                job.CornExpress = p.Attribute("cornExpress").Value;
                result.Add(job);
            }
            return result;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="job">要执行的任务</param>
        private void RunJob(JobObject job)
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
                            try
                            {
                                job.Running = true;
                                switch (job.JobType)
                                {
                                    case JobTypeEnum.Assembly:
                                        job.Instance.Init();
                                        job.Instance.Excute();
                                        break;
                                    case JobTypeEnum.Exe:
                                        var process = new System.Diagnostics.Process();
                                        if (string.IsNullOrWhiteSpace(job.Arguments))
                                        {
                                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(job.ExePath);
                                        }
                                        else
                                        {
                                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(job.ExePath, job.Arguments);
                                        }
                                        process.Start();
                                        process.WaitForExit();
                                        break;
                                }
                            }
                            finally { job.Running = false; }
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
                try
                {
                    job.Instance.OnError(ex);
                }
                catch { }
                logger.Error(string.Format("执行任务:{0}时出错.", job.Name), ex);
            }
        }
        #endregion

    }
}
