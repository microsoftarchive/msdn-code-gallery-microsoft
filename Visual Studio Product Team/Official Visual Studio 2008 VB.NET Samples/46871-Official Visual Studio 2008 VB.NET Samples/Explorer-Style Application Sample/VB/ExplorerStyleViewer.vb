' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class ExplorerStyleViewer
    ' Declare variables to hold instances of each of the custom classes
    Private dtvwDirectory As DirectoryTreeView
    Private flvFiles As FileListView
    Private mivChecked As MenuItemView

    ' Handles the AfterSelect event for the DirectoryTreeView, which causes the
    ' FileListView object to display the contents of the selected directory.
    Sub DirectoryTreeViewOnAfterSelect(ByVal obj As Object, ByVal tvea As TreeViewEventArgs)
        flvFiles.ShowFiles(tvea.Node.FullPath)
    End Sub

    ' This subroutine handles the Form Load event.
    Private Sub ExplorerStyleViewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Create a flvFilesView instance.
        flvFiles = New FileListView()
        SplitContainer1.Panel2.Controls.Add(flvFiles)
        flvFiles.Dock = DockStyle.Fill

        ' Create a DirectoryTreeView instance and add an OnAfterSelect event handler.
        dtvwDirectory = New DirectoryTreeView()
        'dtvwDirectory.Parent = Me
        SplitContainer1.Panel1.Controls.Add(dtvwDirectory)
        dtvwDirectory.Dock = DockStyle.Left
        ' Dynamically add an AfterSelect event handler.
        AddHandler dtvwDirectory.AfterSelect, _
            AddressOf DirectoryTreeViewOnAfterSelect

        ' Add a View menu command to the existing main menu.
        Dim menuView As New ToolStripMenuItem("&View")
        MenuStrip1.Items.Add(menuView)

        ' Add four menu items to the new View menu. Start by creating arrays to set
        ' properties of each menu item.
        Dim astrView As String() = {"Lar&ge Icons", "S&mall Icons", "&List", "&Details"}
        Dim aview As View() = {View.LargeIcon, View.SmallIcon, View.List, View.Details}
        ' Create an event handler for the menu items.
        Dim eh As New EventHandler(AddressOf MenuOnViewSelect)

        Dim i As Integer
        For i = 0 To 3
            ' Use a custom class MenuItemView, which extends MenuItem to support a 
            ' View property.
            Dim miv As New MenuItemView()
            miv.Text = astrView(i)
            miv.View = aview(i)
            miv.Checked = False
            ' Associate the handler created earlier with the Click event.
            AddHandler miv.Click, eh

            ' Set the Default view to Details.
            If i = 3 Then
                mivChecked = miv
                mivChecked.Checked = True
                flvFiles.View = mivChecked.View
            End If
            ' Add the new menu item to the View menu.
            menuView.DropDownItems.Add(miv)
        Next i
    End Sub

    ' Handles the OnViewSelect event for the View menu items.
    Sub MenuOnViewSelect(ByVal obj As Object, ByVal ea As EventArgs)
        ' Uncheck the currently checked item.
        mivChecked.Checked = False
        ' Cast the event sender and check it.
        mivChecked = CType(obj, MenuItemView)
        mivChecked.Checked = True
        ' Change how the files are viewed in the FileListView control.
        flvFiles.View = mivChecked.View
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

End Class