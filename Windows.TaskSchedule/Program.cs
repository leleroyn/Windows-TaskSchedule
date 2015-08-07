using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Topshelf;
using Windows.TaskSchedule.Utility;

namespace Windows.TaskSchedule
{
    class Program
    {
        static ILog logger = LogManager.GetLogger("SystemLogger");

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ScheduleFactory>(sc =>
                {
                    sc.SetServiceName(ScheduleFactory.ServerName);
                    sc.ConstructUsing(() => new ScheduleFactory());                  
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                });
                
                x.SetEventTimeout(new TimeSpan(0, 30, 0));
                x.SetServiceName(ScheduleFactory.ServerName);
                x.SetDisplayName(ScheduleFactory.DisplayName);
                x.SetDescription(ScheduleFactory.Description);
                x.RunAsLocalSystem();
                x.StartAutomatically();              
            });

        }
    }
}
