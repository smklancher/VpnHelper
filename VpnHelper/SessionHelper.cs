using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VpnLink;

public class SessionHelper
{
    public static bool IsConsoleActiveSession()
    {
        var pe = new ProcessExecutor("cmd.exe", "/c query session");
        var result = pe.Execute();

        Log.WriteLine(result.Output);
        Log.WriteLine(result.StdErr);

        using (StringReader reader = new StringReader(result.Output))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("console") && line.Contains("Active"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void SendCurrentSessionToConsole()
    {
        //need the real system32 folder even if runnin as 32 bit process
        // https://docs.microsoft.com/en-us/windows/win32/winprog64/file-system-redirector?redirectedfrom=MSDN
        var systemdir = string.Empty;
        if (Environment.Is64BitProcess)
        {
            systemdir = @"%windir%\System32\";
        }
        else
        {
            systemdir = @"%windir%\Sysnative\"; //this doesn't seem to work, so unless it does, this needs to be called by a 64-bit process
        }

        var curSession = Process.GetCurrentProcess().SessionId;

        string command = $"{Environment.ExpandEnvironmentVariables(systemdir)}\\tscon.exe";
        var args = $"{curSession} /dest:console";

        var logtxt = $"{DateTime.Now.ToString("G")} \r\nRunning command: {command} {args}";
        var pe = new ProcessExecutor(command, args);

        var result = pe.Execute();

        Log.WriteLine(result.Output);
        Log.WriteLine(result.StdErr);

        logtxt += $"StrOut:\r\n{result.Output}\r\n\r\nStdErr:\r\n{result.StdErr}\r\n\r\n";
        var log = Path.Combine(AppContext.BaseDirectory, "TsCon.log");
        File.AppendAllText(log, logtxt);
    }

    private void IsConsoleActiveSessionAPI()
    {
        // potentially more robust than using the query session command is to use win32 apis
        // maybe necessary when running as a service?
        // https://stackoverflow.com/questions/3197138/how-to-get-currently-logged-users-session-id
        // https://gist.github.com/heri16/8f69aa919ee1d87f3bb53255ef78f188
        // https://fleexlab.blogspot.com/2015/04/remote-desktop-surprise.html
        // https://docs.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtsregistersessionnotification
        // https://www.pinvoke.net/default.aspx/wtsapi32.wtsregistersessionnotification
    }
}