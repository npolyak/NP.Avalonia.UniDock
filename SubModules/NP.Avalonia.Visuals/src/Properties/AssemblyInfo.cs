// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using Avalonia.Metadata;
using System.Reflection;
using System.Runtime.InteropServices;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: AssemblyDescription("Visual Controls, Utilities and Behaviors for AvaloniaUI")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("0a922f17-66b8-4240-b3ed-ed9fda3d819f")]

[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals")]
[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals.Converters")]
[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals.Controls")]
[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals.Behaviors")]
[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals.MarkupExtensions")]
[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals.ThemingAndL10N")]
[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals.ColorUtils")]
[assembly: XmlnsDefinition("https://np.com/visuals", "NP.Avalonia.Visuals.MarkupExtensions")]