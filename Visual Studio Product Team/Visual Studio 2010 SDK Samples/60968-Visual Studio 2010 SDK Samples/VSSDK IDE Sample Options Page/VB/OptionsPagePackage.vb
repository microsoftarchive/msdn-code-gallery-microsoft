'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage

	''' <summary>
	''' This class implements Visual studio package that is registered within Visual Studio IDE.
	''' The VSPackage class uses a number of registration attribute to specify integration parameters.
	''' Implementation of IVsUserSettings within the package allows to persist a VSPackage configuration
	''' </summary>
	''' <remarks>
	''' <para>A description of the different attributes used here is given below:</para>
	''' <para>ProvideOptionPageAttribute: This registers a given class for persisting part 
	''' or all of VSPackage's state through the Visual Studio settings mechanism. 
	''' </para>
	''' <para>ProvideProfileAttribute: This registers that a specific independent class implements 
	''' a specific instance of Visual Studio settings functionality for the VSPackage, 
	''' allowing it to save and retrieve VSPackage state information.
	''' </para>
    ''' </remarks>
    ' Allows to persist a VSPackage configuration
    <PackageRegistration(UseManagedResourcesOnly:=True), ProvideOptionPageAttribute(GetType(OptionsPageGeneral), "My Options Page (VisualBasic)", "General", 101, 106, True), ProvideProfileAttribute(GetType(OptionsPageGeneral), "My Options Page (VisualBasic)", "General Options", 101, 106, True, DescriptionResourceID:=101), ProvideOptionPageAttribute(GetType(OptionsPageCustom), "My Options Page (VisualBasic)", "Custom", 101, 107, True), InstalledProductRegistration("My Options Page (VisualBasic)", "My Options Page (VisualBasic) Sample", "1.0"), Guid(GuidStrings.GuidPackage)> _
    Public Class OptionsPagePackageVB
        Inherits Package
        Implements IVsUserSettings
#Region "Methods"
        ''' <summary>
        ''' Initialization of the package.
        ''' </summary>
        Protected Overrides Sub Initialize()
            Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", Me.ToString()))
            MyBase.Initialize()
        End Sub

#Region "IVsUserSettings"
        ''' <summary>
        ''' This method is left for the reader to implement if it is needed.
        ''' </summary>
        ''' <returns>S_OK if method is succeeded.</returns>
        Public Function ExportSettings(ByVal pszCategoryGUID As String, ByVal pSettings As IVsSettingsWriter) As Integer
            Return VSConstants.S_OK
        End Function
        ''' <summary>
        ''' This method is left for the reader to implement if it is needed.
        ''' </summary>
        ''' <returns>S_OK if method is succeeded.</returns>
        Public Function ImportSettings(ByVal pszCategoryGUID As String, ByVal pSettings As IVsSettingsReader, ByVal flags As UInteger, ByRef pfRestartRequired As Integer) As Integer
            Return VSConstants.S_OK
        End Function
#End Region

#End Region
    End Class
End Namespace
