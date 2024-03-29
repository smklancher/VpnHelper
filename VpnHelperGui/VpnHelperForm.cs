﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using UtilityCommon;
using VpnHelper;

namespace VpnLinkGui;

// run as service? https://devblogs.microsoft.com/ifdef-windows/creating-a-windows-service-with-c-net5/
//get credentials: https://github.com/AdysTech/CredentialManager

public partial class VpnHelperForm : Form
{
    private const string SettingsKey = @"SOFTWARE\smklancher\VpnLinkGui";
    private RegistryLocation CredentialName;
    private RegistryLocation IsConsoleSessionRequired;
    private bool IsTestMonitoring = false;
    private RegistryLocation Server;
    private RegistryLocation ShowVpnCliOutput;

    public VpnHelperForm()
    {
        InitializeComponent();

        Server = new RegistryLocation(RegistryHive.CurrentUser, SettingsKey, "Server");
        IsConsoleSessionRequired = new RegistryLocation(RegistryHive.CurrentUser, SettingsKey, "IsConsoleSessionRequired");
        CredentialName = new RegistryLocation(RegistryHive.CurrentUser, SettingsKey, "CredentialName");
        ShowVpnCliOutput = new RegistryLocation(RegistryHive.CurrentUser, SettingsKey, "ShowVpnCliOutput");
    }

    private void CredManButton_Click(object sender, EventArgs e)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "control",
            Arguments = "keymgr.dll"
        };
        Process.Start(psi);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        TextBoxListener.SetTextBox(LogTextBox);
        LoadSettings();
    }

    private void ListAllCredentialsButton_Click(object sender, EventArgs e)
    {
        Trace.WriteLine("\r\n" + CredentialHelper.AllCredentials());
    }

    private void LoadSettings()
    {
        Options.Instance.Server = Server.Value;
        Options.Instance.CredentialName = CredentialName.Value;
        bool result = false;
        bool.TryParse(IsConsoleSessionRequired.Value, out result);
        Options.Instance.IsConsoleSessionRequired = result;
        result = false;
        bool.TryParse(ShowVpnCliOutput.Value, out result);
        Options.Instance.ShowVpnCliOutput = result;
    }

    private void OptionsButton_Click(object sender, EventArgs e)
    {
        OptionsDialog.ShowOptions(Options.Instance, this);
    }

    private void ReconnectButton_Click(object sender, EventArgs e)
    {
        VpnController.ConnectWithUIAutomation(
            () => { Trace.WriteLine("Login Success"); },
            () => { Trace.WriteLine("Accept button Success"); },
            () => Trace.WriteLine("No Success"));
    }

    private void SaveSettings()
    {
        try
        {
            Server.Value = Options.Instance.Server;
            CredentialName.Value = Options.Instance.CredentialName;
            IsConsoleSessionRequired.Value = Options.Instance.IsConsoleSessionRequired.ToString();
            ShowVpnCliOutput.Value = Options.Instance.ShowVpnCliOutput.ToString();
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
        }
    }

    private void StatusButton_Click(object sender, EventArgs e)
    {
        VpnController.IsConnected();
    }

    private void StoredPwdButton_Click(object sender, EventArgs e)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "rundll32.exe",
            Arguments = "keymgr.dll, KRShowKeyMgr"
        };
        Process.Start(psi);
    }

    private void TestUIButton_Click(object sender, EventArgs e)
    {
        if (IsTestMonitoring)
        {
            UIAutomationTesting.DisableAutomationEventHandlers();
            IsTestMonitoring = false;
        }
        else
        {
            UIAutomationTesting.EnableAutomationEventHandlers();
            IsTestMonitoring = true;
        }
    }

    private void VpnLinkForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        SaveSettings();
        UIAutomation.WrapUp(false);
    }
}