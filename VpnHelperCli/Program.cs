using VpnHelper;

namespace VpnHelperCli;

public class Program
{
    public static void AcceptButtonSuccess()
    {
        ConsoleUtilities.CancelReadLine();
    }

    public static void Failure()
    {
        ConsoleUtilities.CancelReadLine();
        Environment.ExitCode = -1;
    }

    public static void LoginButtonSuccess()
    {
        Console.WriteLine("Login automation success, now waiting for accept button after MFA is finished...");
    }

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
        VpnController.ConnectWithUIAutomation(LoginButtonSuccess, AcceptButtonSuccess, Failure);

        Console.WriteLine("Waiting for UI automation...");
        ConsoleUtilities.ReadLine("");
    }
}