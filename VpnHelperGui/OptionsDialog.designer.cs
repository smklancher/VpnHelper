﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VpnLinkGui;

partial class OptionsDialog
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.propertyGrid = new System.Windows.Forms.PropertyGrid();
        this.SuspendLayout();
        // 
        // propertyGrid
        // 
        this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        this.propertyGrid.Location = new System.Drawing.Point(0, 0);
        this.propertyGrid.Name = "propertyGrid";
        this.propertyGrid.Size = new System.Drawing.Size(800, 450);
        this.propertyGrid.TabIndex = 0;
        // 
        // OptionsDialog
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.propertyGrid);
        this.Name = "OptionsDialog";
        this.Text = "OptionsDialog";
        this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OptionsDialog_FormClosed);
        this.Load += new System.EventHandler(this.OptionsDialog_Load);
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PropertyGrid propertyGrid;
}
