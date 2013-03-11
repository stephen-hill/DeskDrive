// Copyright 2006 Blue Onion Software
// All rights reserved

namespace BlueOnion
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                LogInformation("Program started");

                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                {
                    const int flags = (int)NativeMethods.MessageBroadcastFlags.BSF_POSTMESSAGE;
                    var recipients = (int)NativeMethods.MessageBroadcastRecipients.BSM_APPLICATIONS;
                    NativeMethods.BroadcastSystemMessage(flags, ref recipients, NativeMethods.WM_NOTIFYDD, 0, 0);
                    LogInformation("Second instance detected");
                    return;
                }

                UpgradeSettings();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Microsoft.Win32.SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                Application.Run(new DeskDrive());
                Desktop.SaveIconPositions();
            }

            catch (Exception ex)
            {
                LogError("Unhandled exception: " + ex);
                throw;
            }

            finally
            {
                LogInformation("Program terminated");
            }
        }

        static void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            Microsoft.Win32.SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
            Desktop.SaveIconPositions();
        }

        static void UpgradeSettings()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version.ToString();

            if (version != Properties.Settings.Default.ApplicationVersion)
            {
                Properties.Settings.Default.Upgrade();
                var oldVersion = Properties.Settings.Default.ApplicationVersion;
                Properties.Settings.Default.ApplicationVersion = version;
                Properties.Settings.Default.Save();
                LogInformation("Settings upgraded: " + oldVersion + " -> " + version);
            }
        }

        public static void LogError(string message)
        {
            Log(message, EventLogEntryType.Error);
        }

        public static void LogInformation(string message)
        {
            Log(message, EventLogEntryType.Information);
        }

        static void Log(string message, EventLogEntryType logEntryType)
        {
            try
            {
                EventLog.WriteEntry(InstallEventLog.EventSource, message, logEntryType);
            }

            catch (Exception)
            {
                // in case the event source is not registered
            }
        }
    }
}