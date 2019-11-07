'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage

    ''' <summary>
    ''' This class implements a Visual studio package that is registered for the Visual Studio IDE.
    ''' The package class uses a number of registration attribute to specify integration parameters.
    ''' </summary>
    <
    PackageRegistration(UseManagedResourcesOnly:=True),
    ProvideOptionPageAttribute(GetType(OptionsPageGeneral), "My Options Page (VisualBasic)", "General", 100, 101, True, {"Change sample general options (VB)"}),
    ProvideProfileAttribute(GetType(OptionsPageGeneral), "My Options Page (VisualBasic)", "General Options", 100, 101, True, DescriptionResourceID:=100),
    ProvideOptionPageAttribute(GetType(OptionsPageCustom), "My Options Page (VisualBasic)", "Custom", 100, 102, True, {"Change sample custom options (VB)"}),
    InstalledProductRegistration("My Options Page (VisualBasic)", "My Options Page (VisualBasic) Sample", "1.0"), Guid(GuidStrings.GuidPackage)
    >
    Public Class OptionsPagePackageVB
        Inherits Package

        ''' <summary>
        ''' Initialization of the package.  This is where you should put all initialization
        ''' code that depends on VS services.
        ''' </summary>
        Protected Overrides Sub Initialize()
            MyBase.Initialize()
            ' TODO: add initialization code here
        End Sub
    End Class
End Namespace
