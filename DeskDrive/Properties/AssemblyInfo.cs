// Copyright (c) 2009 Blue Onion Software
// All rights reserved

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Permissions;

[assembly: AssemblyTitle("Desk Drive")]
[assembly: AssemblyDescription("Automatic drive/media shortcuts on your desktop")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Blue Onion Software")]
[assembly: AssemblyProduct("DeskDrive")]
[assembly: AssemblyCopyright("Copyright © Blue Onion Software 2009")]
[assembly: AssemblyTrademark("Desk Drive")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: Guid("855c8a64-bc33-46ba-9dfe-345eabdc9725")]

[assembly: AssemblyVersion("1.8.0.0")]
[assembly: AssemblyFileVersion("1.8.0.0")]

[assembly: IsolatedStorageFilePermission(SecurityAction.RequestMinimum, UserQuota = 1048576)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]
[assembly: FileIOPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
[assembly: UIPermission(SecurityAction.RequestMinimum, Unrestricted = true)]

[assembly: NeutralResourcesLanguageAttribute("en")]
