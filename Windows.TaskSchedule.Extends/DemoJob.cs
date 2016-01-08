using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule.Extends
{
    public class DemoJob:DefaultLogger, IJob
    {      
        DateTime date = new DateTime();
        public void Init()
        {
            date = DateTime.Now;
        }

        public void Excute()
        {
           Logger.Debug(date);          
        }

        public void OnError(Exception ex)
        {
            Logger.Debug(ex.ToString());
        }       
    }
}
