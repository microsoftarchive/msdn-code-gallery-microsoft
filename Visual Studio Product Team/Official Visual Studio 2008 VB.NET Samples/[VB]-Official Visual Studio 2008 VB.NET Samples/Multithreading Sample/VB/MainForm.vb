' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Threading

Public Class MainForm

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load
        ' Append the ID of the current thread
        ThreadID.Text &= CStr(Thread.CurrentThread.ManagedThreadId)

    End Sub

    Private Sub theLongRunningTask()
        Dim progressForm As New TaskProgress()
        progressForm.Show()
        ' Refresh causes an instant (non-posted) display of the label.
        progressForm.Refresh()

        ' Slowly increment the progress bar.
        For i As Integer = 1 To 10
            progressForm.ProgressIndicator.Value += 10
            ' Half-second delay
            System.Threading.Thread.Sleep(500)
        Next

        ' Remove the form after the "task" finishes.
        progressForm.Hide()
        progressForm.Dispose()

    End Sub

    Private Sub SameThread_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SameThread.Click
        ' Run the task on the same thread that is managing the frmMain window.
        theLongRunningTask()
    End Sub

    ' To run the task on a worker pool thread, you can use an asynchronous
    ' invocation on a delegate. For this example, we'll declare a delegate
    ' named TaskDelegate, and call it asynchronously. The signature of the
    ' delegate declaration must match the method (TheLongRunningTask) exactly.
    Delegate Sub TaskDelegate()

    Private Sub ThreadPool_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ThreadPool.Click
        ' To run the task an a thread from the worker pool, create an instance
        ' of a delegate whose signature matches the method that will be called,
        ' then call BeginInvoke on that delegate. For this example, the arguments
        ' and return value of BeginInvoke are unneeded. This technique is sometimes
        ' referred to as "Fire and Forget".

        Dim td As New TaskDelegate(AddressOf theLongRunningTask)
        ' Runs on a worker thread from the pool.
        td.BeginInvoke(Nothing, Nothing)
    End Sub

    Private Sub BackgroundWorker_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BackgroundWorker.Click
        ' Run the task on a thread from the worker pool using the new BackgroundWorker
        ' component that will automatically handle marshalling results back to the main
        ' UI thread via the RunWorkerCompleted event
        Worker.RunWorkerAsync()
    End Sub

    Private Sub Worker_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles Worker.DoWork
        ' This event is fired on the background thread and where we place the
        ' call(s) to our long running task
        theLongRunningTask()
    End Sub

    Private Sub exitToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
