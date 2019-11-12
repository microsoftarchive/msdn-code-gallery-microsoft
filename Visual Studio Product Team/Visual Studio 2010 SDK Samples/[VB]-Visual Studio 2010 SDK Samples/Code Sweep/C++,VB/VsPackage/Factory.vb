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
Imports Microsoft.VisualStudio.Shell.Interop
Imports System.Runtime.InteropServices
Imports Microsoft.Samples.VisualStudio.CodeSweep.BuildTask

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class Factory
        Private Shared _serviceProvider As IServiceProvider

        Private Sub New()
        End Sub
        Public Shared Property ServiceProvider() As IServiceProvider
            Set(ByVal value As IServiceProvider)
                _serviceProvider = value
                ProjectUtilities.SetServiceProvider(_serviceProvider)
            End Set

            Get
                Return _serviceProvider
            End Get
        End Property

        Public Shared Function GetDialog() As IConfigurationDialog
            Dim dialog As New ConfigDialog()
            dialog.ServiceProvider = _serviceProvider
            Return dialog
        End Function

        Private Shared _projectStores As New Dictionary(Of IVsProject, IProjectConfigurationStore)()

        Public Shared Function GetProjectConfigurationStore(ByVal project As IVsProject) As IProjectConfigurationStore

            If _projectStores.ContainsKey(project) Then
                Return _projectStores(project)
            Else
                Dim store As IProjectConfigurationStore

                If ProjectUtilities.IsMSBuildProject(project) Then
                    store = New ProjectConfigStore(project)
                Else
                    store = New NonMSBuildProjectConfigStore(project, _serviceProvider)
                End If

                _projectStores.Add(project, store)
                Return store
            End If
        End Function

        Private Shared _host As IScannerHost

        Public Shared Function GetScannerHost() As IScannerHost
            If _host Is Nothing Then
                _host = New ScannerHost(_serviceProvider)
            End If
            Return _host
        End Function

        Private Shared _taskProvider As ITaskProvider

        Public Shared Function GetTaskProvider() As ITaskProvider
            If _taskProvider Is Nothing Then
                _taskProvider = New TaskProvider(_serviceProvider)
            End If
            Return _taskProvider
        End Function

        Private Shared _buildManager As IBuildManager

        Public Shared Function GetBuildManager() As IBuildManager
            If _buildManager Is Nothing Then
                _buildManager = New BuildManager(_serviceProvider)
            End If
            Return _buildManager
        End Function

        Private Shared _backgroundScanner As IBackgroundScanner

        Public Shared Function GetBackgroundScanner() As IBackgroundScanner
            If _backgroundScanner Is Nothing Then
                _backgroundScanner = New BackgroundScanner(_serviceProvider)
            End If
            Return _backgroundScanner
        End Function

        Public Shared Sub CleanupFactory()
            _backgroundScanner = Nothing
            _buildManager = Nothing
            _taskProvider = Nothing
            _host = Nothing
            _projectStores.Clear()
        End Sub
    End Class
End Namespace
