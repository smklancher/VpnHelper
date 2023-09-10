using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace VpnHelper
{
    public class UIAutomationTesting
    {
        public static void DisableAutomationEventHandlers()
        {
            Log.WriteLine("Disabled UI Automation Monitoring");
            Automation.RemoveAllEventHandlers();
        }

        public static void EnableAutomationEventHandlers()
        {
            Log.WriteLine("Enabled UI Automation Monitoring");
            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, WindowOpenEvent);
            Automation.AddStructureChangedEventHandler(AutomationElement.RootElement, TreeScope.Descendants, StructureChangedEvent);
            // enable accept button automation
        }

        private static void StructureChangedEvent(object sender, StructureChangedEventArgs e)
        {
            var window = sender as AutomationElement;
            if (window == null)
            {
                return;
            }
            Log.WriteLine($"Structure change ({e.StructureChangeType}): {window.Current.Name}");
        }

        private static void WindowOpenEvent(object sender, AutomationEventArgs e)
        {
            var window = sender as AutomationElement;
            if (window == null)
            {
                return;
            }
            Log.WriteLine($"Window Open: {window.Current.Name}");
        }
    }
}