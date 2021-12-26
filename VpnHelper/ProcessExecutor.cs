#nullable disable //might want to reevaluate this code in general anyway

using System.Diagnostics;
using System.Text;

namespace VpnLink;

public class ElevatedCommandResult
{
    private readonly int? _exitCode;
    private readonly DateTime? _exitTime;
    private readonly bool _hasExited;
    private readonly string _output;

    public ElevatedCommandResult(string output, string err, int? exitCode = null, DateTime? exitTime = null, bool hasExited = false)
    {
        _output = output;
        StdErr = err;
        _exitCode = exitCode;
        _exitTime = exitTime;
        _hasExited = hasExited;
    }

    public int? ExitCode
    {
        get { return _exitCode; }
    }

    public DateTime? ExitTime
    {
        get { return _exitTime; }
    }

    public bool HasExited
    {
        get { return _hasExited; }
    }

    public string Output
    {
        get { return _output; }
    }

    public string StdErr { get; private set; } = string.Empty;
}

/// <summary>
/// Simplified from https://github.com/borismod/OsTestFramework/blob/master/OsTestFramework/ProcessExecutor.cs
/// </summary>
public class ProcessExecutor
{
    private StringBuilder _error;
    private AutoResetEvent _errorWaitHandle;
    private TimeSpan? _executionTimeout;
    private StringBuilder _output;
    private AutoResetEvent _outputWaitHandle;
    private Process _process;
    private Action<string> errorReceivedAction;
    private Action<string> outputReceivedAction;
    private List<string> standardInputLines = new List<string>();

    public ProcessExecutor(string command, string argsStr, TimeSpan? executionTimeout = null)
    {
        _executionTimeout = executionTimeout;

        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = argsStr,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            ErrorDialog = false,
            CreateNoWindow = false
        };

        Setup(psi, executionTimeout);
    }

    public ProcessExecutor(ProcessStartInfo psi, TimeSpan? executionTimeout = null, List<string> stdInLines = null)
    {
        if (stdInLines != null && stdInLines.Count > 0)
        {
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            standardInputLines = stdInLines;
        }

        Setup(psi, executionTimeout);
    }

    public void Dispose()
    {
        UnregisterFromEvents();

        if (_process != null)
        {
            _process.Dispose();
            _process = null;
        }
        if (_errorWaitHandle != null)
        {
            _errorWaitHandle.Close();
            _outputWaitHandle = null;
        }
        if (_outputWaitHandle != null)
        {
            _outputWaitHandle.Close();
            _outputWaitHandle = null;
        }
    }

    public ElevatedCommandResult Execute()
    {
        _process.Start();

        WriteLinesToStandardInput();

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        int milliseconds = _executionTimeout.HasValue
            ? (int)_executionTimeout.Value.TotalMilliseconds
            : int.MaxValue;

        if (_process.WaitForExit(milliseconds) &&
            _outputWaitHandle.WaitOne(milliseconds) &&
            _errorWaitHandle.WaitOne(milliseconds))
        {
            return new ElevatedCommandResult(_output.ToString(), _error.ToString(), _process.ExitCode, _process.ExitTime, _process.HasExited);
        }

        return new ElevatedCommandResult(_output.ToString(), _error.ToString());
    }

    public void SetDataRecievedActions(Action<string> outputAction, Action<string> errorAction)
    {
        outputReceivedAction = outputAction;
        errorReceivedAction = errorAction;
    }

    private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null)
        {
            _errorWaitHandle.Set();
        }
        else
        {
            _error.AppendLine(e.Data);
            if (errorReceivedAction != null)
            {
                errorReceivedAction.Invoke(e.Data);
            }
        }
    }

    private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null)
        {
            _outputWaitHandle.Set();
        }
        else
        {
            _output.AppendLine(e.Data);
            if (outputReceivedAction != null)
            {
                outputReceivedAction.Invoke(e.Data);
            }
        }
    }

    private void RegisterToEvents()
    {
        _process.OutputDataReceived += process_OutputDataReceived;
        _process.ErrorDataReceived += process_ErrorDataReceived;
    }

    private void Setup(ProcessStartInfo psi, TimeSpan? executionTimeout = null)
    {
        _executionTimeout = executionTimeout;

        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false; //must be set when redirecting streams
        psi.ErrorDialog = false;

        _process = new Process
        {
            StartInfo = psi
        };

        _output = new StringBuilder();
        _error = new StringBuilder();

        Log.WriteLine("Running " + _process.StartInfo.FileName + " " + _process.StartInfo.Arguments);
        _outputWaitHandle = new AutoResetEvent(false);
        _errorWaitHandle = new AutoResetEvent(false);
        RegisterToEvents();
    }

    private void UnregisterFromEvents()
    {
        _process.OutputDataReceived -= process_OutputDataReceived;
        _process.ErrorDataReceived -= process_ErrorDataReceived;
    }

    private void WriteLinesToStandardInput()
    {
        if (_process.StartInfo.RedirectStandardInput & standardInputLines.Count > 0)
        {
            var stdIn = _process.StandardInput;
            foreach (var l in standardInputLines)
            {
                stdIn.WriteLine(l);
            }
            stdIn.Close();
        }
    }
}