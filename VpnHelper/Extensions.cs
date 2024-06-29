using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace VpnHelper
{
    public static class Extensions
    {
        public static AutomationElement FindFirstLog(this AutomationElement element, TreeScope scope, Condition condition)
        {
            var prop = condition as PropertyCondition;
            var conditiontxt = "unknown conditon";
            if (prop != null)
            {
                conditiontxt = $"{prop.Property.ProgrammaticName} = {prop.Value}";
            }

            var stopwatch = Stopwatch.StartNew();
            Log.WriteLine($"Automation FindFirst: {conditiontxt}");
            var result = element.FindFirst(scope, condition);
            Log.WriteLine($"Finished Automation FindFirst {stopwatch.Elapsed}: {conditiontxt}");

            return result;
        }
    }
}