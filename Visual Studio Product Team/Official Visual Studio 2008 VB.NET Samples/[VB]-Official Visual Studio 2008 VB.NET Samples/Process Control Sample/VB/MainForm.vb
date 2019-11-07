' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Private Sub CurrentProcessInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCurrentProcessInfo.Click
        ' Shows how to retrieve information about the current Process.
        Dim curProc As Process = Process.GetCurrentProcess()

        Dim description As String = "The total working set of the current process is: " + _
                curProc.WorkingSet64.ToString() + vbCrLf

        description += "The minimum working set of the current process is: " + _
                curProc.MinWorkingSet.ToString() + vbCrLf

        description += "The max working set of the current process is: " + _
                curProc.MaxWorkingSet.ToString() + vbCrLf

        description += "The start time of the current process is: " + _
                curProc.StartTime.ToLongTimeString() + vbCrLf

        description += "The processor time used by the current process is: " + _
        curProc.TotalProcessorTime.ToString() + vbCrLf

        DisplayText.Text = description
    End Sub

    Private Sub btnStartProcess_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartProcess.Click
        ' Simple Demonstration of starting a process using the process class.
        Process.Start("notepad.exe")
    End Sub

    Private Sub btnProcessStartInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcessStartInfo.Click
        ' The StartInfo object allows you to pass additional parameters to your application 
        ' before starting it.  In this case the default window state of the application is set.
        Dim startInfo As New ProcessStartInfo("notepad.exe")
        startInfo.WindowStyle = ProcessWindowStyle.Maximized
        Process.Start(startInfo)
    End Sub

    Private Sub btnTaskManager_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTaskManager.Click
        ' Using the process class you can get access to additional information such as the 
        ' modules loaded by a process.  The form shown by this code illustrates this.
        Dim manager As New TaskManager()
        manager.Show()
    End Sub

    Private Sub btnShellExecute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShellExecute.Click

        If Not System.IO.File.Exists("c:\demofile_shell.txt") Then
            Dim sw As New System.IO.StreamWriter("c:\demofile_shell.txt")
            sw.WriteLine("Shell Execute Demo")
            sw.Close()
        End If

        ' The StartInfo class can also be used to specify that you wish Operating System Shell 
        ' to execute the process.  'This means that you can pass file names with extensions that
        ' are known by the operating system and the operating system will launch the appropriate
        ' application type.
        Dim startInfo As New ProcessStartInfo("c:\demofile_shell.txt")
        ' The default for UseShellExecute is false. If this is not set an exception would be 
        ' thrown when the start method is executed.
        startInfo.UseShellExecute = True
        Process.Start(startInfo)

    End Sub

    Private Sub btnCommandLine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCommandLine.Click
        ' Starts Windows Explorer with two different command line arguments.
        Dim startInfo As New ProcessStartInfo("explorer.exe")
        ' Opens a new single-pane Window for the default selection. 
        ' This is usually the root of the drive on which Windows is installed. 
        startInfo.Arguments = "/n"
        Process.Start(startInfo)
    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
