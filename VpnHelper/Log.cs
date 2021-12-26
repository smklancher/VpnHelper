using System.Diagnostics;

namespace VpnLink;

internal class Log
{
    public static void WriteLine(string message)
    {
        Trace.WriteLine(message);
        Console.WriteLine(message);
    }
}