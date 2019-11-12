' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Threading

Public Class MainForm

    ' Used to track which thread called the UI Update function.
    Private callingThread As Integer

    ' Initialize the form.
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Update the UI to show the current thread that the UI is running on.
        callingThread = System.Threading.Thread.CurrentThread.ManagedThreadId()
        Label2.Text = "The UI Thread Number is: " + callingThread.ToString()
        Label1.Text = ""
    End Sub

    ' Functions to update the UI from a new thread.
#Region "New Thread Functions"

    Private Sub ThreadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ThreadButton.Click
        ' Starts a new thread to call UpdateUI from.
        Dim newThread As New Thread(AddressOf UpdateUI)
        newThread.Start()
    End Sub

#End Region

    ' Functions to update the UI using the UI thread.
#Region "UI Update Functions"

    Private Sub UIButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UIButton.Click
        ' Update the UI from the UI thread.
        callingThread = System.Threading.Thread.CurrentThread.ManagedThreadId()
        UpdateUI()
    End Sub

    ' The Delegate that is invoked by the control on the form that needs to be updated.
    Delegate Sub UIDelegate()

    Private Sub UpdateUI()
        ' If InvokeRequired is true then the call is being made on a thread other
        ' than the UI thread.
        If Label1.InvokeRequired Then
            ' Call UpdateUI by invoking a delegate with the UI control
            callingThread = System.Threading.Thread.CurrentThread.ManagedThreadId()
            Dim newDelegate As New UIDelegate(AddressOf UpdateUI)
            Label1.Invoke(newDelegate)
        Else
            Label1.Text = "The update function used thread " + callingThread.ToString()
        End If
    End Sub

#End Region

    ' Functions to update the UI using a threading timer. 
#Region "Timer Functions"
    Private Sub StartTimer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartTimer.Click

        Dim timerThread As New Thread(AddressOf RunTimer)
        timerThread.Start()

    End Sub

    Class TimerObjClass
        Public Count As Integer
        Public TimerReference As System.Threading.Timer
        Public TimerCanceled As Boolean
    End Class

    Sub RunTimer()
        Dim StateObj As New TimerObjClass()
        StateObj.TimerCanceled = False
        StateObj.Count = 1
        Dim TimerDelegate As New Threading.TimerCallback(AddressOf TimerTask)

        Dim TimerItem As New System.Threading.Timer(TimerDelegate, StateObj, _
                                                    2000, 2000)
        StateObj.TimerReference = TimerItem
        While StateObj.Count < 5
            System.Threading.Thread.Sleep(2000)
        End While

        StateObj.TimerCanceled = True
    End Sub

    Sub TimerTask(ByVal StateObj As Object)
        Dim State As TimerObjClass = CType(StateObj, TimerObjClass)
        State.Count += 1
        callingThread = System.Threading.Thread.CurrentThread.ManagedThreadId()

        UpdateUI()

        If State.TimerCanceled Then
            State.TimerReference.Dispose()
        End If
    End Sub
#End Region

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
