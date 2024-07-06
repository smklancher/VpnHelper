using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace VpnHelper;

public class Options
{
    private static readonly Lazy<Options> Lazy =
                                new Lazy<Options>(() => new Options());

    private Options()
    {
    }

    public static Options Instance => Lazy.Value;

    [Description("Name of saved Windows credential to use.  Set in Windows Credential Manager under Windows Credentials > Generic Credentials.")]
    public string CredentialName { get; set; } = string.Empty;

    [Description("If VPN disallows connections from RDP, this uses tscon to send the session to console before connecting.")]
    public bool IsConsoleSessionRequired { get; set; } = false;

    [Description("Number of retries for a given UI automation")]
    public int NumberOfRetries { get; set; } = 10;

    [Description("Timeout for the UI automation")]
    public int OverallTimeoutSeconds { get; set; } = 120;

    [Description("Seemed like AI needed to be running, but that was likely helping with a timing issue...")]
    public bool RunAccessiblityInsightsDuringAutomation { get; set; } = false;

    [Description("VPN server to connect to")]
    public string Server { get; set; } = string.Empty;

    [Description("Include full text of VPN command line interface in log output.")]
    public bool ShowVpnCliOutput { get; set; } = false;

    [Description("Don't check if already connected: just kill and reconnect.")]
    public bool SkipExistingConnectionCheck { get; set; } = false;

    [Description("Delay milliseconds between some parts of the UI automation")]
    public int UIAutomationDelayMs { get; set; } = 500;

    public void OnCloseOptionsForm()
    {
    }
}