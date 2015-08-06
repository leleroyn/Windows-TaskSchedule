using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace Windows.TaskSchedule.Utility
{
    public class CornUtility
    {
        public static bool Trigger(string cornExpress, DateTime dateUtc)
        {
            if(!CronExpression.IsValidExpression(cornExpress)){
                throw new Exception(string.Format("corn表达式：{0}不正确。",cornExpress));
            }
            CronExpression corn = new CronExpression(cornExpress);
            return corn.IsSatisfiedBy(dateUtc);
        }       
    }
}
