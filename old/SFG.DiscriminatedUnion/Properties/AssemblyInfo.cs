using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BrightSword.CSharpExtensions.DiscriminatedUnion;
using Microsoft.VisualStudio;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Discriminated Unions For C#")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("John Azariah")]
[assembly: AssemblyProduct("DiscriminatedUnions")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: ProvideAssemblyObject(typeof(DiscriminatedUnionFileGenerator))]
[assembly: ProvideGenerator(
    typeof(DiscriminatedUnionFileGenerator),
    VSConstants.UICONTEXT.CSharpProject_string,
    Description = "Description of the generator",
    GeneratesDesignTimeSource = true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f2b38b50-b95d-46ae-b197-f37e51ef6cc2")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
