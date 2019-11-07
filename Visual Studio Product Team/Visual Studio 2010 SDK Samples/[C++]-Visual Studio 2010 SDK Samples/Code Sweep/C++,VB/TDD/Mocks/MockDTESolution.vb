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
    Friend Class MockDTESolution
        Implements EnvDTE.Solution
        Private ReadOnly _serviceProvider As IServiceProvider
        Private ReadOnly _projects As MockDTEProjects

        Public Sub New(ByVal serviceProvider As IServiceProvider)
            _serviceProvider = serviceProvider
            _projects = New MockDTEProjects(_serviceProvider)
        End Sub

#Region "_Solution Members"

        Public Function AddFromFile(ByVal FileName As String, Optional ByVal Exclusive As Boolean = False) As EnvDTE.Project Implements EnvDTE._Solution.AddFromFile
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function AddFromTemplate(ByVal FileName As String, ByVal Destination As String, ByVal ProjectName As String, Optional ByVal Exclusive As Boolean = False) As EnvDTE.Project Implements EnvDTE._Solution.AddFromTemplate
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property AddIns() As EnvDTE.AddIns Implements EnvDTE._Solution.AddIns
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Sub Close(Optional ByVal SaveFirst As Boolean = False) Implements EnvDTE._Solution.Close
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public ReadOnly Property Count() As Integer Implements EnvDTE._Solution.Count
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Sub Create(ByVal Destination As String, ByVal Name As String) Implements EnvDTE._Solution.Create
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public ReadOnly Property DTE() As EnvDTE.DTE Implements EnvDTE._Solution.DTE
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ExtenderCATID() As String Implements EnvDTE._Solution.ExtenderCATID
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ExtenderNames() As Object Implements EnvDTE._Solution.ExtenderNames
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property FileName() As String Implements EnvDTE._Solution.FileName
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function FindProjectItem(ByVal FileName As String) As EnvDTE.ProjectItem Implements EnvDTE._Solution.FindProjectItem
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property FullName() As String Implements EnvDTE._Solution.FullName
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements EnvDTE._Solution.GetEnumerator, System.Collections.IEnumerable.GetEnumerator
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property Globals() As EnvDTE.Globals Implements EnvDTE._Solution.Globals
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Property IsDirty() As Boolean Implements EnvDTE._Solution.IsDirty
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As Boolean)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property IsOpen() As Boolean Implements EnvDTE._Solution.IsOpen
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function Item(ByVal index As Object) As EnvDTE.Project Implements EnvDTE._Solution.Item
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Sub Open(ByVal FileName As String) Implements EnvDTE._Solution.Open
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public ReadOnly Property Parent() As EnvDTE.DTE Implements EnvDTE._Solution.Parent
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function ProjectItemsTemplatePath(ByVal ProjectKind As String) As String Implements EnvDTE._Solution.ProjectItemsTemplatePath
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property Projects() As EnvDTE.Projects Implements EnvDTE._Solution.Projects
            Get
                Return _projects
            End Get
        End Property

        Public ReadOnly Property Properties() As EnvDTE.Properties Implements EnvDTE._Solution.Properties
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Sub Remove(ByVal proj As EnvDTE.Project) Implements EnvDTE._Solution.Remove
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public Sub SaveAs(ByVal FileName As String) Implements EnvDTE._Solution.SaveAs
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public Property Saved() As Boolean Implements EnvDTE._Solution.Saved
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As Boolean)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property SolutionBuild() As EnvDTE.SolutionBuild Implements EnvDTE._Solution.SolutionBuild
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Extender(ByVal ExtenderName As String) As Object Implements EnvDTE._Solution.Extender
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get

        End Property

        Public ReadOnly Property TemplatePath(ByVal ProjectType As String) As String Implements EnvDTE._Solution.TemplatePath
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

#End Region
    End Class
End Namespace
