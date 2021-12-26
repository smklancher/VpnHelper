#nullable disable //leaving this the way it was on .net framework

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace UtilityCommon;

public class TextBoxListener : TraceListener
{
    // To use, just call TextBoxListener.SetTextBox(TextBoxToLogTo)

    private static readonly Lazy<TextBoxListener> Lazy = new Lazy<TextBoxListener>(() =>
       {
           TextBoxListener tbl = new TextBoxListener();
           tbl.items.CollectionChanged += tbl.Items_CollectionChanged;
           Trace.Listeners.Add(tbl);
           return tbl;
       });

    private readonly ObservableCollection<TraceDetails> items = new ObservableCollection<TraceDetails>();

    private LogAction logAction;
    private TextBox textBox;

    private TextBoxListener()
    {
    }

    public delegate void LogAction(TraceDetails td, TextBox tb);

    public static TextBoxListener Instance => Lazy.Value;

    public override bool IsThreadSafe => false;

    private static ObservableCollection<TraceDetails> TraceItems => Instance.items;

    /// <summary>
    /// Set the textbox to log to, and optionally how to log to it.
    /// </summary>
    /// <param name="textBox">TextBox to log to</param>
    /// <param name="logAction">Delegate for how to log each TraceDetails object.  Default: (TraceDetails td, TextBox tb) => tb.Text = $"{td}\r\n{tb.Text}"</param>
    /// <param name="includeThread">Whether or not to include the thread in the logged message when using default log action</param>
    public static void SetTextBox(TextBox textBox, LogAction logAction = null, bool includeThread = true)
    {
        if (logAction == null) { logAction = (TraceDetails td, TextBox tb) => tb.Text = $"{td.ToString(includeThread)}\r\n{tb.Text}"; }
        Instance.logAction = logAction;
        Instance.textBox = textBox;

        // Name the UI Thread for logging purposes
        try
        {
            Thread.CurrentThread.Name = "UIThread";
        }
        catch (Exception) { }
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
    {
        foreach (object o in data)
        {
            TraceEvent(eventCache, source, eventType, id, o.ToString());
        }
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
    {
        TraceEvent(eventCache, source, eventType, id, data.ToString());
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
    {
        TraceEvent(eventCache, source, eventType, id, string.Empty);
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
    {
        TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
        TraceItems.Add(new TraceDetails(eventCache.DateTime.ToLocalTime(), message, source));
    }

    public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
    {
        TraceEvent(eventCache, source, TraceEventType.Transfer, id, message);
    }

    public override void Write(string message)
    {
        TraceItems.Add(new TraceDetails(DateTime.Now, message, string.Empty));
    }

    public override void Write(object o, string category)
    {
        Write(o.ToString(), category);
    }

    public override void Write(string message, string category)
    {
        TraceItems.Add(new TraceDetails(DateTime.Now, message, category));
    }

    public override void WriteLine(string message)
    {
        Write(message);
    }

    public override void WriteLine(string message, string category)
    {
        TraceItems.Add(new TraceDetails(DateTime.Now, message, category));
    }

    public override void WriteLine(object o, string category)
    {
        WriteLine(o.ToString(), category);
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // update the textbox when items are added
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems)
            {
                var td = item as TraceDetails;
                if (textBox.InvokeRequired)
                {
                    textBox.BeginInvoke(logAction, td, textBox);
                }
                else
                {
                    logAction.Invoke(td, textBox);
                }
            }
        }
    }

    public class TraceDetails
    {
        public TraceDetails(DateTime timestamp, string message, string source, string thread = "")
        {
            Timestamp = timestamp;
            Message = message;
            Source = source;
            if (thread == string.Empty)
            {
                thread = string.IsNullOrEmpty(Thread.CurrentThread.Name) ? Thread.CurrentThread.ManagedThreadId.ToString() : Thread.CurrentThread.Name;
            }

            ThreadName = thread;
        }

        public string Message { get; }

        public string Source { get; }

        public string ThreadName { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool includeThread)
        {
            return $"{Timestamp.ToString("G")} - {(string.IsNullOrEmpty(ThreadName) || includeThread == false ? string.Empty : $"{ThreadName} - ")}{Message}";
        }
    }
}