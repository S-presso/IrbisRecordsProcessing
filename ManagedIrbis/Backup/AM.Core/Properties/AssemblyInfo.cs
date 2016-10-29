/* AssemblyInfo.cs -- assembly info
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

#endregion

[assembly: CLSCompliant ( true )]
[assembly: ReliabilityContract ( Consistency.WillNotCorruptState, Cer.None )]

//[assembly: SuppressMessage ( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly" ) ]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle ( "AMCore.dll" )]
[assembly: AssemblyDescription ( "Ars Magna core services" )]
//[assembly : AssemblyConfiguration ( "" )]

[assembly: AssemblyCompany ( "Ars Magna Project" )]
[assembly: AssemblyProduct ( "Ars Magna" )]
[assembly: AssemblyCopyright("Copyright © Ars Magna Project 2005-2014")]
//[assembly : AssemblyTrademark ( "" )]
//[assembly : AssemblyCulture ( "" )]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM componenets.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible ( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid ( "7967861d-0530-42be-9a6e-a9422b195f41" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion ( "1.0.0.0" )]
[assembly: AssemblyFileVersion ( "1.0.0.0" )]
