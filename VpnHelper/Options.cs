using System.ComponentModel;

namespace VpnLink;

public class Options
{
    private static readonly Lazy<Options> Lazy =
                                new Lazy<Options>(() => new Options());

    private Options()
    {
    }

    public static Options Instance => Lazy.Value;

    [Description("Name of saved Windows credential to use.  Set in Windows Credential Manager under Windows Credentials > Generic Credentials.")]
    public string CredentialName { get; set; } = String.Empty;

    [Description("If VPN disallows connections from RDP, this uses tscon to send the session to console before connecting.")]
    public bool IsConsoleSessionRequired { get; set; } = false;

    [Description("VPN server to connect to")]
    public string Server { get; set; } = String.Empty;

    [Description("Include full text of VPN command line interface in log output.")]
    public bool ShowVpnCliOutput { get; set; } = false;

    public void OnCloseOptionsForm()
    {
    }
}