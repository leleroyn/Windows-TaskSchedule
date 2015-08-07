using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule
{
    public interface IJob
    {
        void Init();
        void Excute();
        void OnError(Exception ex);        
    }
}
