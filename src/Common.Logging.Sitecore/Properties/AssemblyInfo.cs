using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Common.Logging.Sitecore")]
[assembly: AssemblyDescription("Sitecore.Logging.dll hasn't changed since Sitecore 6.6.0 rev 120918, it wraps a very old version of log4net. This adapter allows you to use Common.Logging with Sitecore.Logging. You may have a Sitecore agnostic library that is used in a Sitecore solution. If that library implements the Common.Logging ILog for all logging functionality, then you will be able to patch into Sitecore's logging using this adapter. That means that logging from your Sitecore agnostic assembly will go to the Sitecore log as configured in the web.congfig.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Alex Tselevich")]
[assembly: AssemblyProduct("Common.Logging.Sitecore")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f877c446-2e3b-41a2-9f0b-701f99eb868e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.5")]
[assembly: AssemblyFileVersion("1.0.0.5")]
