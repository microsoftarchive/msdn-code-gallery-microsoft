'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualStudio.Shell
Imports System
Imports System.Drawing.Design
Imports System.Reflection
Imports System.Runtime.InteropServices

''' <summary>
''' This is the class that implements the package exposed by this assembly.
'''
''' The minimum requirement for a class to be considered a valid package for Visual Studio
''' is to implement the IVsPackage interface and register itself with the shell.
''' This package uses the helper classes defined inside the Managed Package Framework (MPF)
''' to do it: it derives from the Package class that provides the implementation of the 
''' IVsPackage interface and uses the registration attributes defined in the framework to 
''' register itself and its components with the shell.
''' </summary>

' The PackageRegistration attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class
' is a package.
'
' The InstalledProductRegistration attribute is used to register the information needed to show this package
' in the Help/About dialog of Visual Studio.

<PackageRegistration(UseManagedResourcesOnly:=True), _
InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400), _
Guid(GuidList.guidWinformsControlsInstallerPkgString), _
ProvideToolboxItems(WinformsControlsInstallerPackage.ToolboxVersion)>
Public NotInheritable Class WinformsControlsInstallerPackage
    Inherits Package

    ' This value, passed to the constructor of ProvideToolboxItemsAttribute to generate
    ' toolbox registration for the package, must be >= 1.  Increment it if your toolbox
    ' content changes (for example, you have new items to install).  After the updated
    ' version of your package is installed, the toolbox will notice the updated value and
    ' invoke your ToolboxUpgraded event to allow you to update your content.
    Public Const ToolboxVersion As Integer = 1

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Overridden Package Implementation
#Region "Package Members"

    ''' <summary>
    ''' Initialization of the package; this method is called right after the package is sited, so this is the place
    ''' where you can put all the initialization code that rely on services provided by VisualStudio.
    ''' </summary>
    Protected Overrides Sub Initialize()
        MyBase.Initialize()
        ' TODO: add initialization code here
    End Sub
#End Region

    ''' <summary>
    ''' This method is called when the toolbox content version (the parameter to the ProvideToolboxItems
    ''' attribute) changes.  This tells Visual Studio that items may have changed 
    ''' and need to be reinstalled.
    ''' </summary>
    Sub WinformsControlsInstaller_ToolboxUpgraded(ByVal sender As Object, ByVal e As EventArgs) Handles Me.ToolboxUpgraded
        RemoveToolboxItems()
        InstallToolboxItems()
    End Sub

    ''' <summary>
    ''' This method will add items to the toolbox.  It is called the first time the toolbox
    ''' is used after this package has been installed.
    ''' </summary>
    Sub WinformsControlsInstaller_ToolboxInitialized(ByVal sender As Object, ByVal e As EventArgs) Handles Me.ToolboxInitialized
        InstallToolboxItems()
    End Sub

    ''' <summary>
    ''' Removes all the toolbox items installed by this package (those which came from this assembly).
    ''' </summary>
    Sub RemoveToolboxItems()
        Dim a As Assembly = GetType(WinformsControlsInstallerPackage).Assembly

        Dim tbxService As IToolboxService = DirectCast(GetService(GetType(IToolboxService)), IToolboxService)

        For Each item As ToolboxItem In ToolboxService.GetToolboxItems(a, newCodeBase:=Nothing)
            tbxService.RemoveToolboxItem(item)
        Next item
    End Sub

    ''' <summary>
    ''' Installs all the toolbox items defined in this assembly.
    ''' </summary>
    Sub InstallToolboxItems()
        ' For demonstration purposes, this assembly includes toolbox items and loads them from itself.
        ' It is of course possible to load toolbox items from a different assembly by either:
        ' a)  loading the assembly yourself and calling ToolboxService.GetToolboxItems
        ' b)  call AssemblyName.GetAssemblyName("...") and then ToolboxService.GetToolboxItems(assemblyName)
        Dim a As Assembly = GetType(WinformsControlsInstallerPackage).Assembly

        Dim tbxService As IToolboxService = DirectCast(GetService(GetType(IToolboxService)), IToolboxService)

        For Each item As ToolboxItem In ToolboxService.GetToolboxItems(a, newCodeBase:=Nothing)
            ' This tab name can be whatever you would like it to be.
            tbxService.AddToolboxItem(item, "MyOwnTab")
        Next
    End Sub
End Class
