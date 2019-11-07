' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class TaskManager

    ' List of processes currently running
    Dim processList As New System.Collections.Generic.List(Of ProcessInfo)


    Private Sub btnModules_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnModules.Click
        ' Get the item that is currently selected in the combo box.  Then all of the modules
        ' for that process are displayed to the user via the richTextBox on frmModules.
        Dim processID As Integer = (CType(cboCurrentProcesses.SelectedItem, ProcessInfo)).ID
        Dim modulesForm As New ModulesDisplay
        modulesForm.ProcessID = processID
        modulesForm.Show()
    End Sub

    Private Sub TaskManager_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            ' Loop through all the running process, and add them to the ComboBox
            ' so that the user can select a process and see the information
            ' for that process.
            For Each oneProcess As Process In Process.GetProcesses()
                ' Devenv is the Visual Studio Development Environment.  You will see one entry 
                ' for each instance of the development environment that you have open. IEXPLORE is
                ' the Internet Explorer.
                If oneProcess.ProcessName = "IEXPLORE" Or oneProcess.ProcessName = "devenv" Then
                    processList.Add(New ProcessInfo(oneProcess.Id, oneProcess.ProcessName))
                End If
            Next
            cboCurrentProcesses.DataSource = processList
            With cboCurrentProcesses
                .DataSource = processList
                .SelectedIndex = 0
            End With

            If btnModules.Enabled = False Then
                btnModules.Enabled = True
            End If
        Catch exc As Exception
            MsgBox("Unable to load process names: " & vbCrLf & "Choose another process.", MsgBoxStyle.OKOnly, Me.Text)
            btnModules.Enabled = False
        End Try
    End Sub

    Private Sub cboCurrentProcesses_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCurrentProcesses.SelectedIndexChanged
        Dim processID As Integer

        Try

            ' Retrieve the information for the process based on the item that the
            ' user has selected in the combo box.
            'Dim ProcessListIndex As Integer = cboCurrentProcesses.SelectedIndex
            processID = (CType(cboCurrentProcesses.SelectedItem, ProcessInfo)).ID
            Dim ProcessInfo As Process = _
                Process.GetProcessById(processID)

            'Information is displayed about the currently selected process.
            txtPriority.Text = ProcessInfo.BasePriority.ToString()
            txtNumberOfThreads.Text = ProcessInfo.Threads.Count.ToString()
            txtMaxWorkingSet.Text = ProcessInfo.MaxWorkingSet.ToString()
            txtMinWorkingSet.Text = ProcessInfo.MinWorkingSet.ToString()
            txtStartTime.Text = ProcessInfo.StartTime.ToLongTimeString()
            txtTotalProcessorTime.Text = ProcessInfo.TotalProcessorTime.ToString()
        Catch exc As Exception
            MsgBox("Unable to retrieve information for ProcessID : " & processID & ".", MsgBoxStyle.OKOnly)
        End Try

    End Sub

    Private Class ProcessInfo
        Public ID As Integer
        Public Name As String
        Public Sub New(ByVal newId As Integer, ByVal newName As String)
            ID = newId
            Name = newName
        End Sub
        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

End Class