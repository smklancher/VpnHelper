using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using VpnHelper;

namespace VpnHelper
{
    public static class UIAutomation
    {
        // https://web.archive.org/web/20190205004524/https://blogs.msdn.microsoft.com/oldnewthing/20140217-00/?p=1743
        // https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-clientportal?WT.mc_id=DT-MVP-5003235
        // https://stackoverflow.com/a/40285043/221018

        public static bool AcceptButtonComplete = false;
        public static bool Failed = false;
        public static bool LoginAutomationComplete = false;
        private const string AcceptButtonDialogTitle = "Cisco AnyConnect";

        // find Window elements with https://github.com/microsoft/accessibility-insights-windows
        private const string EmailTextBoxName = "Enter your email, phone, or Skype.";

        private const string HelperWindowTitle = "Cisco AnyConnect Login";
        private const string NextButtonText = "Next";
        private const string SignInButtonText = "Sign in";
        private static Process? AccessibilityInsightsProcess;
        private static string PasswordTextBoxName = "Enter the password for [email]";

        private static int WaitForLoadMs = 5000;

        private static Action? AcceptButtonAutomationSuccessCallback { get; set; }
        private static Action? LoginAutomationSuccessCallback { get; set; }
        private static Action? TimeoutCallback { get; set; }

        public static void AutomationWithTimeout(int timeoutSeconds, Action? onLoginSuccess, Action? onAcceptButtonSuccess, Action? onTimeout)
        {
            Failed = false;
            LoginAutomationComplete = false;
            AcceptButtonComplete = false;

            Task.Delay(timeoutSeconds * 1000).ContinueWith(_ =>
            {
                if (!(LoginAutomationComplete && AcceptButtonComplete) & !Failed)
                {
                    Log.WriteLine($"Login automation complete: {LoginAutomationComplete}, accept button automation complelte: {AcceptButtonComplete}");
                    Log.WriteLine($"Automation timed out after {timeoutSeconds} seconds.");
                    WrapUp(false);
                    TimeoutCallback?.Invoke();
                }
            });

            TimeoutCallback = onTimeout;
            LoginAutomationSuccessCallback = onLoginSuccess;
            AcceptButtonAutomationSuccessCallback = onAcceptButtonSuccess;
            EnableAutomationEventHandlers();
        }

        public static void WrapUp(bool automationSuccess)
        {
            StopAccessibilityInsights();
            DisableAutomationEventHandlers();
        }

        private static void AcceptButtonAutomation(object sender, AutomationEventArgs e)
        {
            var window = sender as AutomationElement;
            if (window == null) { return; }

            Log.WriteLine($"(AcceptButton) Found Window {window.Current.Name}");
            if (window.Current.Name != AcceptButtonDialogTitle) return;

            var acceptButton = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Accept"));
            var invokePattern = acceptButton?.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (acceptButton != null && invokePattern != null)
            {
                Log.WriteLine($"Found {window.Current.Name}.{acceptButton.Current.Name}");
                invokePattern.Invoke();

                Log.WriteLine($"Accept button automation complete");
                AcceptButtonComplete = true;
                WrapUp(true);
                AcceptButtonAutomationSuccessCallback?.Invoke();
            }
            else
            {
                Fail($"accept button not found");
            }
        }

        private static void DisableAutomationEventHandlers()
        {
            Automation.RemoveAllEventHandlers();
        }

        private static void EnableAutomationEventHandlers()
        {
            LoginAutomationComplete = false;
            StartAccessibilityInsights();
            PasswordTextBoxName = $"Enter the password for {GetEmail()}";

            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, LoginAutomation);

