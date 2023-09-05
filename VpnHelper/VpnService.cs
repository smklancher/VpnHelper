using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace VpnLink;

public class VpnService
{
    private static string VpnServiceDisplayName => "Cisco AnyConnect Secure Mobility Agent";

    private static string VpnServiceName => "vpnagent";

    public static void RestartService()
    {
        var svc = GetService();
        if (svc.Status.Equals(ServiceControllerStatus.Running) || (svc.Status.Equals(ServiceControllerStatus.StartPending)))
        {
            Log.WriteLine($"Stopping {VpnServiceName}...");
            svc.Stop();
        }
        svc.WaitForStatus(ServiceControllerStatus.Stopped);
        Log.WriteLine($"Starting {VpnServiceName}...");
        svc.Start();
        svc.WaitForStatus(ServiceControllerStatus.Running);
        Log.WriteLine($"Started {VpnServiceName}");
    }

    public static bool ServiceIsRunning()
    {
        var svc = GetService();
        return svc.Status == ServiceControllerStatus.Running;
    }

    private static ServiceController GetService()
    {
        var services = ServiceController.GetServices();

        var svc = services.FirstOrDefault(x => x.ServiceName == VpnServiceName);

        if (svc == null)
        {
            throw new Exception($"Expected service does not exist: {VpnServiceName}");
        }

        return svc;
    }
}