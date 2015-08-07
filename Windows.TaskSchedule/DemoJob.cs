using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule
{
    public class DemoJob:IJob
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DemoJob));
        DateTime date = new DateTime();
        public void Init()
        {
            date = DateTime.Now;
        }

        public void Excute()
        {
            logger.Debug(date);           
        }

        public void OnError(Exception ex)
        {
            logger.Debug(ex.ToString());
        }       
    }
}
