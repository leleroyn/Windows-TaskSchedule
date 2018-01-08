using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule.Extends
{
    public class DemoJob : DefaultLogger, IJob
    {
        public void Excute()
        {
            DateTime date = new DateTime();
            date = DateTime.Now;
            Logger.Debug(date);
            Logger.Debug(System.Configuration.ConfigurationManager.AppSettings["test"]);
        }
    }
}
