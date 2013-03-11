// Copyright (c) 2008 Blue Onion Software
// All rights reserved

namespace BlueOnion
{
    using System.Collections.ObjectModel;

    class Shortcut
    {
        public string DesktopShortcutPath { get; set; }
        public string RootDirectoryPath { get; set; }
        public string Name { get; set; }

        public Shortcut(string desktopShortcutPath, string rootDirectoryPath, string name)
        {
            Throw.IfNullOrEmpty(desktopShortcutPath, "desktopShortcutPath");
            Throw.IfNullOrEmpty(rootDirectoryPath, "rootDirectoryPath");
            Throw.IfNullOrEmpty(name, "name");

            DesktopShortcutPath = desktopShortcutPath;
            RootDirectoryPath = rootDirectoryPath;
            Name = name;
        }
    }

    class ShortcutCollection : KeyedCollection<string, Shortcut>
    {
        protected override string GetKeyForItem(Shortcut item)
        {
            return item.Name;
        }
    }
}
