using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace VpnLinkGui
{
    partial class VpnHelperForm
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
            this.StatusButton = new System.Windows.Forms.Button();
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.ReconnectButton = new System.Windows.Forms.Button();
            this.OptionsButton = new System.Windows.Forms.Button();
            this.CredManButton = new System.Windows.Forms.Button();
            this.StoredPwdButton = new System.Windows.Forms.Button();
            this.ListAllCredentialsButton = new System.Windows.Forms.Button();
            this.TestUIButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StatusButton
            // 
            this.StatusButton.Location = new System.Drawing.Point(12, 12);
            this.StatusButton.Name = "StatusButton";
            this.StatusButton.Size = new System.Drawing.Size(114, 23);
            this.StatusButton.TabIndex = 0;
            this.StatusButton.Text = "Check Status";
            this.StatusButton.UseVisualStyleBackColor = true;
            this.StatusButton.Click += new System.EventHandler(this.StatusButton_Click);
            // 
            // LogTextBox
            // 
            this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTextBox.Location = new System.Drawing.Point(12, 41);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(839, 397);
            this.LogTextBox.TabIndex = 1;
            // 
            // ReconnectButton
            // 
            this.ReconnectButton.Location = new System.Drawing.Point(132, 12);
            this.ReconnectButton.Name = "ReconnectButton";
            this.ReconnectButton.Size = new System.Drawing.Size(75, 23);
            this.ReconnectButton.TabIndex = 2;
            this.ReconnectButton.Text = "Reconnect";
            this.ReconnectButton.UseVisualStyleBackColor = true;
            this.ReconnectButton.Click += new System.EventHandler(this.ReconnectButton_Click);
            // 
            // OptionsButton
            // 
            this.OptionsButton.Location = new System.Drawing.Point(213, 12);
            this.OptionsButton.Name = "OptionsButton";
            this.OptionsButton.Size = new System.Drawing.Size(75, 23);
            this.OptionsButton.TabIndex = 3;
            this.OptionsButton.Text = "Options";
            this.OptionsButton.UseVisualStyleBackColor = true;
            this.OptionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
            // 
            // CredManButton
            // 
            this.CredManButton.Location = new System.Drawing.Point(294, 12);
            this.CredManButton.Name = "CredManButton";
            this.CredManButton.Size = new System.Drawing.Size(154, 23);
            this.CredManButton.TabIndex = 4;
            this.CredManButton.Text = "Open Credential Manager";
            this.CredManButton.UseVisualStyleBackColor = true;
            this.CredManButton.Click += new System.EventHandler(this.CredManButton_Click);
            // 
            // StoredPwdButton
            // 
            this.StoredPwdButton.Location = new System.Drawing.Point(458, 12);
            this.StoredPwdButton.Name = "StoredPwdButton";
            this.StoredPwdButton.Size = new System.Drawing.Size(153, 23);
            this.StoredPwdButton.TabIndex = 5;
            this.StoredPwdButton.Text = "Open Stored Passwords";
            this.StoredPwdButton.UseVisualStyleBackColor = true;
            this.StoredPwdButton.Click += new System.EventHandler(this.StoredPwdButton_Click);
            // 
            // ListAllCredentialsButton
            // 
            this.ListAllCredentialsButton.Location = new System.Drawing.Point(615, 12);
            this.ListAllCredentialsButton.Name = "ListAllCredentialsButton";
            this.ListAllCredentialsButton.Size = new System.Drawing.Size(117, 23);
            this.ListAllCredentialsButton.TabIndex = 7;
            this.ListAllCredentialsButton.Text = "List All Credentials";
            this.ListAllCredentialsButton.UseVisualStyleBackColor = true;
            this.ListAllCredentialsButton.Click += new System.EventHandler(this.ListAllCredentialsButton_Click);
            // 
            // TestUIButton
            // 
            this.TestUIButton.Location = new System.Drawing.Point(738, 12);
            this.TestUIButton.Name = "TestUIButton";
            this.TestUIButton.Size = new System.Drawing.Size(108, 23);
            this.TestUIButton.TabIndex = 8;
            this.TestUIButton.Text = "Toggle UI Monitor";
            this.TestUIButton.UseVisualStyleBackColor = true;
            this.TestUIButton.Click += new System.EventHandler(this.TestUIButton_Click);
            // 
            // VpnHelperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 450);
            this.Controls.Add(this.TestUIButton);
            this.Controls.Add(this.ListAllCredentialsButton);
            this.Controls.Add(this.StoredPwdButton);
            this.Controls.Add(this.CredManButton);
            this.Controls.Add(this.OptionsButton);
            this.Controls.Add(this.ReconnectButton);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.StatusButton);
            this.Name = "VpnHelperForm";
            this.Text = "VPN Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VpnLinkForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StatusButton;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.Button ReconnectButton;
        private System.Windows.Forms.Button OptionsButton;
        private Button CredManButton;
        private Button StoredPwdButton;
        private Button ListAllCredentialsButton;
        private Button TestUIButton;
    }
}

