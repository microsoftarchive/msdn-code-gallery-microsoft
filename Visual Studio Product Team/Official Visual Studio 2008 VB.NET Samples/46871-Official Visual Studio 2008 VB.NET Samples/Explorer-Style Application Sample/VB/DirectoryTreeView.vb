' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Class DirectoryTreeView
    Inherits TreeView

    ' This is the Class constructor.
    Public Sub New()
        ' Make a little more room for long directory names.
        Me.Width *= 2

        ' Get images for tree.
        Me.ImageList = New ImageList()
        With Me.ImageList.Images
            .Add(My.Resources.FLOPPY)
            .Add(My.Resources.CLSDFOLD)
            .Add(My.Resources.OPENFOLD)
        End With

        ' Construct tree.
        RefreshTree()
    End Sub

    ' Handles the BeforeExpand event for the subclassed TreeView. See further 
    ' comments about the Before_____ and After_______ TreeView event pairs in 
    ' /DirectoryScanner/DirectoryScanner.vb.
    Protected Overrides Sub OnBeforeExpand(ByVal tvcea As TreeViewCancelEventArgs)
        MyBase.OnBeforeExpand(tvcea)

        ' For performance reasons and to avoid TreeView "flickering" during an 
        ' large node update, it is best to wrap the update code in BeginUpdate...
        ' EndUpdate statements.
        Me.BeginUpdate()

        Dim tn As TreeNode
        ' Add child nodes for each child node in the node clicked by the user. 
        ' For performance reasons each node in the DirectoryTreeView 
        ' contains only the next level of child nodes in order to display the + sign 
        ' to indicate whether the user can expand the node. So when the user expands
        ' a node, in order for the + sign to be appropriately displayed for the next
        ' level of child nodes, *their* child nodes have to be added.
        For Each tn In tvcea.Node.Nodes
            AddDirectories(tn)
        Next tn

        Me.EndUpdate()
    End Sub

    ' This subroutine is used to add a child node for every directory under its
    ' parent node, which is passed as an argument. See further comments in the
    ' OnBeforeExpand event handler.
    Sub AddDirectories(ByVal tn As TreeNode)
        tn.Nodes.Clear()

        Dim strPath As String = tn.FullPath
        Dim diDirectory As New DirectoryInfo(strPath)
        Dim adiDirectories() As DirectoryInfo

        Try
            ' Get an array of all sub-directories as DirectoryInfo objects.
            adiDirectories = diDirectory.GetDirectories()
        Catch exp As Exception
            Exit Sub
        End Try

        Dim di As DirectoryInfo
        For Each di In adiDirectories
            ' Create a child node for every sub-directory, passing in the directory
            ' name and the images its node will use.
            Dim tnDir As New TreeNode(di.Name, 1, 2)
            ' Add the new child node to the parent node.
            tn.Nodes.Add(tnDir)

            ' We could now fill up the whole tree by recursively calling 
            ' AddDirectories():
            '
            '   AddDirectories(tnDir)
            '
            ' This is way too slow, however. Give it a try!
        Next
    End Sub

    ' This subroutine clears out the existing TreeNode objects and rebuilds the 
    ' DirectoryTreeView, showing the logical drives.
    Public Sub RefreshTree()

        ' For performance reasons and to avoid TreeView "flickering" during an 
        ' large node update, it is best to wrap the update code in BeginUpdate...
        ' EndUpdate statements.
        BeginUpdate()

        Nodes.Clear()

        ' Make disk drives the root nodes. 
        Dim astrDrives As String() = Directory.GetLogicalDrives()

        Dim strDrive As String
        For Each strDrive In astrDrives
            Dim tnDrive As New TreeNode(strDrive, 0, 0)
            Nodes.Add(tnDrive)
            AddDirectories(tnDrive)

            ' Set the C drive as the default selection.
            If strDrive = "C:\" Then
                Me.SelectedNode = tnDrive
            End If
        Next

        EndUpdate()
    End Sub
End Class
