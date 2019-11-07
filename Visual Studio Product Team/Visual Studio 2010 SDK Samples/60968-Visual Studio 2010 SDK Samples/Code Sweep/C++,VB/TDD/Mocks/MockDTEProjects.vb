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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockDTEProjects
        Implements EnvDTE.Projects
        Private ReadOnly _serviceProvider As IServiceProvider
        Private ReadOnly _projects As New Dictionary(Of String, MockDTEProject)()

        Public Sub New(ByVal serviceProvider As IServiceProvider)
            _serviceProvider = serviceProvider
        End Sub

#Region "Projects Members"

        Public ReadOnly Property Count() As Integer Implements EnvDTE.Projects.Count
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property DTE() As EnvDTE.DTE Implements EnvDTE.Projects.DTE
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements EnvDTE.Projects.GetEnumerator, System.Collections.IEnumerable.GetEnumerator
            Dim solution As MockSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), MockSolution)
            Dim results As New List(Of Object)
            For Each project As MockIVsProject In solution.Projects
                If (Not _projects.ContainsKey(project.FullPath)) Then
                    _projects.Add(project.FullPath, New MockDTEProject(project))
                End If
                results.Add(_projects(project.FullPath))
            Next project
            Return results.GetEnumerator()
        End Function

        Public Function Item(ByVal index As Object) As EnvDTE.Project Implements EnvDTE.Projects.Item
            Return Utilities.ListFromEnum(_projects.Values)(CInt(Fix(index)))
        End Function

        Public ReadOnly Property Kind() As String Implements EnvDTE.Projects.Kind
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Parent() As EnvDTE.DTE Implements EnvDTE.Projects.Parent
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Properties() As EnvDTE.Properties Implements EnvDTE.Projects.Properties
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

#End Region
    End Class
End Namespace
