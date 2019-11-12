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
    Friend Class MockDTEProject
        Implements EnvDTE.Project
        Private ReadOnly _project As MockIVsProject
        Private ReadOnly _globals As New MockDTEGlobals()

        Public Sub New(ByVal project As MockIVsProject)
            _project = project
        End Sub

#Region "Project Members"

        Public ReadOnly Property CodeModel() As EnvDTE.CodeModel Implements EnvDTE.Project.CodeModel
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Collection() As EnvDTE.Projects Implements EnvDTE.Project.Collection
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ConfigurationManager() As EnvDTE.ConfigurationManager Implements EnvDTE.Project.ConfigurationManager
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property DTE() As EnvDTE.DTE Implements EnvDTE.Project.DTE
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Sub Delete() Implements EnvDTE.Project.Delete
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public ReadOnly Property ExtenderCATID() As String Implements EnvDTE.Project.ExtenderCATID
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ExtenderNames() As Object Implements EnvDTE.Project.ExtenderNames
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property FileName() As String Implements EnvDTE.Project.FileName
            Get
                Return _project.FullPath
            End Get
        End Property

        Public ReadOnly Property FullName() As String Implements EnvDTE.Project.FullName
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Globals() As EnvDTE.Globals Implements EnvDTE.Project.Globals
            Get
                Return _globals
            End Get
        End Property

        Public Property IsDirty() As Boolean Implements EnvDTE.Project.IsDirty
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As Boolean)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property Kind() As String Implements EnvDTE.Project.Kind
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Property Name() As String Implements EnvDTE.Project.Name
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As String)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property [Object]() As Object Implements EnvDTE.Project.Object
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ParentProjectItem() As EnvDTE.ProjectItem Implements EnvDTE.Project.ParentProjectItem
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property ProjectItems() As EnvDTE.ProjectItems Implements EnvDTE.Project.ProjectItems
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Properties() As EnvDTE.Properties Implements EnvDTE.Project.Properties
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Sub Save(Optional ByVal FileName As String = "") Implements EnvDTE.Project.Save
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public Sub SaveAs(ByVal NewFileName As String) Implements EnvDTE.Project.SaveAs
            Throw New Exception("The method or operation is not implemented.")
        End Sub

        Public Property Saved() As Boolean Implements EnvDTE.Project.Saved
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
            Set(ByVal value As Boolean)
                Throw New Exception("The method or operation is not implemented.")
            End Set
        End Property

        Public ReadOnly Property UniqueName() As String Implements EnvDTE.Project.UniqueName
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Extender(ByVal ExtenderName As String) As Object Implements EnvDTE.Project.Extender
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

#End Region
    End Class
End Namespace
