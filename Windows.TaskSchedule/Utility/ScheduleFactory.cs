using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Windows.TaskSchedule.Utility
{
    public class ScheduleFactory
    {
        static readonly string configPath = Path.Combine(Environment.CurrentDirectory, "Jobs.xml");
        static XDocument doc = XDocument.Load(configPath);

        /// <summary>
        /// 获取配置文件中所有的任务
        /// </summary>
        /// <returns></returns>
        public static List<JobObject> GetJobs()
        {
            List<JobObject> result = new List<JobObject>();
            var jobs = doc.Element("Jobs").Elements("Job");
            foreach (var p in jobs)
            {
                string assembly = p.Attribute("type").Value.Split(',')[1];
                string className = p.Attribute("type").Value.Split(',')[0];
                JobObject job = new JobObject();
                job.Name = p.Attribute("name").Value;
                job.Instance = Assembly.Load(assembly).CreateInstance(className) as IJob;
                job.CornExpress = p.Attribute("cornExpress").Value;               
                result.Add(job);
            }
            return result;
        }
    }
}
