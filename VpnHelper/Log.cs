using System;
using System.Diagnostics;

namespace VpnHelper;

internal class Log
{
    public static DateTime? ShowSecondsSince { get; set; }

    public static void WriteLine(string message)
    {
        if (ShowSecondsSince != null)
        {
            message = $"{DateTime.Now.Subtract(ShowSecondsSince.Value).TotalSeconds}: {message}";
        }

        Trace.WriteLine(message);
        Console.WriteLine(message);
    }
}