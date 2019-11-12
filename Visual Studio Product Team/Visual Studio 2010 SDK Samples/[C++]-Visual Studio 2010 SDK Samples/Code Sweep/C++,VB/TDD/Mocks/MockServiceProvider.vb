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
Imports EnvDTE
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.Shell
Imports System.ComponentModel.Design
Imports System.Diagnostics

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockServiceProvider
        Implements IServiceProvider
        Private ReadOnly _taskList As New MockTaskList()
        Private ReadOnly _uiShell As New MockShell()
        Private ReadOnly _statusBar As New MockStatusBar()
        Private ReadOnly _dte As MockDTE
        Private ReadOnly _solution As New MockSolution()
        Private ReadOnly _rdt As New MockRDT()
        Private ReadOnly _uiShellOpenDoc As New MockUIShellOpenDocument()
        Private ReadOnly _textMgr As New MockTextManager()
        Private ReadOnly _webBrowser As New MockWebBrowsingService()
        Private ReadOnly _menuService As OleMenuCommandService

        Public Sub New()
            _menuService = New OleMenuCommandService(Me)
            _dte = New MockDTE(Me)
        End Sub

        Public ReadOnly Property TaskList() As MockTaskList
            Get
                Return _taskList
            End Get
        End Property

#Region "IServiceProvider Members"

        Public Function GetService(ByVal serviceType As Type) As Object Implements IServiceProvider.GetService
            If GetType(SVsTaskList).IsEquivalentTo(serviceType) Then
                Return _taskList
            ElseIf GetType(SVsUIShell).IsEquivalentTo(serviceType) Then
                Return _uiShell
            ElseIf GetType(SVsStatusbar).IsEquivalentTo(serviceType) Then
                Return _statusBar
            ElseIf GetType(DTE).IsEquivalentTo(serviceType) Then
                Return _dte
            ElseIf GetType(SVsSolution).IsEquivalentTo(serviceType) Then
                Return _solution
            ElseIf GetType(SVsRunningDocumentTable).IsEquivalentTo(serviceType) Then
                Return _rdt
            ElseIf GetType(SVsUIShellOpenDocument).IsEquivalentTo(serviceType) Then
                Return _uiShellOpenDoc
            ElseIf GetType(SVsTextManager).IsEquivalentTo(serviceType) Then
                Return _textMgr
            ElseIf GetType(SVsWebBrowsingService).IsEquivalentTo(serviceType) Then
                Return _webBrowser
            ElseIf GetType(IMenuCommandService).IsEquivalentTo(serviceType) Then
                Return _menuService
            ElseIf GetType(ISelectionService).IsEquivalentTo(serviceType) Then
                Return Nothing
            ElseIf GetType(IDesignerHost).IsEquivalentTo(serviceType) Then
                Return Nothing
            Else
                Debug.Fail("Service " & CType(CType(serviceType, Object), String) & " not found.")
                Return Nothing
            End If
        End Function

#End Region
    End Class
End Namespace
