using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule
{
    public class DemoJob:IJob
    {
        DateTime date = new DateTime();
        public void Init()
        {
            date = DateTime.Now;
        }

        public void Excute()
        {
            Console.WriteLine(date);           
        }

        public void OnError(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }       
    }
}
