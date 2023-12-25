using System.Diagnostics;

namespace VpnHelper
{
    public class VpnUI
    {
        public static void Kill()
        {
            foreach (var process in Process.GetProcessesByName("csc_ui"))
            {
                Log.WriteLine($"Killing process {process.ProcessName} PID {process.Id}");
                process.Kill();
            }
        }

        public static Process Start()
        {
            var p = Process.Start(@"C:\Program Files (x86)\Cisco\Cisco Secure Client\UI\csc_ui.exe");

            return p;
        }
    }
}