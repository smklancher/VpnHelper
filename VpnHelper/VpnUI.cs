using System.Diagnostics;

namespace VpnLink
{
    public class VpnUI
    {
        public static void Kill()
        {
            foreach (var process in Process.GetProcessesByName("vpnui"))
            {
                Log.WriteLine($"Killing process {process.ProcessName} PID {process.Id}");
                process.Kill();
            }
        }

        public static Process Start()
        {
            var p = Process.Start(@"C:\Program Files (x86)\Cisco\Cisco AnyConnect Secure Mobility Client\vpnui.exe");

            return p;
        }
    }
}