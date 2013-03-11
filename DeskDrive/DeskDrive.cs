// Copyright (c) 2006 Blue Onion Software
// All rights reserved

namespace BlueOnion
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using BlueOnion.Properties;
    using Microsoft.Win32;

    public partial class DeskDrive : Form
    {
        readonly ShortcutCollection shortcuts = new ShortcutCollection();

        public DeskDrive()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("nl-NL");
            InitializeComponent();
            Localize();
            LoadSettings();
            Desktop.LoadIconPositions();

            CDCheckBox.CheckedChanged += CheckedChanged;
            removableCheckBox.CheckedChanged += CheckedChanged;
            fixedCheckBox.CheckedChanged += CheckedChanged;
            networkedCheckBox.CheckedChanged += CheckedChanged;
            ramCheckBox.CheckedChanged += CheckedChanged;
            hideTrayCheckBox.CheckedChanged += CheckedChanged;
            startupCheckBox.CheckedChanged += CheckedChanged;
            excludedTextBox.LostFocus += ExcludedTextBoxLostFocus;
            minimizeAllCheckBox.CheckedChanged += CheckedChanged;
            locusEffectCheckBox.CheckedChanged += CheckedChanged;
            rememberIconPositionsCheckBox.CheckedChanged += CheckedChanged;
            openExplorerCheckBox.CheckedChanged += CheckedChanged;

            SetWorkingSetSize();
        }

        public void Localize()
        {
            try
            {
                using (var stream = File.OpenRead(Application.ExecutablePath + ".xml"))
                using (var xmlResourceManager = new XmlResourceManager(stream))
                {
                    Text = xmlResourceManager.GetString("DeskDrive");
                    notifyIcon.Text = xmlResourceManager.GetString("notifyIcon");
                    hideButton.Text = xmlResourceManager.GetString("hideButton");
                    label1.Text = xmlResourceManager.GetString("label1");
                    label2.Text = xmlResourceManager.GetString("label2");
                    label3.Text = xmlResourceManager.GetString("label3");
                    CDCheckBox.Text = xmlResourceManager.GetString("CDCheckBox");
                    removableCheckBox.Text = xmlResourceManager.GetString("RemovableCheckBox");
                    fixedCheckBox.Text = xmlResourceManager.GetString("FixedCheckBox");
                    networkedCheckBox.Text = xmlResourceManager.GetString("NetworkedCheckBox");
                    ramCheckBox.Text = xmlResourceManager.GetString("RamCheckBox");
                    label5.Text = xmlResourceManager.GetString("label5");
                    label6.Text = xmlResourceManager.GetString("label6");
                    linkLabel1.Text = xmlResourceManager.GetString("linkLabel1");
                    groupBox1.Text = xmlResourceManager.GetString("groupBox1");
                    label7.Text = xmlResourceManager.GetString("label7");
                    excludedTextBox.Text = xmlResourceManager.GetString("ExcludedTextBox");
                    label8.Text = xmlResourceManager.GetString("label8");
                    hideTrayCheckBox.Text = xmlResourceManager.GetString("HideTrayCheckBox");
                    startupCheckBox.Text = xmlResourceManager.GetString("StartupCheckBox");
                    showToolStripMenuItem.Text = xmlResourceManager.GetString("showToolStripMenuItem");
                    exitToolStripMenuItem.Text = xmlResourceManager.GetString("exitToolStripMenuItem");
                    minimizeAllCheckBox.Text = xmlResourceManager.GetString("minimizeAllCheckBox");
                    locusEffectCheckBox.Text = xmlResourceManager.GetString("locusEffectCheckBox");
                    rememberIconPositionsCheckBox.Text = xmlResourceManager.GetString("rememberIconPositionsCheckBox");
                    openExplorerCheckBox.Text = xmlResourceManager.GetString("openWindowsExplorerCheckBox");
                }
            }

            catch (Exception ex)
            {
                Program.LogError(ex.Message);
            }
        }

        void LoadSettings()
        {
            CDCheckBox.Checked = Settings.Default.CD;
            removableCheckBox.Checked = Settings.Default.Removable;
            fixedCheckBox.Checked = Settings.Default.Fixed;
            networkedCheckBox.Checked = Settings.Default.Networked;
            ramCheckBox.Checked = Settings.Default.Ram;
            excludedTextBox.Text = Settings.Default.Exclude;
            hideTrayCheckBox.Checked = Settings.Default.HideTrayIcon;
            startupCheckBox.Checked = Settings.Default.AutoStart;
            RegisterStartup(Settings.Default.AutoStart);
            minimizeAllCheckBox.Checked = Settings.Default.MinimizeAll;
            locusEffectCheckBox.Checked = Settings.Default.Locus;
            rememberIconPositionsCheckBox.Checked = Settings.Default.RememberIconPositions;
            openExplorerCheckBox.Checked = Settings.Default.OpenExplorer;
        }

        void SaveSettings()
        {
            Settings.Default.CD = CDCheckBox.Checked;
            Settings.Default.Removable = removableCheckBox.Checked;
            Settings.Default.Fixed = fixedCheckBox.Checked;
            Settings.Default.Networked = networkedCheckBox.Checked;
            Settings.Default.Ram = ramCheckBox.Checked;
            Settings.Default.Exclude = excludedTextBox.Text;
            Settings.Default.HideTrayIcon = hideTrayCheckBox.Checked;
            Settings.Default.AutoStart = startupCheckBox.Checked;
            RegisterStartup(Settings.Default.AutoStart);
            Settings.Default.MinimizeAll = minimizeAllCheckBox.Checked;
            Settings.Default.Locus = locusEffectCheckBox.Checked;
            Settings.Default.RememberIconPositions = rememberIconPositionsCheckBox.Checked;
            Settings.Default.OpenExplorer = openExplorerCheckBox.Checked;
            Settings.Default.Save();
        }

        void CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        void ExcludedTextBoxLostFocus(object sender, EventArgs e)
        {
            if (excludedTextBox.Text != Settings.Default.Exclude)
                SaveSettings();
        }

        void CheckDrivesTimerTick(object sender, EventArgs e)
        {
            // Additions...
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (DriveIncluded(drive) && drive.IsReady)
                    {
                        var name = GetDriveName(drive);

                        if (shortcuts.Contains(name))
                            continue;

                        var desktopShortcutPath = DesktopShortcutPath(name + DriveInformation(drive));
                        Shell.CreateShortcut(desktopShortcutPath, drive.RootDirectory.FullName);
                        shortcuts.Add(new Shortcut(desktopShortcutPath, drive.RootDirectory.FullName, name));
                        var location = Desktop.WaitForShortcut(name);

                        if (Settings.Default.RememberIconPositions)
                        {
                            location = Desktop.LoadIconPosition(name) ?? location;
                            Desktop.SetIconPosition(name, (Point)location);
                        }

                        if (Settings.Default.MinimizeAll)
                            Shell.MinimizeAll();

                        if (Settings.Default.Locus)
                            Effects.ShowEffect(this, (Point)location);

                        if (Settings.Default.OpenExplorer)
                            NativeMethods.ShellExecute(IntPtr.Zero, "explore", desktopShortcutPath,
                                                       "", "", NativeMethods.ShowCommands.SW_SHOWDEFAULT);

                        SetWorkingSetSize();
                    }
                }

                catch (Exception ex)
                {
                    Program.LogError(ex.Message);
                }
            }

            // Subtractions
            var shortcutsToRemove = new List<Shortcut>();

            foreach (var shortcut in shortcuts)
            {
                if (!Directory.Exists(shortcut.RootDirectoryPath) ||
                    !DriveIncluded(new DriveInfo(shortcut.RootDirectoryPath.Substring(0, 1))))
                {
                    shortcutsToRemove.Add(shortcut);
                    Desktop.SaveIconPosition(shortcut.Name, Desktop.GetIconPosition(shortcut.Name));
                }
            }

            foreach (var shortcut in shortcutsToRemove)
            {
                SafeDelete(shortcut.DesktopShortcutPath);
                shortcuts.Remove(shortcut.Name);
                SetWorkingSetSize();
            }
        }

        static string GetDriveName(DriveInfo drive)
        {
            var name = drive.VolumeLabel;

            if (string.IsNullOrEmpty(name))
            {
                if (drive.Name[1] == Path.VolumeSeparatorChar)
                {
                    var key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\DriveIcons\" +
                              drive.Name[0] + @"\DefaultLabel";

                    name = Registry.LocalMachine.GetValue(key) as string ?? "";
                }

                if (string.IsNullOrEmpty(name))
                    name = "Desk Drive";
            }

            if (drive.Name[1] == Path.VolumeSeparatorChar)
                name += " (" + drive.Name[0] + ")";

            return name;
        }

        bool DriveIncluded(DriveInfo drive)
        {
            if (drive == null)
                return false;

            if (Settings.Default.Exclude.Contains(Path.GetPathRoot(drive.RootDirectory.FullName)))
                return false;

            switch (drive.DriveType)
            {
                case DriveType.CDRom: return CDCheckBox.Checked;
                case DriveType.Removable: return removableCheckBox.Checked;
                case DriveType.Fixed: return fixedCheckBox.Checked;
                case DriveType.Network: return networkedCheckBox.Checked;
                case DriveType.Ram: return ramCheckBox.Checked;
                default: return false;
            }
        }

        static string DesktopShortcutPath(string folder)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), folder + ".lnk");
        }

        static string DriveInformation(DriveInfo drive)
        {
            string information = string.Empty;

            try
            {
                const double gb = 1024 * 1024 * 1024;

                if (drive.DriveType == DriveType.Removable)
                    information = string.Format(" - {0:0.0}GB ({1:0.0})", drive.TotalSize / gb, drive.TotalFreeSpace / gb);
            }

            catch (IOException)
            { 
            }

            return information;
        }

        void DeskDriveFormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var shortcut in shortcuts)
            {
                Desktop.SaveIconPosition(shortcut.Name, Desktop.GetIconPosition(shortcut.Name));
                SafeDelete(shortcut.DesktopShortcutPath);
            }
        }

        void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        void HideButtonClick(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
            SetWorkingSetSize();
        }

        void ShowToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Location.X >= Screen.PrimaryScreen.WorkingArea.Width ||
                Location.Y >= Screen.PrimaryScreen.WorkingArea.Height)
            {
                Location = new Point(200, 200);
            }

            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            Show();
            BringToFront();
        }

        static void SafeDelete(string path)
        {
            try
            {
                File.Delete(path);
            }

            catch (Exception ex)
            {
                Program.LogError(ex.Message);
            }
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == NativeMethods.WM_NOTIFYDD)
            {
                ShowToolStripMenuItemClick(this, EventArgs.Empty);
                BringToFront();
                Activate();
            }

            else if (message.Msg == NativeMethods.WM_DEVICECHANGE)
            {
                var wpar = (uint)message.WParam;

                if (wpar == NativeMethods.DBT_DEVICEARRIVAL || wpar == NativeMethods.DBT_DEVICEREMOVECOMPLETE)
                    CheckDrivesTimerTick(this, EventArgs.Empty);
            }

            base.WndProc(ref message);
        }

        void HideTrayCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            notifyIcon.Visible = !hideTrayCheckBox.Checked;
        }

        static void RegisterStartup(bool register)
        {
            const string subkey = "DeskDriveStartup";
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (key == null)
                return;

            if (register)
                key.SetValue(subkey, Application.ExecutablePath, RegistryValueKind.String);

            else
                key.DeleteValue(subkey, false);
        }

        void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://blueonionsoftware.com/deskdrive.aspx");
        }

        void DonateLinkLabelClick(object sender, EventArgs e)
        {
            Process.Start(
                "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=mike%40blueonionsoftware%2ecom&no_shipping=1&cn=Leave%20a%20note&tax=0&currency_code=USD&lc=US&bn=PP%2dDonationsBF&charset=UTF%2d8");
        }

        void DonateLinkLabelMouseEnter(object sender, EventArgs e)
        {
            donateLinkLabel.Image = Resources.DarkDonateButton;
        }

        void DonateLinkLabelMouseLeave(object sender, EventArgs e)
        {
            donateLinkLabel.Image = Resources.DonateButton;
        }

        static void SetWorkingSetSize()
        {
            var size = new UIntPtr(UInt32.MaxValue);
            NativeMethods.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, size, size);
        }

        void SetWorkingSetSizeTimerTick(object sender, EventArgs e)
        {
            SetWorkingSetSize();
        }
    }
}