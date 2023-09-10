using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation;

namespace VpnHelper;

public static class VpnController
{
    /// <summary>
    /// False if already connected, if not in or able to change to console session, or if don't have saved credentials
    /// </summary>
    /// <returns></returns>
    public static bool AbleContinueWithConnectionAttempt()
    {
        if (!CredentialHelper.SavedCredentialsAvailable())
        {
            Log.WriteLine($"Cannot attempt connection saved credential not available ({CredentialHelper.SavedCredentialName()})");
            return false;
        }

        if (!Options.Instance.SkipExistingConnectionCheck)
        {
            if (IsConnected()) { Log.WriteLine("Already connected."); return false; }
        }

        if (!SessionHelper.ChangeToConsoleSessionIfNeeded()) { return false; }

        return true;
    }

    public static void ConnectIfNeededOldCli()
    {
        if (!AbleContinueWithConnectionAttempt()) { return; }

        // kill the GUI process and restart the service
        VpnUI.Kill();
        VpnService.RestartService();

        var cliOutput = VpnCli.SendCommandsViaStandardInputAndWait(VpnCli.VpnCommands());

        // this can fail because the cli can default to trying to reconnect and jumps right to the password prompt.
        // Thus an earlier command is sent as the password.
        // Best solution would be to dynamically adjust the standard input by reading standard output, but for now:
        // just ignore auth fail, then disconnect and try again from a cleanly disconnected state.

        //if (cliOutput.Contains("Authentication failed"))
        //{ Log.WriteLine($"Authentication failed.  Fix saved credentials ({CredentialHelper.SavedCredentialName()})."); return; }

        var connected = IsConnected();
        if (!connected)
        {
            Log.WriteLine($"Connection attempt wasn't successful, trying disconnect command and UI kill before another attempt.");
            VpnCli.Disconnect();
            VpnUI.Kill();
            var cliOutput2 = VpnCli.SendCommandsViaStandardInputAndWait(VpnCli.VpnCommands());
            if (cliOutput2.Contains("Authentication failed"))
            { Log.WriteLine($"Authentication failed.  Fix saved credentials ({CredentialHelper.SavedCredentialName()})."); return; }
            connected = IsConnected();
        }

        if (connected)
        {
            Log.WriteLine($"Now connected to VPN!");
        }
        else
        {
            Log.WriteLine($"Unable to connect to VPN!");
        }
    }

    public static void ConnectWithUIAutomation(Action? onLoginSuccess, Action? onAcceptButtonSuccess, Action? onFailure)
    {
        if (!AbleContinueWithConnectionAttempt())
        {
            onFailure?.Invoke();
            return;
        }

        VpnUI.Kill();
        VpnService.RestartService();

        UIAutomation.AutomationWithTimeout(60, onLoginSuccess, onAcceptButtonSuccess, onFailure);
        var vpnUiProcess = VpnUI.Start();
    }

    public static bool IsConnected()
    {
        if (VpnService.ServiceIsRunning())
        {
            Log.WriteLine("Checking connection status (takes a few seconds)...");
            var constatus = VpnCli.GetConnectionStatus();

            Log.WriteLine($"VPN connection status is: {constatus}");

            return constatus == VpnConnectionStatus.Connected;
        }

        Log.WriteLine($"VPN connection status is: ServiceStopped");
        return false;
    }
}