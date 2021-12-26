using System.Diagnostics;

namespace VpnLink;

public static class VpnController
{
    public static bool ChangeToConsoleSessionIfNeeded()
    {
        if (Options.Instance.IsConsoleSessionRequired && !SessionHelper.IsConsoleActiveSession())
        {
            Log.WriteLine("Sending current Windows session to console to allow VPN connections...");
            SessionHelper.SendCurrentSessionToConsole();

            // not sure if waiting is needed here

            if (!SessionHelper.IsConsoleActiveSession())
            {
                Log.WriteLine("Could not get to console session, thus cannot connect.");
                return false;
            }
        }
        return true;
    }

    public static void ConnectIfNeeded()
    {
        // skip if already connected, if not in or able to change to console session, or if don't have saved credentials
        if (!CredentialHelper.SavedCredentialsAvailable())
        { Log.WriteLine($"Cannot attempt connection saved credential not available ({CredentialHelper.SavedCredentialName()})"); return; }
        if (IsConnected()) { Log.WriteLine("Already connected."); return; }
        if (!ChangeToConsoleSessionIfNeeded())
        { Log.WriteLine($"Not able to switch to console session to make VPN connection."); return; }

        // kill the GUI process and restart the service
        VpnUI.Kill();
        VpnService.RestartService();

        var cliOutput = VpnCli.SendCommandsViaStandardInputAndWait(VpnCommands());

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
            var cliOutput2 = VpnCli.SendCommandsViaStandardInputAndWait(VpnCommands());
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

    private static List<string> VpnCommands()
    {
        var cred = CredentialHelper.GetVpnCredentials();
        return VpnCli.ConnectCommands(Options.Instance.Server, cred.UserName, cred.Password);
    }
}