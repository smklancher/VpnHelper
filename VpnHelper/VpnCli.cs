using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace VpnLink;

public enum VpnConnectionStatus
{
    Unknown,
    Error,
    Disconnected,
    Connected,
    Connecting
}

public static class VpnCli
{
    private static string ExePath => @"C:\Program Files (x86)\Cisco\Cisco AnyConnect Secure Mobility Client\vpncli.exe";

    public static List<string> ConnectCommands(string server, string user, string password)
    {
        return new List<string>() {
            "block 0",
            $"connect {server}",
            user,
            password,
            "y",
            "state",
            "exit"
        };
    }

    public static void Disconnect()
    {
        try
        {
            SendVpnCommandAndWait("disconnect");
        }
        catch (Exception ex)
        {
            Log.WriteLine($"Error sending disconnect: {ex}");
        }
    }

    public static VpnConnectionStatus GetConnectionStatus()
    {
        var output = string.Empty;
        try
        {
            output = SendVpnCommandAndWait("state");
        }
        catch (Exception ex)
        {
            Log.WriteLine($"Error checking VPN status: {ex}");
            return VpnConnectionStatus.Error;
        }

        if (output.Contains("state: Disconnected")) { return VpnConnectionStatus.Disconnected; }
        if (output.Contains("state: Connecting")) { return VpnConnectionStatus.Connecting; }
        if (output.Contains("state: Connected")) { return VpnConnectionStatus.Connected; }

        return VpnConnectionStatus.Unknown;
    }

    public static List<string> GetHosts()
    {
        var output = string.Empty;
        var results = new List<string>();
        try
        {
            output = SendVpnCommandAndWait("hosts");
        }
        catch (Exception ex)
        {
            Log.WriteLine($"Error checking VPN hosts: {ex}");
            return results;
        }

        var regex = new Regex(@">\s(?<host>[\w\-\.]+)");

        var m = regex.Matches(output);

        foreach (Match match in m)
        {
            results.Add(match.Groups["host"].Value);
        }

        return results;
    }

    public static string SendCommandsViaStandardInputAndWait(List<string> cmds)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = ExePath,
            Arguments = "-s",
            WorkingDirectory = Path.GetDirectoryName(ExePath),
            CreateNoWindow = true
        };

        //Log.WriteLine($"Running command {psi.FileName} {psi.Arguments}");

        if (Options.Instance.ShowVpnCliOutput)
        {
            Log.WriteLine($"Standard input lines:\r\n{string.Join("\r\n", cmds)}");
        }

        var pe = new ProcessExecutor(psi, new TimeSpan(0, 0, 120), cmds);
        var result = pe.Execute();

        if (Options.Instance.ShowVpnCliOutput)
        {
            Log.WriteLine(result.Output);
            Log.WriteLine(result.StdErr);
        }

        var logtxt = $"StrOut:\r\n{result.Output}\r\n\r\nStdErr:\r\n{result.StdErr}";
        return logtxt;
    }

    private static string SendVpnCommandAndWait(string cmd)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = ExePath,
            Arguments = cmd,
            WorkingDirectory = Path.GetDirectoryName(ExePath),
            CreateNoWindow = true
        };

        //Log.WriteLine($"Running command {psi.FileName} {psi.Arguments}");

        var pe = new ProcessExecutor(psi, new TimeSpan(0, 0, 20));
        var result = pe.Execute();

        if (Options.Instance.ShowVpnCliOutput)
        {
            Log.WriteLine(result.Output);
            Log.WriteLine(result.StdErr);
        }

        var logtxt = $"StrOut:\r\n{result.Output}\r\n\r\nStdErr:\r\n{result.StdErr}";
        return logtxt;
    }
}