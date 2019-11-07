' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm



    ' Collection to hold processes for faster retrieval
    Private processes As System.Collections.Generic.SortedList(Of String, Process)
    ' Child form reference to show module detail
    Private modulesForm As Modules

    ' String constants for display in listviews
    Private Const NAProcess As String = "N/A"
    Private Const TotalProcess As String = "_Total (0x0)"
    Private Const IdleProcess As String = "Idle"
    Private Const SystemProcess As String = "System"

    ' Used by AddNameValuePair to reduce typing
    Private mits As ListView.ListViewItemCollection


    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load
        ' Load the list of processes
        EnumProcesses()
    End Sub

    Private Sub lvProcesses_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvProcesses.SelectedIndexChanged
        Try
            Dim lv As ListView = lvProcesses

            If lv.SelectedItems.Count = 1 Then
                ' Get the process id from the first subitem column
                Dim processId As String = lv.SelectedItems(0).SubItems(1).Text

                ' Check to see if we got our fake 'total' process
                If processId = NAProcess Then
                    Me.lvProcessDetail.Items.Clear()
                    Me.lvThreads.Items.Clear()
                    Exit Sub
                End If

                Dim p As Process

                p = CType(processes.Item(processId), Process)
                ' Get the most current data
                p.Refresh()

                ' Get the process detail
                EnumProcess(p)
                ' Get the thread detail
                EnumThreads(p)
            End If
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try
    End Sub

    Private Sub mnuModules_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ModulesToolStripMenuItem.Click, ModuleContextMenuItem.Click
        ' Load the child form to display module information
        Try
            Dim lv As ListView = Me.lvProcesses

            If lv.SelectedItems.Count = 1 Then

                Dim strProcessId As String = lv.SelectedItems(0).SubItems(1).Text
                ' Check to see if we got our fake 'total' process
                If strProcessId = NAProcess Then
                    Exit Sub
                End If

                Dim p As Process

                p = CType(processes.Item(strProcessId), Process)

                ' Don't enumerate the idle process.
                ' You will receive an access denied error.
                If p.ProcessName = IdleProcess Then
                    Exit Sub
                End If
                ' Nothing to show
                If p.ProcessName = SystemProcess Then
                    Exit Sub
                End If
                p.Refresh()

                ' Finally check to see if we can even 
                ' Get the module count.
                ' If not, no point in going further.
                Try
                    Dim i As Integer = p.Modules.Count
                Catch exp As System.ComponentModel.Win32Exception
                    MessageBox.Show("Sorry, you are not authorized to read this information.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End Try
                ' If the form is not available, load it
                If modulesForm Is Nothing Then
                    modulesForm = New Modules
                End If

                ' Pass the selected process
                modulesForm.ParentProcess = p
                ' Get the module data
                modulesForm.RefreshModules()
                ' Show the form
                modulesForm.ShowDialog(Me)
            End If
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshToolStripMenuItem.Click, RefreshContextMenuItem.Click
        ' Refresh the process list
        Me.StatusStrip1.Text = "Refreshing list, please wait"
        Me.StatusStrip1.Refresh()
        EnumProcesses()
        Me.StatusStrip1.Text = "Ready"
    End Sub

    Private Sub AddNameValuePair(ByVal Item As String, ByVal SubItem As String)
        ' Helper procedure to add name/value pairs to a listview control
        With mits.Add(Item)
            .SubItems.Add(SubItem)
        End With
    End Sub

    Private Sub EnumThreads(ByVal p As Process)
        ' Get the thread information for the process.
        ' This information is about the physical Win32 threads
        ' not System.Threading.Thread threads.
        Try
            Me.lvThreads.Items.Clear()

            Dim strProcessId As String = ""

            If strProcessId = NAProcess Then
                Me.lvThreads.Items.Add(NAProcess)
            Else
                Dim t As ProcessThread

                ' Timespans for individual thread times
                Dim tpt As TimeSpan
                Dim tppt As TimeSpan
                Dim tupt As TimeSpan

                For Each t In p.Threads
                    ' Get thread time and store
                    tppt = t.PrivilegedProcessorTime
                    tupt = t.UserProcessorTime
                    tpt = t.TotalProcessorTime

                    ' % User Processor Time for thread
                    Dim strPUPT As String = CDbl(tupt.Ticks / tpt.Ticks).ToString("#0%")
                    If tupt.Ticks = 0 Then
                        strPUPT = "0%"
                    End If

                    ' % Privileged Processor Time for thread
                    Dim strPPPT As String = CDbl(tppt.Ticks / tpt.Ticks).ToString("#0%")
                    If tppt.Ticks = 0 Then
                        strPPPT = "0%"
                    End If

                    Dim strTPT As String
                    With tpt
                        strTPT = (.Days.ToString("00") & "." & .Hours.ToString("00") & ":" & .Minutes.ToString("00") & ":" & .Seconds.ToString("00"))
                    End With

                    With Me.lvThreads.Items.Add(t.Id.ToString())
                        .SubItems.Add(t.BasePriority.ToString())
                        .SubItems.Add(t.CurrentPriority.ToString())
                        Try
                            .SubItems.Add(t.PriorityBoostEnabled.ToString())
                        Catch exp As System.ComponentModel.Win32Exception
                            .SubItems.Add("N/A")
                        End Try
                        Try
                            .SubItems.Add(t.PriorityLevel.ToString())
                        Catch exp As System.ComponentModel.Win32Exception
                            .SubItems.Add("N/A")
                        End Try

                        .SubItems.Add(strPPPT)
                        .SubItems.Add(Hex(t.StartAddress.ToInt32()).ToLower())
                        .SubItems.Add(t.StartTime.ToShortDateString() & " " & t.StartTime.ToShortTimeString())
                        .SubItems.Add(strTPT)
                        .SubItems.Add(strPUPT)
                    End With
                Next
            End If
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub EnumProcess(ByVal p As Process)
        ' Get process information
        Dim lv As ListView = Me.lvProcessDetail
        lv.Items.Clear()

        If p.ProcessName = IdleProcess Then
            Exit Sub
        End If

        mits = lv.Items

        Try
            Dim valuePairs()() As Object = { _
                New Object() {"Start Time", p.StartTime.ToLongDateString() & " " & p.StartTime.ToLongTimeString(), ""}, _
                New Object() {"Responding", p.Responding, ""}, _
                New Object() {"Handle", p.Handle, ""}, _
                New Object() {"Handle Count", p.HandleCount, "N0"}, _
                New Object() {"Main Window Handle", p.MainWindowHandle, ""}, _
                New Object() {"Main Window Title", p.MainWindowTitle, ""}, _
                New Object() {"Module Count", p.Modules.Count, "N0"}, _
                New Object() {"Base Priority", p.BasePriority, ""}, _
                New Object() {"Working Set", p.WorkingSet64, "N0"}, _
                New Object() {"Peak Working Set", p.PeakWorkingSet64, "N0"}, _
                New Object() {"Private Memory Size", p.PrivateMemorySize64, "N0"}, _
                New Object() {"Nonpaged System Memory Size", p.NonpagedSystemMemorySize64, "N0"}, _
                New Object() {"Paged Memory Size", p.PagedMemorySize64, "N0"}, _
                New Object() {"Peak Paged Memory Size", p.PeakPagedMemorySize64, "N0"}, _
                New Object() {"Virtual Memory Size", p.VirtualMemorySize64, "N0"}, _
                New Object() {"Peak Virtual Memory Size", p.PeakVirtualMemorySize64, "N0"}, _
                New Object() {"Priority Boost Enabled", p.PriorityBoostEnabled, ""}, _
                New Object() {"Priority Class", p.PriorityClass, ""}, _
                New Object() {"Processor Affinity", p.ProcessorAffinity.ToInt32, ""}, _
                New Object() {"Thread Count", p.Threads.Count, ""}, _
                New Object() {"Min Working Set", p.MinWorkingSet.ToInt32, "N0"}, _
                New Object() {"Max Working Set", p.MaxWorkingSet.ToInt32, "N0"}, _
                New Object() {"Main Module", IIf(p.MainModule Is Nothing, "", p.MainModule.ModuleName), ""}}

            Me.AddPairs(valuePairs)
        Catch ex As Exception
            MsgBox("One or more of the process properities is not available: " & vbCrLf & ex.Message, _
                MsgBoxStyle.Critical, ex.Source)
        End Try

    End Sub

    Private Sub AddPairs(ByVal pairs()() As Object)
        Const NA As String = "Not Available"

        Dim nPairs As Integer = pairs.GetLength(0)
        For index As Integer = 0 To nPairs - 1
            Dim pair() As Object = pairs(index)
            ' first item is the title
            Dim newItem As ListViewItem = mits.Add(pair(0).ToString)
            ' second is the value, and third is the format string
            ' If there is a formatting string, the value is either a Long or an Integer.
            If CType(pair(2), String) <> "" Then
                Try
                    Dim format As String = CStr(pair(2))
                    Dim value As Long = CType(pair(1), Long)
                    Dim contents As String = value.ToString(format)
                    newItem.SubItems.Add(contents)
                Catch ex As Exception
                    newItem.SubItems.Add(NA)
                End Try
            Else
                Try
                    newItem.SubItems.Add(pair(1).ToString)
                Catch ex As Exception
                    newItem.SubItems.Add(NA)
                End Try
            End If
        Next
    End Sub


    Private Sub EnumProcesses()
        ' Enumerate all processes
        Try
            Dim processList() As Process

            ' Timespans for individual process information
            Dim total As TimeSpan
            Dim privileged As TimeSpan
            Dim user As TimeSpan

            ' Timespans for machine
            Dim totalTotal As TimeSpan
            Dim privilegedTotal As TimeSpan
            Dim userTotal As TimeSpan

            processes = New System.Collections.Generic.SortedList(Of String, Process)

            Me.lvProcesses.Items.Clear()
            Me.lvProcessDetail.Items.Clear()
            Me.lvThreads.Items.Clear()

            processList = Process.GetProcesses()

            For Each p As Process In processList
                Try
                    processes.Add(p.Id.ToString, p)

                    ' Get processor time and store
                    privileged = p.PrivilegedProcessorTime
                    user = p.UserProcessorTime
                    total = p.TotalProcessorTime

                    ' Add the current process’ times to total times.
                    totalTotal = totalTotal.Add(total)
                    privilegedTotal = privilegedTotal.Add(privileged)
                    userTotal = userTotal.Add(user)

                    ' % User Processor Time
                    Dim userPercent As String = CDbl(user.Ticks / total.Ticks).ToString("#0%")
                    ' % Privileged Processor Time
                    Dim privilegedPercent As String = CDbl(privileged.Ticks / total.Ticks).ToString("#0%")

                    Dim totalString As String = FormatTimeSpan(total)

                    Dim processItem As ListViewItem = _
                        Me.lvProcesses.Items.Add(p.ProcessName & " (0x" & Hex(p.Id).ToLower() & ")")
                    With processItem
                        .SubItems.Add(p.Id.ToString())
                        .SubItems.Add(totalString)
                        .SubItems.Add(privilegedPercent)
                        .SubItems.Add(userPercent)
                    End With
                Catch ex As Exception
                    ' We'll just ignore any processes that we can't enumerate (i.e., we cannot access)
                End Try
            Next

            ' % Total User Processor Time
            Dim stringUserPercent As String = CDbl(userTotal.Ticks / totalTotal.Ticks).ToString("#0%")
            ' % Total Privileged Processor Time
            Dim stringPrivilegedPercent As String = CDbl(privilegedTotal.Ticks / totalTotal.Ticks).ToString("#0%")

            Dim stringTotalTotal As String = FormatTimeSpan(totalTotal)

            ' Add entry for all processes
            With Me.lvProcesses.Items.Add(TotalProcess)
                .SubItems.Add(NAProcess)
                .SubItems.Add(stringTotalTotal)
                .SubItems.Add(stringPrivilegedPercent)
                .SubItems.Add(stringUserPercent)
            End With
        Catch exp As Exception
            MsgBox(exp.Message, MsgBoxStyle.Critical, exp.Source)
        End Try
    End Sub


    Private Function FormatTimeSpan(ByVal interval As TimeSpan) As String
        Return _
            interval.Days.ToString("00") & "." & _
            interval.Hours.ToString("00") & ":" & _
            interval.Minutes.ToString("00") & ":" & _
            interval.Seconds.ToString("00")
    End Function

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class