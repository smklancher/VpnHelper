using System.Diagnostics;
using Microsoft.Win32;

namespace VpnLinkGui;

public class RegistryLocation
{
    public RegistryLocation(RegistryHive basekey, string subkey, string keyname, string defaultValue = "")
    {
        BaseKey = basekey;
        SubKey = subkey;
        KeyName = keyname;
        DefaultValue = defaultValue;
    }

    public RegistryHive BaseKey { get; init; } = RegistryHive.LocalMachine;

    public string DefaultValue { get; init; }

    public string FullKey => $"{BaseKeyName()}\\{SubKey}";

    /// <summary>
    /// Officially "value": https://blogs.msdn.microsoft.com/oldnewthing/20090204-00/?p=19263
    /// </summary>
    public string KeyName { get; init; }

    public RegistryView RegistryView { get; set; } = RegistryView.Default;

    public string SubKey { get; init; }

    public RegistryValueKind Type { get; set; } = RegistryValueKind.String;

    public string Value
    {
        get => Read();

        set => Write(value);
    }

    public string BaseKeyName()
    {
        switch (BaseKey)
        {
        case RegistryHive.ClassesRoot:
            return "HKEY_CLASSES_ROOT";

        case RegistryHive.CurrentUser:
            return "HKEY_CURRENT_USER";

        case RegistryHive.LocalMachine:
            return "HKEY_LOCAL_MACHINE";

        case RegistryHive.Users:
            return "HKEY_USERS";

        case RegistryHive.PerformanceData:
            return "HKEY_PERFORMANCE_DATA";

        case RegistryHive.CurrentConfig:
            return "HKEY_CURRENT_CONFIG";

        default:
            return string.Empty;
        }
    }

    /// <summary>
    /// Jump to location in regedit by setting last key.  Key bitness (Wow6432Node or not) must match version of regedit opened, which appears to be based on parent process bitness.
    /// </summary>
    public void OpenRegEdit()
    {
        try
        {
            var last = new RegistryLocation(
                RegistryHive.CurrentUser,
                @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit",
                "LastKey"
            );

            //Write the current key as the last key used in regedit, thus will reopen to it
            last.Write($"Computer\\{FullKey}");
        }
        catch (Exception e)
        {
            Debug.Print(e.Message + ": " + FullKey);
        }

        Process.Start("regedit");
    }

    public string Read()
    {
        try
        {
            var rk = RegistryKey.OpenBaseKey(BaseKey, RegistryView);
            var sk1 = rk.OpenSubKey(SubKey);

            // Return the value as string, or empty string if key or value does not exist
            return sk1?.GetValue(KeyName)?.ToString() ?? string.Empty;
        }
        catch (Exception e)
        {
            Debug.Print(e.Message + ": " + KeyName.ToUpper());
            return string.Empty;
        }
    }

    public void ResetToDefault()
    {
        Value = DefaultValue;
    }

    public override string ToString()
    {
        return $"{FullKey}\\{KeyName}";
    }

    public void Write(string Value)
    {
        var rk = RegistryKey.OpenBaseKey(BaseKey, RegistryView);
        var sk1 = rk.CreateSubKey(SubKey);
        sk1.SetValue(KeyName, Value, Type);
    }
}