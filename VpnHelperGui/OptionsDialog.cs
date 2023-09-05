using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VpnLink;

namespace VpnLinkGui;

public partial class OptionsDialog : Form
{
    public OptionsDialog()
    {
        InitializeComponent();
    }

    public static void ShowOptions(object options, Form parentform)
    {
        var form = new OptionsDialog();
        form.propertyGrid.SelectedObject = options;
        if (parentform == null)
        {
            form.Show();
        }
        else
        {
            form.ShowDialog(parentform);
        }
    }

    private void OptionsDialog_FormClosed(object sender, FormClosedEventArgs e)
    {
        Options.Instance.OnCloseOptionsForm();
    }

    private void OptionsDialog_Load(object sender, EventArgs e)
    {
    }
}