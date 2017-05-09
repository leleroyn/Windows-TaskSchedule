using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Quartz;
using Windows.TaskSchedule.Extends;

namespace Windows.TaskSchedule.Utility
{
    public class ScheduleFactory : DefaultLogger
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Jobs.config");
        private static readonly XDocument Doc = XDocument.Load(ConfigPath);
        public static readonly string ServerName = Doc.Element("Jobs").Attribute("serverName").Value;
        public static readonly string Description = Doc.Element("Jobs").Attribute("description").Value;
        public static readonly string DisplayName = Doc.Element("Jobs").Attribute("displayName").Value;

        private static List<JobObject> _jobs = new List<JobObject>();

        public void Start()
        {
            Logger.Debug("服务开始启动.");

            _jobs = GetJobs();
            BatchProcess(_jobs);

            Logger.Debug("共找到【{0}】个任务.", _jobs.Count);
            Logger.Debug("当前服务运行目录:【{0}】.", AppDomain.CurrentDomain.BaseDirectory);
            Logger.Debug("服务启动成功.");
        }

        public void Stop()
        {
            foreach (var job in _jobs)
            {
                if (job.RunInSandbox && job.Sandbox != null)
                {
                    job.Sandbox.Dispose();
                }
            }

            Logger.Debug("服务停止.");
        }

        #region Private Methods

        /// <summary>
        /// 启动轮询任务
        /// </summary>
        /// <param name="jobs"></param>
        private void BatchProcess(List<JobObject> jobs)
        {
            foreach (var job in jobs)
            {
                Task jobTask = new Task(() =>
                {
                    while (true)
                    {
                        if (!job.Running)
                        {
                            job.Running = true;

                            RunJob(job);

                            job.Running = false;
                        }
                        System.Threading.Thread.Sleep(800);
                    }
                });

                jobTask.Start();
            }
        }


        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <returns></returns>
        private List<JobObject> GetJobs()
        {
            List<JobObject> result = new List<JobObject>();
            var jobs = Doc.Element("Jobs").Elements("Job");
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

                    string runInSandbox = "false";
                    if (p.Attributes().Any(o => o.Name.ToString() == "runInSandbox"))
                    {
                        runInSandbox = p.Attribute("runInSandbox").Value;
                    }

                    if (runInSandbox.ToLower() == "true")
                    {
                        Random r = new Random();
                        var name = p.Attribute("name").Value + r.Next(1000);

                        //创建sandbox
                        job.Sandbox = Sandbox.Create(name);
                        job.AssemblyName = assembly;
                        job.TypeName = className;

                        job.RunInSandbox = true;
                    }
                    else
                    {
                        var targetAssembly = Assembly.Load(assembly);
                        job.Instance = targetAssembly.CreateInstance(className) as IJob;

                        job.RunInSandbox = false;
                    }
                }
                if (p.Attributes().Any(o => o.Name.ToString() == "expireSecond"))
                {
                    job.ExpireSecond = int.Parse(p.Attribute("expireSecond").Value);
                }
                else if (p.Attributes().Any(o => o.Name.ToString() == "exePath"))
                {
                    job.JobType = JobTypeEnum.Exe;
                    job.ExePath = p.Attribute("exePath").Value.Replace("${basedir}", AppDomain.CurrentDomain.BaseDirectory);
                    if (p.Attributes().Any(o => o.Name.ToString() == "arguments"))
                    {
                        job.Arguments = p.Attribute("arguments").Value;
                    }
                }

                job.Name = p.Attribute("name").Value;
                job.CornExpress = p.Attribute("cornExpress").Value;
                if (!CronExpression.IsValidExpression(job.CornExpress))
                {
                    throw new Exception(string.Format("corn表达式：{0}不正确。", job.CornExpress));
                }
                result.Add(job);
            }
            return result;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="job">任务信息</param>
        private void RunJob(JobObject job)
        {
            try
            {
                if (CornUtility.Trigger(job.CornExpress, DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))))
                {
                    if (!job.Triggering)
                    {
                        job.Triggering = true;
                        switch (job.JobType)
                        {
                            case JobTypeEnum.Assembly:
                                if (job.RunInSandbox)
                                {
                                    job.Sandbox.Execute(job.AssemblyName, job.TypeName, "Init", null);
                                    job.Sandbox.Execute(job.AssemblyName, job.TypeName, "Excute", null);
                                }
                                else
                                {
                                    job.Instance.Init();
                                    job.Instance.Excute();
                                }

                                break;
                            case JobTypeEnum.Exe:
                                using (var process = new Process())
                                {
                                    bool hasValue = job.ExpireSecond.HasValue;
                                    if (string.IsNullOrWhiteSpace(job.Arguments))
                                    {
                                        process.StartInfo = new ProcessStartInfo(job.ExePath);
                                    }
                                    else
                                    {
                                        process.StartInfo = new ProcessStartInfo(job.ExePath, job.Arguments);
                                    }
                                    process.Start();
                                    if (hasValue) //如果设置了最长运行时间，到达时间时，自动中止进程
                                    {
                                        bool result = process.WaitForExit(job.ExpireSecond.Value * 1000);
                                        if (!result)
                                        {
                                            Logger.Info("任务【{0}】因长时间：{1}秒未返回运行状态，程序已自动将其Kill.", job.Name, job.ExpireSecond);
                                            process.Kill();
                                        }
                                    }
                                    else
                                    {
                                        process.WaitForExit();
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    job.Triggering = false;
                }
            }
            catch (Exception ex) //不处理错误，防止日志爆长
            {
                try
                {
                    if (job.JobType == JobTypeEnum.Assembly)
                    {
                        if (job.RunInSandbox)
                        {
                            job.Sandbox.Execute(job.AssemblyName, job.TypeName, "OnError", ex);
                        }
                        else
                        {
                            job.Instance.OnError(ex);
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        #endregion

    }
}