            // enable accept button automation
            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Descendants, AcceptButtonAutomation);
        }

        private static void Fail(string message)
        {
            Log.WriteLine(message);
            WrapUp(false);
            TimeoutCallback?.Invoke();
        }

        private static string GetEmail()
        {
            return CredentialHelper.GetVpnCredentials().UserName;
        }

        private static void LoginAutomation(object sender, AutomationEventArgs e)
        {
            var window = sender as AutomationElement;
            if (window == null) { return; }

            if (SetEmail(window))
            {
                // it would be better to figure out separate automation events, but waiting a bit should be good enough to get something working
                Thread.Sleep(WaitForLoadMs);
                var finished = SetPassword(window);
                LoginAutomationComplete = true;

                Log.WriteLine("Login automation success, waiting for accept button...");
                LoginAutomationSuccessCallback?.Invoke();
            }
        }

        private static bool SetEmail(AutomationElement window)
        {
            Log.WriteLine($"(SetEmail) Found Window {window.Current.Name}");
            if (window.Current.Name != HelperWindowTitle) return false;

            //Since it's a web page, inner elements are not loaded immediately
            Thread.Sleep(WaitForLoadMs);
            //Log.WriteLine($"Found Window {window.Current.Name}");


            // This seems like it only works when accessiblity insights utilty is running...
            var email = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, EmailTextBoxName));
            var tries = 3;
            while (email==null && tries > 0)
            {
                Log.WriteLine($"Trying to find email textbox again...");
                email = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, EmailTextBoxName));
                tries--;

                if (email == null)
                {
                    Thread.Sleep(WaitForLoadMs);
                }
            }

            var valuePattern = email?.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            if (email != null && valuePattern != null)
            {
                Log.WriteLine($"Found {window.Current.Name}.{email.Current.Name}");
                valuePattern.SetValue(GetEmail());
            }
            else
            {
                Fail($"email field not found");
                return false;
            }

            var nextButton = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, NextButtonText));
            var invokePattern = nextButton?.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (nextButton != null && invokePattern != null)
            {
                Log.WriteLine($"Found {window.Current.Name}.{nextButton.Current.Name}");
                invokePattern.Invoke();
            }
            else
            {
                Fail($"next button not found");
                return false;
            }

            return true;
        }

        private static bool SetPassword(AutomationElement window)
        {
            if (window == null) { return false; }

            var password = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, PasswordTextBoxName));
            var valuePattern = password?.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            if (password != null && valuePattern != null)
            {
                Log.WriteLine($"Found {window.Current.Name}.{password.Current.Name}");

                var cred = CredentialHelper.GetVpnCredentials();
                valuePattern.SetValue(cred.Password);
            }
            else
            {
                Fail($"password field not found");
                return false;
            }

            var nextButton = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, SignInButtonText));
            var invokePattern = nextButton?.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (nextButton != null && invokePattern != null)
            {
                Log.WriteLine($"Found {window.Current.Name}.{nextButton.Current.Name}");
                invokePattern.Invoke();
            }
            else
            {
                Fail($"signin button not found");
                return false;
            }

            return true;
        }

        private static void StartAccessibilityInsights()
        {
            // Currently it looks like automation is only working with AccessibilityInsights is actually running
            // for now that's find and we'll just run and kill it as needed

            // To not need to run it, possibly need the uiAccess flag in manifest?
            // Visual Studio won't even try to lanch the process with uiAccess, thus looks like it would need to meet requirements:
            /* https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-securityoverview?f1url=%3FappId%3DDev10IDEF1%26l%3DEN-US%26k%3Dk%2528VS.DEBUG.ERROR.LAUNCH_ELEVATION_REQUIREMENTS%2529%26rd%3Dtrue
                To use UIAccess, an assistive technology application needs to:

                Be signed with a certificate to interact with applications running at a higher privilege level.
                Be trusted by the system. The application must be installed in a secure location that requires a user account control (UAC) prompt for access. For example, the Program Files folder.
                Be built with a manifest file that includes the uiAccess flag.

             */
            if (Options.Instance.RunAccessiblityInsightsDuringAutomation)
            {
                AccessibilityInsightsProcess = new Process
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = @"C:\Program Files (x86)\AccessibilityInsights\1.1\AccessibilityInsights.exe",
                        WindowStyle = ProcessWindowStyle.Minimized,
                    }
                };
                AccessibilityInsightsProcess.Start();

                Log.WriteLine($"Started AccessibilityInsights PID:{AccessibilityInsightsProcess.Id}");
            }
        }

        private static void StopAccessibilityInsights()
        {
            if (AccessibilityInsightsProcess == null) { return; }
            try
            {
                //var closeRequested = AccessibilityInsightsProcess.CloseMainWindow();
                //Log.WriteLine($"Asked AccessibilityInsights to close: {closeRequested}, PID:{AccessibilityInsightsProcess.Id}");
                //if (!closeRequested || AccessibilityInsightsProcess.WaitForExit(3000))
                //{
                Log.WriteLine($"Killing AccessibilityInsights PID:{AccessibilityInsightsProcess.Id}");
                AccessibilityInsightsProcess.Kill();
                Log.WriteLine($"AccessibilityInsights, HasExited:{AccessibilityInsightsProcess.HasExited} ExitCode:{AccessibilityInsightsProcess.ExitCode}");
                //}
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Unable to close AccessibilityInsights: {ex}");
            }
        }
    }
}