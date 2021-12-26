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
    }
}