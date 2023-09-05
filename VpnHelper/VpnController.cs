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

namespace VpnLink;

public static class VpnController
{
    // find Window elements with https://github.com/microsoft/accessibility-insights-windows
    private const string EmailTextBoxName = "Enter your email, phone, or Skype.";

    private const string HelperWindowTitle = "Cisco AnyConnect Login";
    private const string NextButtonText = "Next";
    private const string SignInButtonText = "Sign in";
    private static string PasswordTextBoxName = "Enter the password for [email]";

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

        if (!ChangeToConsoleSessionIfNeeded())
        {
            Log.WriteLine($"Not able to switch to console session to make VPN connection.");
            return false;
        }

        return true;
    }

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
        if (!AbleContinueWithConnectionAttempt()) { return; }

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

    public static void ConnectIfNeeded2()
    {
        if (!AbleContinueWithConnectionAttempt()) { return; }

        VpnUI.Kill();
        VpnService.RestartService();

        PasswordTextBoxName = $"Enter the password for {GetEmail()}";
        EnableAutomationEventHandlers();
        var vpnUiProcess = VpnUI.Start();

        if (TryFindVpnWebHelperProcess(out var helperProcess))
        {
        }

        // https://web.archive.org/web/20190205004524/https://blogs.msdn.microsoft.com/oldnewthing/20140217-00/?p=1743
        // https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-clientportal?WT.mc_id=DT-MVP-5003235
        // https://stackoverflow.com/a/40285043/221018
    }

    public static void DisableAutomationEventHandlers()
    {
        Automation.RemoveAllEventHandlers();
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

    private static void EnableAutomationEventHandlers()
    {
        //not sure why this triggers multiple times
        Automation.AddAutomationEventHandler(
          WindowPattern.WindowOpenedEvent,
          AutomationElement.RootElement,
          TreeScope.Children, LoginAutomation);
    }

    private static string GetEmail()
    {
        return CredentialHelper.GetVpnCredentials().UserName;
    }

    private static void LoginAutomation(object sender, AutomationEventArgs e)
    {
        var window = sender as AutomationElement;
        if (window == null) { return; }

        if (SetEmail(window))
        {
            // it would be better to figure out separate automation events, but waiting a bit should be good enough to get something working
            Thread.Sleep(3000);
            SetPassword(window);
        }
    }

    private static bool SetEmail(AutomationElement window)
    {
        Log.WriteLine($"Found Window {window.Current.Name}");
        if (window.Current.Name != HelperWindowTitle) return false;

        //Since it's a web page, inner elements are not loaded immediately
        Thread.Sleep(3000);
        Log.WriteLine($"Found Window {window.Current.Name}");

        var email = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, EmailTextBoxName));
        var valuePattern = email?.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
        if (email != null && valuePattern != null)
        {
            Log.WriteLine($"Found {window.Current.Name}.{email.Current.Name}");
            valuePattern.SetValue(GetEmail());
        }
        else
        {
            Log.WriteLine($"email field not found");
            return false;
        }

        var nextButton = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, NextButtonText));
        var invokePattern = nextButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
        if (nextButton != null && invokePattern != null)
        {
            Log.WriteLine($"Found {window.Current.Name}.{nextButton.Current.Name}");
            invokePattern.Invoke();
        }
        else
        {
            Log.WriteLine($"next button not found");
            return false;
        }

        return true;
    }

    private static void SetPassword(AutomationElement window)
    {
        if (window == null) { return; }

        var password = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, PasswordTextBoxName));
        var valuePattern = password?.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
        if (password != null && valuePattern != null)
        {
            Log.WriteLine($"Found {window.Current.Name}.{password.Current.Name}");

            var cred = CredentialHelper.GetVpnCredentials();
            valuePattern.SetValue(cred.Password);
        }
        else
        {
            Log.WriteLine($"password field not found");
        }

        var nextButton = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, SignInButtonText));
        var invokePattern = nextButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
        if (nextButton != null && invokePattern != null)
        {
            Log.WriteLine($"Found {window.Current.Name}.{nextButton.Current.Name}");
            invokePattern.Invoke();
        }
        else
        {
            Log.WriteLine($"signin button not found");
        }
    }

    private static bool TryFindVpnWebHelperProcess([NotNullWhen(true)] out Process process)
    {
        int elapsed = 0;
        var timeoutMilliseconds = 15_000;
        var intervalMs = 500;

        process = Process.GetProcesses().Where(x => x.ProcessName == "acwebhelper.exe").FirstOrDefault()!;
        while ((process == null) && (elapsed < timeoutMilliseconds))
        {
            Thread.Sleep(intervalMs);
            elapsed += intervalMs;
            process = Process.GetProcesses().Where(x => x.ProcessName == "acwebhelper.exe").FirstOrDefault()!;
        }

        return process != null;
    }

    private static List<string> VpnCommands()
    {
        var cred = CredentialHelper.GetVpnCredentials();
        return VpnCli.ConnectCommands(Options.Instance.Server, cred.UserName, cred.Password);
    }
}