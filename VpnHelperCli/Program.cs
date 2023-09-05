using VpnLink;

namespace VpnLinkCli;

public class Program
{
    /// <summary>
    /// Connect to VPN if needed
    /// </summary>
    /// <param name="server">Server to connect to.</param>
    /// <param name="savedCredentialName">Name of saved Windows credential to use.  Set using cmdkey.exe or in Windows Credential Manager under Windows Credentials > Generic Credentials.</param>
    /// <param name="consoleSessionRequired">Will switch to console session before connecting if true (required in some VPN configurations).</param>
    public static void Main(string server, string savedCredentialName, bool consoleSessionRequired)
    {
        Options.Instance.Server = server;
        Options.Instance.CredentialName = savedCredentialName;
        Options.Instance.IsConsoleSessionRequired = consoleSessionRequired;
        VpnController.ConnectIfNeeded2();
    }
}