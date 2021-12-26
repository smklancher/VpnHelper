
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
            this.SuspendLayout();
            // 
            // StatusButton
            // 
            this.StatusButton.Location = new System.Drawing.Point(14, 14);
            this.StatusButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StatusButton.Name = "StatusButton";
            this.StatusButton.Size = new System.Drawing.Size(133, 27);
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
            this.LogTextBox.Location = new System.Drawing.Point(14, 47);
            this.LogTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(905, 457);
            this.LogTextBox.TabIndex = 1;
            // 
            // ReconnectButton
            // 
            this.ReconnectButton.Location = new System.Drawing.Point(154, 14);
            this.ReconnectButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ReconnectButton.Name = "ReconnectButton";
            this.ReconnectButton.Size = new System.Drawing.Size(88, 27);
            this.ReconnectButton.TabIndex = 2;
            this.ReconnectButton.Text = "Reconnect";
            this.ReconnectButton.UseVisualStyleBackColor = true;
            this.ReconnectButton.Click += new System.EventHandler(this.ReconnectButton_Click);
            // 
            // OptionsButton
            // 
            this.OptionsButton.Location = new System.Drawing.Point(248, 14);
            this.OptionsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.OptionsButton.Name = "OptionsButton";
            this.OptionsButton.Size = new System.Drawing.Size(88, 27);
            this.OptionsButton.TabIndex = 3;
            this.OptionsButton.Text = "Options";
            this.OptionsButton.UseVisualStyleBackColor = true;
            this.OptionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
            // 
            // CredManButton
            // 
            this.CredManButton.Location = new System.Drawing.Point(343, 14);
            this.CredManButton.Name = "CredManButton";
            this.CredManButton.Size = new System.Drawing.Size(158, 27);
            this.CredManButton.TabIndex = 4;
            this.CredManButton.Text = "Open Credential Manager";
            this.CredManButton.UseVisualStyleBackColor = true;
            this.CredManButton.Click += new System.EventHandler(this.CredManButton_Click);
            // 
            // StoredPwdButton
            // 
            this.StoredPwdButton.Location = new System.Drawing.Point(507, 14);
            this.StoredPwdButton.Name = "StoredPwdButton";
            this.StoredPwdButton.Size = new System.Drawing.Size(178, 27);
            this.StoredPwdButton.TabIndex = 5;
            this.StoredPwdButton.Text = "Open Stored Passwords";
            this.StoredPwdButton.UseVisualStyleBackColor = true;
            this.StoredPwdButton.Click += new System.EventHandler(this.StoredPwdButton_Click);
            // 
            // VpnHelperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 519);
            this.Controls.Add(this.StoredPwdButton);
            this.Controls.Add(this.CredManButton);
            this.Controls.Add(this.OptionsButton);
            this.Controls.Add(this.ReconnectButton);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.StatusButton);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
    }
}

