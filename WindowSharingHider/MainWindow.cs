using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WindowSharingHider
{
    public partial class MainWindow : Form
    {
        public class WindowInfo
        {
            public String Title { get; set; }
            public IntPtr Handle { get; set; }
            public Boolean stillExists = false;
            public override string ToString()
            {
                return Title;
            }
        }
        public MainWindow(Boolean startHidden = false)
        {
            InitializeComponent();
            InitializeAutostartCheckBox();
            if (startHidden) Shown += (sender, e) => HideToBackground();
            var timer = new Timer();
            timer.Interval = 500;
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        const String AutostartRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const String AutostartValueName = "WindowSharingHider";
        const Int32 VisibleAffinity = 0x0;
        const Int32 HiddenAffinity = 0x11;
        readonly Dictionary<IntPtr, Int32> automaticHideOriginalAffinities = new Dictionary<IntPtr, Int32>();
        Boolean automaticHideActive = false;
        Boolean initializingControls = false;
        Boolean flagToPreserveSettings = false;
        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (WindowInfo window in windowListCheckBox.Items) window.stillExists = false;
            var currWindows = WindowHandler.GetVisibleWindows();
            foreach (var window in currWindows)
            {
                var existingWindow = windowListCheckBox.Items.Cast<WindowInfo>().FirstOrDefault(i => i.Handle == window.Key);
                if (existingWindow != null)
                {
                    existingWindow.stillExists = true;
                    existingWindow.Title = window.Value;
                }
                else windowListCheckBox.Items.Add(new WindowInfo { Title = window.Value, Handle = window.Key, stillExists = true });
            }
            foreach (var window in windowListCheckBox.Items.Cast<WindowInfo>().ToArray()) if (window.stillExists == false) windowListCheckBox.Items.Remove(window);

            var shouldAutoHideAll = ShouldAutoHideAll();
            if (!shouldAutoHideAll && automaticHideActive) RestoreAutomaticHideOriginalAffinities();
            automaticHideActive = shouldAutoHideAll;

            foreach (var window in windowListCheckBox.Items.Cast<WindowInfo>().ToArray())
            {
                var status = WindowHandler.GetWindowDisplayAffinity(window.Handle);
                var index = windowListCheckBox.Items.IndexOf(window);
                if (shouldAutoHideAll)
                {
                    if (!automaticHideOriginalAffinities.ContainsKey(window.Handle)) automaticHideOriginalAffinities[window.Handle] = status;
                    if (status != HiddenAffinity)
                    {
                        WindowHandler.SetWindowDisplayAffinity(window.Handle, HiddenAffinity);
                        status = WindowHandler.GetWindowDisplayAffinity(window.Handle);
                    }
                }
                else
                {
                    var target = windowListCheckBox.GetItemChecked(index) ? HiddenAffinity : VisibleAffinity;
                    if (target != status && flagToPreserveSettings)
                    {
                        WindowHandler.SetWindowDisplayAffinity(window.Handle, target);
                        status = WindowHandler.GetWindowDisplayAffinity(window.Handle);
                    }
                }
                windowListCheckBox.SetItemChecked(index, status > 0);
            }
            flagToPreserveSettings = true;
        }

        private void autoHideAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Timer_Tick(sender, e);
        }

        private void autoHideWhenVeraCryptOpenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Timer_Tick(sender, e);
        }

        private void hideToBackgroundButton_Click(object sender, EventArgs e)
        {
            HideToBackground();
        }

        private void autostartHiddenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (initializingControls) return;

            SetAutostartHidden(autostartHiddenCheckBox.Checked);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) RestoreFromBackground();
        }

        private Boolean ShouldAutoHideAll()
        {
            return autoHideAllCheckBox.Checked || (autoHideWhenVeraCryptOpenCheckBox.Checked && IsConfiguredAppRunning());
        }

        private Boolean IsConfiguredAppRunning()
        {
            var configuredPath = autoHideAppPathTextBox.Text.Trim().Trim('"');
            if (String.IsNullOrWhiteSpace(configuredPath)) return false;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var processPath = process.MainModule?.FileName;
                    if (String.Equals(processPath, configuredPath, StringComparison.OrdinalIgnoreCase)) return true;
                }
                catch
                {
                    if (String.Equals(process.ProcessName, Path.GetFileNameWithoutExtension(configuredPath), StringComparison.OrdinalIgnoreCase)) return true;
                }
                finally
                {
                    process.Dispose();
                }
            }
            return false;
        }

        private void RestoreAutomaticHideOriginalAffinities()
        {
            foreach (var window in windowListCheckBox.Items.Cast<WindowInfo>().ToArray())
            {
                if (!automaticHideOriginalAffinities.TryGetValue(window.Handle, out Int32 originalAffinity)) continue;

                var index = windowListCheckBox.Items.IndexOf(window);
                WindowHandler.SetWindowDisplayAffinity(window.Handle, originalAffinity);
                windowListCheckBox.SetItemChecked(index, originalAffinity > 0);
            }
            automaticHideOriginalAffinities.Clear();
        }

        private void HideToBackground()
        {
            notifyIcon.Visible = true;
            ShowInTaskbar = false;
            Hide();
        }

        private void RestoreFromBackground()
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Activate();
            notifyIcon.Visible = false;
        }

        private void InitializeAutostartCheckBox()
        {
            initializingControls = true;
            autostartHiddenCheckBox.Checked = IsAutostartHiddenEnabled();
            initializingControls = false;
        }

        private Boolean IsAutostartHiddenEnabled()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(AutostartRegistryPath, false))
            {
                var value = key?.GetValue(AutostartValueName) as String;
                return String.Equals(value, GetAutostartCommand(), StringComparison.OrdinalIgnoreCase);
            }
        }

        private void SetAutostartHidden(Boolean enabled)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(AutostartRegistryPath))
            {
                if (enabled) key?.SetValue(AutostartValueName, GetAutostartCommand());
                else key?.DeleteValue(AutostartValueName, false);
            }
        }

        private static String GetAutostartCommand()
        {
            return "\"" + Application.ExecutablePath + "\" --hidden";
        }
    }
}
