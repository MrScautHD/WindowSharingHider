
namespace WindowSharingHider
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.windowListCheckBox = new System.Windows.Forms.CheckedListBox();
            this.autoHideAllCheckBox = new System.Windows.Forms.CheckBox();
            this.autoHideWhenVeraCryptOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.autoHideAppPathTextBox = new System.Windows.Forms.TextBox();
            this.hideToBackgroundButton = new System.Windows.Forms.Button();
            this.autostartHiddenCheckBox = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // autoHideAllCheckBox
            // 
            this.autoHideAllCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autoHideAllCheckBox.Location = new System.Drawing.Point(0, 0);
            this.autoHideAllCheckBox.Name = "autoHideAllCheckBox";
            this.autoHideAllCheckBox.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.autoHideAllCheckBox.Size = new System.Drawing.Size(328, 28);
            this.autoHideAllCheckBox.TabIndex = 0;
            this.autoHideAllCheckBox.Text = "Auto hide all open windows";
            this.autoHideAllCheckBox.UseVisualStyleBackColor = true;
            this.autoHideAllCheckBox.CheckedChanged += new System.EventHandler(this.autoHideAllCheckBox_CheckedChanged);
            // 
            // autoHideWhenVeraCryptOpenCheckBox
            // 
            this.autoHideWhenVeraCryptOpenCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autoHideWhenVeraCryptOpenCheckBox.Location = new System.Drawing.Point(0, 28);
            this.autoHideWhenVeraCryptOpenCheckBox.Name = "autoHideWhenVeraCryptOpenCheckBox";
            this.autoHideWhenVeraCryptOpenCheckBox.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.autoHideWhenVeraCryptOpenCheckBox.Size = new System.Drawing.Size(328, 28);
            this.autoHideWhenVeraCryptOpenCheckBox.TabIndex = 1;
            this.autoHideWhenVeraCryptOpenCheckBox.Text = "Auto hide all when this app is running";
            this.autoHideWhenVeraCryptOpenCheckBox.UseVisualStyleBackColor = true;
            this.autoHideWhenVeraCryptOpenCheckBox.CheckedChanged += new System.EventHandler(this.autoHideWhenVeraCryptOpenCheckBox_CheckedChanged);
            // 
            // autoHideAppPathTextBox
            // 
            this.autoHideAppPathTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autoHideAppPathTextBox.Location = new System.Drawing.Point(0, 56);
            this.autoHideAppPathTextBox.Name = "autoHideAppPathTextBox";
            this.autoHideAppPathTextBox.Size = new System.Drawing.Size(328, 20);
            this.autoHideAppPathTextBox.TabIndex = 2;
            this.autoHideAppPathTextBox.Text = "C:\\Program Files\\VeraCrypt\\VeraCrypt.exe";
            // 
            // hideToBackgroundButton
            // 
            this.hideToBackgroundButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.hideToBackgroundButton.Location = new System.Drawing.Point(0, 76);
            this.hideToBackgroundButton.Name = "hideToBackgroundButton";
            this.hideToBackgroundButton.Size = new System.Drawing.Size(328, 28);
            this.hideToBackgroundButton.TabIndex = 3;
            this.hideToBackgroundButton.Text = "Hide app to background";
            this.hideToBackgroundButton.UseVisualStyleBackColor = true;
            this.hideToBackgroundButton.Click += new System.EventHandler(this.hideToBackgroundButton_Click);
            // 
            // autostartHiddenCheckBox
            // 
            this.autostartHiddenCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autostartHiddenCheckBox.Location = new System.Drawing.Point(0, 104);
            this.autostartHiddenCheckBox.Name = "autostartHiddenCheckBox";
            this.autostartHiddenCheckBox.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.autostartHiddenCheckBox.Size = new System.Drawing.Size(328, 28);
            this.autostartHiddenCheckBox.TabIndex = 4;
            this.autostartHiddenCheckBox.Text = "Start with Windows hidden";
            this.autostartHiddenCheckBox.UseVisualStyleBackColor = true;
            this.autostartHiddenCheckBox.CheckedChanged += new System.EventHandler(this.autostartHiddenCheckBox_CheckedChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            this.notifyIcon.Text = "Window Sharing Hider";
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // windowListCheckBox
            // 
            this.windowListCheckBox.CheckOnClick = true;
            this.windowListCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowListCheckBox.FormattingEnabled = true;
            this.windowListCheckBox.Location = new System.Drawing.Point(0, 132);
            this.windowListCheckBox.Name = "windowListCheckBox";
            this.windowListCheckBox.Size = new System.Drawing.Size(328, 252);
            this.windowListCheckBox.TabIndex = 5;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 384);
            this.Controls.Add(this.windowListCheckBox);
            this.Controls.Add(this.autostartHiddenCheckBox);
            this.Controls.Add(this.hideToBackgroundButton);
            this.Controls.Add(this.autoHideAppPathTextBox);
            this.Controls.Add(this.autoHideWhenVeraCryptOpenCheckBox);
            this.Controls.Add(this.autoHideAllCheckBox);
            this.Name = "MainWindow";
            this.Text = "Window Sharing Hider";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox windowListCheckBox;
        private System.Windows.Forms.CheckBox autoHideAllCheckBox;
        private System.Windows.Forms.CheckBox autoHideWhenVeraCryptOpenCheckBox;
        private System.Windows.Forms.TextBox autoHideAppPathTextBox;
        private System.Windows.Forms.Button hideToBackgroundButton;
        private System.Windows.Forms.CheckBox autostartHiddenCheckBox;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}

