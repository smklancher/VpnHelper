using VpnHelper;

namespace VpnHelperCli;

public class Program
{
    private static bool alreadyFinished = false;

    public static void AcceptButtonSuccess()
    {
        ConsoleUtilities.CancelReadLine();
        alreadyFinished = true;
    }

    public static void Failure()
    {
        Environment.ExitCode = -1;
        ConsoleUtilities.CancelReadLine();
        alreadyFinished = true;
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

        if (!alreadyFinished)
        {
            Console.WriteLine("Waiting for UI automation...");
            ConsoleUtilities.ReadLine("");
        }
    }
}