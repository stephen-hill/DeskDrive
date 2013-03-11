// Copyright (c) 2008 Blue Onion Software
// All rights reserved

namespace BlueOnion
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;

    [RunInstaller(true)]
    public class InstallEventLog : Installer
    {
        public const string EventSource = "Desk Drive";

        public InstallEventLog()
        {
            var eventLogInstaller = new EventLogInstaller {Source = EventSource};
            Installers.Add(eventLogInstaller);
        }
    }
}
