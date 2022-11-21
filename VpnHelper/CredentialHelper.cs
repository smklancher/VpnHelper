using System.Net;
using System.Text;
using AdysTech.CredentialManager;

namespace VpnLink;

public static class CredentialHelper
{
    public static string AllCredentials()
    {
        var list = CredentialManager.EnumerateICredentials();

        var sb = new StringBuilder();
        foreach (var cred in list)
        {
            if (!string.IsNullOrEmpty(cred.CredentialBlob) && cred.CredentialBlob.Length < 200)
            {
                sb.AppendLine($"{cred.Type} - {cred.TargetName}: {cred.UserName} : {cred.CredentialBlob}");
            }
        }

        return sb.ToString();
    }

    public static NetworkCredential GetVpnCredentials()
    {
        // Only seems to return password for generic type.  Should check source and win32 api to see if windows type is possible or not.
        var cred = CredentialManager.GetCredentials(SavedCredentialName(), CredentialType.Generic);

        return cred;
    }

    public static string SavedCredentialName() => Options.Instance.CredentialName;

    public static bool SavedCredentialsAvailable()
    {
        try
        {
            return CredentialManager.GetCredentials(SavedCredentialName(), CredentialType.Generic) != null;
        }
        catch (Exception ex)
        {
            Log.WriteLine($"Error getting saved credential {SavedCredentialName()}: {ex.Message}");
        }

        return false;
    }
}