Public Class MainForm

    ''' <summary>
    ''' Display the correct panel based on the node that was selected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TreeView1_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterSelect
        Select Case e.Node.Text
            Case "Copy a File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(CopyFilePanel.GetInstance())
            Case "Delete a File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(DeleteFilePanel.GetInstance())
            Case "Move a File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(MoveFilePanel.GetInstance())
            Case "Copy a Directory"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(CopyDirectoryPanel.GetInstance())
            Case "Delete a Directory"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(DeleteDirectoryPanel.GetInstance())
            Case "Move a Directory"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(MoveDirectoryPanel.GetInstance())
            Case "Read a Text File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(ReadFilePanel.GetInstance())
            Case "Read a Large Text File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(ReadLargeFilePanel.GetInstance())
            Case "Write a Text File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(WriteFilePanel.GetInstance())
            Case "Write a Large Text File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(WriteLargeFilePanel.GetInstance())
            Case "Parse a File"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(ParseTextFilePanel.GetInstance())
            Case "Find Files"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(FindFilesPanel.GetInstance())
            Case "Find Directories"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(FindDirectoriesPanel.GetInstance())
            Case "Search File Contents For Specific Text"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(FindInFilesPanel.GetInstance())
            Case "View File Properties"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(FileInfoPanel.GetInstance())
            Case "View Directory Properties"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(DirectoryInfoPanel.GetInstance())
            Case "View Drive Properties"
                Me.SplitContainer.Panel2.Controls.Clear()
                Me.SplitContainer.Panel2.Controls.Add(DriveInfoPanel.GetInstance())
        End Select
    End Sub

    ''' <summary>
    ''' Start with the tree fully expanded
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.TreeView1.ExpandAll()
    End Sub
End Class
