'*************************** Module Header ******************************'
' Module Name:  MainForm.vb
' Project:	    VBDetectWindowsSessionState
' Copyright (c) Microsoft Corporation.
' 
' This is the main form of this application. It is used to initialize the UI and 
' handle the events.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports Microsoft.Win32

Partial Public Class MainForm
    Inherits Form
    Private _session As WindowsSession
    Private _timer As System.Threading.Timer

    Public Sub New()
        InitializeComponent()

        ' Initialize the WindowsSession instance.
        _session = New WindowsSession()

        'Initialize the timer, but not start it.
        _timer = New System.Threading.Timer(
            New System.Threading.TimerCallback(AddressOf DetectSessionState),
            Nothing, System.Threading.Timeout.Infinite, 5000)
    End Sub

    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load

        ' Register the StateChanged event.
        AddHandler _session.StateChanged, AddressOf session_StateChanged

    End Sub

    ''' <summary>
    ''' Handle the StateChanged event of WindowsSession.
    ''' </summary>
    Private Sub session_StateChanged(ByVal sender As Object,
                                     ByVal e As SessionSwitchEventArgs)
        ' Display the current state.
        lbState.Text = "Current State: " & e.Reason.ToString()

        ' Record the StateChanged event and add it to the list box.
        lstRecord.Items.Add(String.Format("{0}   {1} " & vbTab & "Occurred",
                                          Date.Now, e.Reason.ToString()))

        lstRecord.SelectedIndex = lstRecord.Items.Count - 1

    End Sub

    Private Sub chkEnableTimer_CheckedChanged(ByVal sender As System.Object,
                                              ByVal e As System.EventArgs) _
                                          Handles chkEnableTimer.CheckedChanged
        If chkEnableTimer.Checked Then
            _timer.Change(0, 5000)
        Else
            _timer.Change(0, System.Threading.Timeout.Infinite)
        End If

    End Sub

    Sub DetectSessionState(ByVal obj As Object)

        ' Check whether the current session is locked.
        Dim isCurrentLocked As Boolean = _session.IsLocked()

        Dim state = If(isCurrentLocked, SessionSwitchReason.SessionLock,
                       SessionSwitchReason.SessionUnlock)

        ' Display the current state.
        lbState.Text = String.Format("Current State: {0}    Time: {1} ",
                                     state, Date.Now)

        ' Record the detected result and add it to the list box.
        lstRecord.Items.Add(String.Format("{0}   {1} ", Date.Now, state))

        lstRecord.SelectedIndex = lstRecord.Items.Count - 1
    End Sub
End Class
