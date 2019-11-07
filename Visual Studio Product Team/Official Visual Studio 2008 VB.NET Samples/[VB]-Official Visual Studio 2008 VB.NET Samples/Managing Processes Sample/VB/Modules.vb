' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Modules
    Private parentValue As Process
    Private moduleNames As New System.Collections.Generic.SortedList(Of String, ProcessModule)

    Friend Property ParentProcess() As Process
        Get
            Return parentValue
        End Get
        Set(ByVal Value As Process)
            parentValue = Value
            If parentValue Is Nothing Then
                moduleNames = Nothing
            End If
        End Set
    End Property

    Private Sub EnumModules()
        Try
            Me.lvModules.Items.Clear()
            If Not moduleNames Is Nothing Then
                moduleNames = New System.Collections.Generic.SortedList(Of String, ProcessModule)
            End If
            Dim m As ProcessModule
            For Each m In parentValue.Modules
                Me.lvModules.Items.Add(m.ModuleName)

                Try
                    moduleNames.Add(m.ModuleName, m)
                Catch exp As ArgumentException
                    ' This means the item was duplicated.
                    ' Eat error and continue.
                Catch exp As Exception
                    MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            Next
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Friend Sub RefreshModules()
        Me.ToolStripStatusLabel1.Text = "Process = " & parentValue.ProcessName
        Me.lvModDetails.Items.Clear()
        EnumModules()
    End Sub

    Private Sub EnumModule(ByVal m As ProcessModule)

        Me.lvModDetails.Items.Clear()
        Dim mits As ListView.ListViewItemCollection = Me.lvModDetails.Items

        Try
            AddNameValuePair("Base Address", Hex(m.BaseAddress.ToInt32).ToLower(), mits)
            AddNameValuePair("Entry Point Address", Hex(m.EntryPointAddress.ToInt32).ToLower(), mits)
            AddNameValuePair("File Name", m.FileName, mits)
            AddNameValuePair("File Version", m.FileVersionInfo.FileVersion.ToString(), mits)
            AddNameValuePair("File Description", m.FileVersionInfo.FileDescription, mits)
            AddNameValuePair("Memory Size", m.ModuleMemorySize.ToString("N0"), mits)

        Catch exp As Exception
            MsgBox(exp.Message, MsgBoxStyle.Critical, exp.Source)
        End Try
    End Sub

    Private Sub AddNameValuePair(ByVal Item As String, ByVal SubItem As String, ByVal mits As ListView.ListViewItemCollection)
        With mits.Add(Item)
            .SubItems.Add(SubItem)
        End With
    End Sub

    Private Sub lvModules_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvModules.SelectedIndexChanged
        Try
            Dim lv As ListView = CType(sender, ListView)

            If lv.SelectedItems.Count = 1 Then
                Dim strMod As String = lv.SelectedItems(0).Text

                Dim m As ProcessModule = CType(moduleNames.Item(strMod), ProcessModule)
                EnumModule(m)
            End If
        Catch exp As Exception
            MsgBox(exp.Message, MsgBoxStyle.Critical, exp.Source)
        End Try
    End Sub



    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        Me.Close()
    End Sub
End Class