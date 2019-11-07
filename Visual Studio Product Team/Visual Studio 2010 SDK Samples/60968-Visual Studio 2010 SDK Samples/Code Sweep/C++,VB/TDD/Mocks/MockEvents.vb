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
    Friend Class MockEvents
        Implements EnvDTE.Events
        Private ReadOnly _buildEvents As New MockBuildEvents()

#Region "Events Members"

        Public ReadOnly Property BuildEvents() As EnvDTE.BuildEvents Implements EnvDTE.Events.BuildEvents
            Get
                Return _buildEvents
            End Get
        End Property

        Public ReadOnly Property DTEEvents() As EnvDTE.DTEEvents Implements EnvDTE.Events.DTEEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property DebuggerEvents() As EnvDTE.DebuggerEvents Implements EnvDTE.Events.DebuggerEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property FindEvents() As EnvDTE.FindEvents Implements EnvDTE.Events.FindEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public Function GetObject(ByVal Name As String) As Object Implements EnvDTE.Events.GetObject
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public ReadOnly Property MiscFilesEvents() As EnvDTE.ProjectItemsEvents Implements EnvDTE.Events.MiscFilesEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property SelectionEvents() As EnvDTE.SelectionEvents Implements EnvDTE.Events.SelectionEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property SolutionEvents() As EnvDTE.SolutionEvents Implements EnvDTE.Events.SolutionEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property SolutionItemsEvents() As EnvDTE.ProjectItemsEvents Implements EnvDTE.Events.SolutionItemsEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property CommandBarEvents(ByVal CommandBarControl As Object) As Object Implements EnvDTE.Events.CommandBarEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property CommandEvents(Optional ByVal Guid As String = "{00000000-0000-0000-0000-000000000000}", Optional ByVal ID As Integer = 0) As EnvDTE.CommandEvents Implements EnvDTE.Events.CommandEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property
        Public ReadOnly Property DocumentEvents(Optional ByVal Document As EnvDTE.Document = Nothing) As EnvDTE.DocumentEvents Implements EnvDTE.Events.DocumentEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property OutputWindowEvents(Optional ByVal Pane As String = "") As EnvDTE.OutputWindowEvents Implements EnvDTE.Events.OutputWindowEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property
        Public ReadOnly Property TaskListEvents(Optional ByVal Filter As String = "") As EnvDTE.TaskListEvents Implements EnvDTE.Events.TaskListEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property
        Public ReadOnly Property TextEditorEvents(Optional ByVal TextDocumentFilter As EnvDTE.TextDocument = Nothing) As EnvDTE.TextEditorEvents Implements EnvDTE.Events.TextEditorEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property
        Public ReadOnly Property WindowEvents(Optional ByVal WindowFilter As EnvDTE.Window = Nothing) As EnvDTE.WindowEvents Implements EnvDTE.Events.WindowEvents
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property



#End Region
    End Class
End Namespace
