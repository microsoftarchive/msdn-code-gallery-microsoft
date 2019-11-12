' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class DirectoryScanner

    Const MB As Long = 1024 * 1024

    ''' <summary>
    ''' Handles the Load event for the DirectoryScanner.
    ''' </summary>
    Private Sub DirectoryScanner_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set up the initial ListView columns
        ListView1.Columns.Add("Size", 90, HorizontalAlignment.Left)
        ListView1.Columns.Add("Folder Name", 400, HorizontalAlignment.Left)
    End Sub

    ''' <summary>
    ''' This subroutine adds the strSubDirectory that the user selects on the TreeView 
    ''' to the ListView, and sets the text, size, and color.
    ''' </summary>
    Private Sub AddToListView(ByVal strSize As String, ByVal strFolderName As String)
        Dim listViewItem As New ListViewItem()
        Dim listViewSubItem As ListViewItem.ListViewSubItem

        listViewItem.Text = strSize
        listViewItem.ForeColor = GetSizeColor(strSize)

        listViewSubItem = New ListViewItem.ListViewSubItem()
        listViewSubItem.Text = strFolderName

        listViewItem.SubItems.Add(listViewSubItem)
        ListView1.Items.Add(listViewItem)
    End Sub

    ''' <summary>
    ''' This subroutine returns a Color based on the combined size of the directory 
    ''' and all its subdirectories. This is one of two overloads.
    ''' </summary>
    Private Function GetSizeColor(ByVal strSize As String) As System.Drawing.Color
        Return GetSizeColor(CLng(CDbl(strSize.Substring(0, _
            strSize.LastIndexOf("M") - 1)) * MB))
    End Function

    ''' <summary>
    ''' This function returns a Color based on the combined size of the directory 
    ''' and all its subdirectories. This is the second of two overloads.
    ''' </summary>
    Private Function GetSizeColor(ByVal intSize As Long) As System.Drawing.Color
        Select Case intSize
            Case 200 * MB To 500 * MB
                Return System.Drawing.Color.Gold
            Case Is > 500 * MB
                Return System.Drawing.Color.Red
            Case Else
                Return System.Drawing.Color.Green
        End Select
    End Function

    ''' <summary>
    ''' This function returns the size of a directory, and all its sub-directories.
    ''' </summary>
    Public Function GetDirectorySize(ByVal strDirPath As String, _
        ByVal dnDriveOrDirectory As DirectoryNode) As Long

        Try
            Dim astrSubDirectories As String() = Directory.GetDirectories(strDirPath)
            Dim strSubDirectory As String

            ' The size of the current directory is dependent on the size 
            ' of the sub-directories in the array astrSubDirectories. So iterate
            ' through the array and use recursion to end up with the total
            ' size of the current directory and all its sub-directories.
            For Each strSubDirectory In astrSubDirectories
                Dim dnSubDirectoryNode As DirectoryNode
                dnSubDirectoryNode = New DirectoryNode()

                ' Set the node text = to only the last part of the full path.
                dnSubDirectoryNode.Text = _
                    strSubDirectory.Remove(0, strSubDirectory.LastIndexOf("\") + 1)

                ' Note that the following line is recursive.
                dnDriveOrDirectory.Size += _
                    GetDirectorySize(strSubDirectory, dnSubDirectoryNode)
                dnDriveOrDirectory.Nodes.Add(dnSubDirectoryNode)
            Next

            ' Add to the size calcutate above all of the files in the current 
            ' directory.
            Dim astrFiles As String() = Directory.GetFiles(strDirPath)
            Dim strFileName As String
            Dim Size As Long = 0

            For Each strFileName In astrFiles
                dnDriveOrDirectory.Size += New FileInfo(strFileName).Length
            Next

            ' Set the color of the TreeNode based on the total calculated size.
            dnDriveOrDirectory.ForeColor = _
                GetSizeColor(dnDriveOrDirectory.Size)

        Catch exc As Exception
            ' Do nothing. Simply skip any directories that can't be read. Control
            ' passes to the first line after End Try.
        End Try

        ' Return the total size for this directory.
        Return dnDriveOrDirectory.Size

    End Function

    ''' <summary>
    ''' When a directory node is expanded, add its subdirectories to the ListView. 
    ''' </summary>
    Public Sub ShowSubDirectories(ByVal dnDrive As DirectoryNode)
        Dim strSubDirectory As DirectoryNode

        ListView1.Items.Clear()

        For Each strSubDirectory In dnDrive.Nodes
            AddToListView(Format(strSubDirectory.Size / MB, "F") + "MB", _
                strSubDirectory.Text)
        Next
    End Sub

    ''' <summary>
    ''' Close the application
    ''' </summary>
    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub AllDirectoriesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllDirectoriesToolStripMenuItem.Click
        Me.Cursor = Cursors.WaitCursor

        ' Get an array of all logical drives.
        Dim drives As String() = Directory.GetLogicalDrives()
        Dim drive As String

        TreeView1.Nodes.Clear()
        ListView1.Items.Clear()

        For Each drive In drives
            Dim dnDrive As DirectoryNode

            Try
                ' Create a DirectoryNode that represents each logical drive and add
                ' it to the TreeView.
                dnDrive = New DirectoryNode()
                dnDrive.Text = drive.Remove(Len(drive) - 1, 1)
                TreeView1.Nodes.Add(dnDrive)

                ' Calculate the size of the drive by adding up the size of all its
                ' sub-directories.
                dnDrive.Size += GetDirectorySize(drive, dnDrive)
            Catch exc As Exception
                ' Do nothing. Simply skip any directories that can't be read. Control
                ' passes to the first line after End Try.
            End Try
        Next
        Me.Cursor = Cursors.Arrow
    End Sub

    Private Sub FromOneDirectoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FromOneDirectoryToolStripMenuItem.Click
        Me.Cursor = Cursors.WaitCursor
        ' Show the FolderBrowser dialog and set the initial directory to the 
        ' user's selection.
        Dim SelectedDirectory As String = FolderBrowser.ShowDialog()
        Dim SelectedDirectoryNode As DirectoryNode

        TreeView1.Nodes.Clear()
        ListView1.Items.Clear()

        Try
            ' Add the DirectoryNode that represents the selected directory to the
            ' TreeView.
            SelectedDirectoryNode = New DirectoryNode()
            SelectedDirectoryNode.Text = SelectedDirectory
            TreeView1.Nodes.Add(SelectedDirectoryNode)

            ' Calculate the size of the selected directory by adding up the size of 
            ' all its sub-directories.
            SelectedDirectoryNode.Size += GetDirectorySize(SelectedDirectory, _
                SelectedDirectoryNode)

        Catch exc As Exception
            ' Do nothing. Simply skip any directories that can't be read. Control
            ' passes to the first line after End Try.
        End Try
        Me.Cursor = Cursors.Arrow
    End Sub

    ''' <summary>
    ''' Handles the AfterExpand event for the TreeView, which does not occur after 
    ''' the TreeView is selected, but after the application decides that the user's 
    ''' attempt to expand the node should be allowed. The corresponding BeforeExpand 
    ''' event handler is used for this decision making, if desired. All Before
    ''' events pass a TreeViewCancelEventArgs object that contains a Cancel property.
    ''' This property can be used for vetoing the user's action. Thus, the "AfterExpand"
    ''' event could rightly be named "AfterExpandApproval".
    ''' </summary>
    Private Sub TreeView1_AfterExpand(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterExpand
        e.Node.Expand()
        ShowSubDirectories(CType(e.Node, DirectoryNode))
    End Sub

    ''' <summary>
    ''' Handles the AfterSelect event for the TreeView, which does not occur after 
    ''' the TreeView is selected, but after the application decides that the user's 
    ''' attempt to select the node should be allowed. The corresponding BeforeSelect 
    ''' event handler is used for this decision making, if desired. All Before
    ''' events pass a TreeViewCancelEventArgs object that contains a Cancel property.
    ''' This property can be used for vetoing the user's action. Thus, the "AfterSelect"
    ''' event could rightly be named "AfterSelectApproval".
    ''' </summary>
    Private Sub TreeView1_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterSelect
        Dim SubDirectory As DirectoryNode = CType(e.Node, DirectoryNode)
        ListView1.Items.Clear()
        AddToListView(Format(SubDirectory.Size / (1024 * 1024), "F") + "MB", _
            SubDirectory.Text)
    End Sub
End Class