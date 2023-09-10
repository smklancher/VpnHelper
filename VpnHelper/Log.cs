using System;
using System.Diagnostics;

namespace VpnHelper;

internal class Log
{
    public static void WriteLine(string message)
    {
        Trace.WriteLine(message);
        Console.WriteLine(message);
    }
}