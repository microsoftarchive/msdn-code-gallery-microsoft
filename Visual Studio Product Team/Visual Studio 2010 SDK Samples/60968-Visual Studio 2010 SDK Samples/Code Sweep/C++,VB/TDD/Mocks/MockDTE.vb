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
Imports System.Collections.Generic
Imports System.Text

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockDTE
        Implements EnvDTE.DTE
        Private ReadOnly _events As New MockEvents()
        Private ReadOnly _solution As MockDTESolution
        Private ReadOnly _serviceProvider As IServiceProvider

        Public Sub New(ByVal serviceProvider As IServiceProvider)
            _serviceProvider = serviceProvider
            _solution = New MockDTESolution(_serviceProvider)
        End Sub

#Region "_DTE Members"

        Public ReadOnly Property ActiveDocument() As EnvDTE.Document Implements EnvDTE._DTE.ActiveDocument
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ActiveSolutionProjects() As Object Implements EnvDTE._DTE.ActiveSolutionProjects
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ActiveWindow() As EnvDTE.Window Implements EnvDTE._DTE.ActiveWindow
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property AddIns() As EnvDTE.AddIns Implements EnvDTE._DTE.AddIns
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Application() As EnvDTE.DTE Implements EnvDTE._DTE.Application
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property CommandBars() As Object Implements EnvDTE._DTE.CommandBars
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property CommandLineArguments() As String Implements EnvDTE._DTE.CommandLineArguments
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Commands() As EnvDTE.Commands Implements EnvDTE._DTE.Commands
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ContextAttributes() As EnvDTE.ContextAttributes Implements EnvDTE._DTE.ContextAttributes
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property DTE() As EnvDTE.DTE Implements EnvDTE._DTE.DTE
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Debugger() As EnvDTE.Debugger Implements EnvDTE._DTE.Debugger
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Property DisplayMode() As EnvDTE.vsDisplay Implements EnvDTE._DTE.DisplayMode
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As EnvDTE.vsDisplay)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property Documents() As EnvDTE.Documents Implements EnvDTE._DTE.Documents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Edition() As String Implements EnvDTE._DTE.Edition
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Events() As EnvDTE.Events Implements EnvDTE._DTE.Events
            Get
                Return _events
            End Get
        End Property

        Public Sub ExecuteCommand(ByVal CommandName As String, Optional ByVal CommandArgs As String = "") Implements EnvDTE._DTE.ExecuteCommand
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public ReadOnly Property FileName() As String Implements EnvDTE._DTE.FileName
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Find() As EnvDTE.Find Implements EnvDTE._DTE.Find
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property FullName() As String Implements EnvDTE._DTE.FullName
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function GetObject(ByVal Name As String) As Object Implements EnvDTE._DTE.GetObject
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property Globals() As EnvDTE.Globals Implements EnvDTE._DTE.Globals
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ItemOperations() As EnvDTE.ItemOperations Implements EnvDTE._DTE.ItemOperations
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function LaunchWizard(ByVal VSZFile As String, ByRef ContextParams As Object()) As EnvDTE.wizardResult Implements EnvDTE._DTE.LaunchWizard
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property LocaleID() As Integer Implements EnvDTE._DTE.LocaleID
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Macros() As EnvDTE.Macros Implements EnvDTE._DTE.Macros
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property MacrosIDE() As EnvDTE.DTE Implements EnvDTE._DTE.MacrosIDE
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property MainWindow() As EnvDTE.Window Implements EnvDTE._DTE.MainWindow
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Mode() As EnvDTE.vsIDEMode Implements EnvDTE._DTE.Mode
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Name() As String Implements EnvDTE._DTE.Name
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ObjectExtenders() As EnvDTE.ObjectExtenders Implements EnvDTE._DTE.ObjectExtenders
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function OpenFile(ByVal ViewKind As String, ByVal FileName As String) As EnvDTE.Window Implements EnvDTE._DTE.OpenFile
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Sub Quit() Implements EnvDTE._DTE.Quit
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public ReadOnly Property RegistryRoot() As String Implements EnvDTE._DTE.RegistryRoot
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function SatelliteDllPath(ByVal Path As String, ByVal Name As String) As String Implements EnvDTE._DTE.SatelliteDllPath
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property SelectedItems() As EnvDTE.SelectedItems Implements EnvDTE._DTE.SelectedItems
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Solution() As EnvDTE.Solution Implements EnvDTE._DTE.Solution
            Get
                Return _solution
            End Get
        End Property

        Public ReadOnly Property SourceControl() As EnvDTE.SourceControl Implements EnvDTE._DTE.SourceControl
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property StatusBar() As EnvDTE.StatusBar Implements EnvDTE._DTE.StatusBar
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Property SuppressUI() As Boolean Implements EnvDTE._DTE.SuppressUI
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As Boolean)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property UndoContext() As EnvDTE.UndoContext Implements EnvDTE._DTE.UndoContext
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Property UserControl() As Boolean Implements EnvDTE._DTE.UserControl
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As Boolean)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property Version() As String Implements EnvDTE._DTE.Version
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property WindowConfigurations() As EnvDTE.WindowConfigurations Implements EnvDTE._DTE.WindowConfigurations
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Windows() As EnvDTE.Windows Implements EnvDTE._DTE.Windows
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property IsOpenFile(ByVal ViewKind As String, ByVal FileName As String) As Boolean Implements EnvDTE._DTE.IsOpenFile
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Properties(ByVal Category As String, ByVal Page As String) As EnvDTE.Properties Implements EnvDTE._DTE.Properties
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

#End Region
    End Class
End Namespace
