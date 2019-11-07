' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm



    ''' <summary>
    '''Declare constant for use in detecting whether the Ctrl key was pressed during the drag operation. 
    ''' </summary>
    Const CtrlMask As Byte = 8


    ''' <summary>
    ''' Handles the event that fires when the Form first loads. 
    ''' </summary>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' There is currently no way to set the AllowDrop property for a PictureBox
        ' in the Visual Studio designer, so they must be set explicitly in the code.
        picLeft.AllowDrop = True
        picRight.AllowDrop = True
    End Sub

    ''' <summary>
    '''Handles the MouseDown event for the left TextBox. This event fires when the
    ''' mouse is in the control's bounds and the mouse button is clicked.
    ''' </summary>
    Private Sub txtSource_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtSource.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            txtSource.SelectAll()
            ' Invoke the drag and drop operation.
            txtSource.DoDragDrop(txtSource.SelectedText, DragDropEffects.Move Or DragDropEffects.Copy)
        End If
    End Sub

    ''' <summary>
    ''' Handles the MouseDown event for both PictureBox controls. This event fires 
    ''' when the mouse is in the control's bounds and the mouse button is 
    ''' clicked.
    ''' </summary>
    Private Sub PictureBox_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picLeft.MouseDown, picRight.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Dim pic As PictureBox = CType(sender, PictureBox)
            ' Invoke the drag and drop operation.
            If Not pic.Image Is Nothing Then
                pic.DoDragDrop(pic.Image, DragDropEffects.Move Or DragDropEffects.Copy)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handles the DragEnter event for both PictureBox controls. DragEnter is the
    ''' event that fires when an object is dragged into the control's bounds.
    ''' </summary>
    Private Sub PictureBox_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles picLeft.DragEnter, picRight.DragEnter
        ' Check to be sure that the drag content is the correct type for this 
        ' control. If not, reject the drop.
        If (e.Data.GetDataPresent(DataFormats.Bitmap)) Then
            ' If the Ctrl key was pressed during the drag operation then perform
            ' a Copy. If not, perform a Move.
            If (e.KeyState And CtrlMask) = CtrlMask Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.Move
            End If
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    ''' <summary>
    ''' This method handles the DragDrop event for both PictureBox controls. One handler can be 
    ''' used for both PictureBox controls. The Is keyword is used to find the identity of 
    ''' the selected PictureBox control.
    ''' </summary>
    Private Sub PictureBox_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles picLeft.DragDrop, picRight.DragDrop
        ' Display the image in the selected PictureBox control.
        Dim pic As PictureBox = CType(sender, PictureBox)
        pic.Image = CType(e.Data.GetData(DataFormats.Bitmap), Bitmap)

        ' The image in the other PictureBox (that is, the PictureBox that was
        ' not the sender in the DragDrop event) is removed if the user executes a Move.
        ' The action is a Move if the Ctrl key was not pressed.
        If (e.KeyState And CtrlMask) <> CtrlMask Then
            If sender Is picLeft Then
                picRight.Image = Nothing
            Else
                picLeft.Image = Nothing
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handles the DragDrop event for both TreeView controls. 
    ''' </summary>
    Private Sub TreeView_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles tvwLeft.DragDrop, tvwRight.DragDrop
        ' Initialize variable that holds the node dragged by the user.
        Dim OriginationNode As TreeNode = CType(e.Data.GetData("System.Windows.Forms.TreeNode"), TreeNode)

        ' Calling GetDataPresent is a little different for a TreeView than for a
        ' PictureBox or TextBox control. The TreeNode is not a member of the DataFormats
        ' class. That is, it's not a predefined type. One of the overloads of GetDataPresent takes
        ' a string that lets you specify the type.
        If e.Data.GetDataPresent("System.Windows.Forms.TreeNode", False) Then
            Dim pt As Point
            Dim DestinationNode As TreeNode

            ' Use PointToClient to compute the location of the mouse over the
            ' destination TreeView.
            pt = CType(sender, TreeView).PointToClient(New Point(e.X, e.Y))
            ' Use this Point to get the closest node in the destination TreeView.
            DestinationNode = CType(sender, TreeView).GetNodeAt(pt)

            ' If user didn't drop the new node directly on top of a node, then
            ' DestinationNode will be Nothing.
            If DestinationNode IsNot Nothing Then
                ' If the original node is the same as the destination node, the
                ' node would disappear. This code ensures that does not happen.
                If Not DestinationNode.TreeView Is OriginationNode.TreeView Then
                    DestinationNode.Nodes.Add(CType(OriginationNode.Clone, TreeNode))
                    ' Expand the parent node when adding the new node so that the drop
                    ' is obvious. Without this, only a + symbol would appear.
                    DestinationNode.Expand()
                    ' If the Ctrl key was not pressed, remove the original node to 
                    ' effect a drag-and-drop move.
                    If (e.KeyState And CtrlMask) <> CtrlMask Then
                        OriginationNode.Remove()
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handles the DragEnter event for both TreeView controls.
    ''' </summary>
    Private Sub TreeView_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles tvwLeft.DragEnter, tvwRight.DragEnter
        ' Check to be sure that the drag content is the correct type for this 
        ' control. If not, reject the drop.
        If (e.Data.GetDataPresent("System.Windows.Forms.TreeNode")) Then
            ' If the Ctrl key was pressed during the drag operation then perform
            ' a Copy. If not, perform a Move.
            If (e.KeyState And CtrlMask) = CtrlMask Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.Move
            End If
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    ''' <summary>
    ''' Handles the ItemDrag event for both TreeView controls.
    ''' </summary>
    Private Sub TreeView_ItemDrag(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles tvwLeft.ItemDrag, tvwRight.ItemDrag
        If e.Button = Windows.Forms.MouseButtons.Left Then
            'invoke the drag and drop operation
            DoDragDrop(e.Item, DragDropEffects.Move Or DragDropEffects.Copy)
        End If
    End Sub

    ''' <summary>
    ''' Handles the DragEnter event for the TextBox that allows dropping. DragEnter is the
    ''' event that fires when an object is dragged into the control's bounds.
    ''' </summary>
    Private Sub txtUpperRight_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles txtAllowDrop.DragEnter
        ' Check to be sure that the drag content is the correct type for this 
        ' control. If not, reject the drop.
        If (e.Data.GetDataPresent(DataFormats.Text)) Then
            ' If the Ctrl key was pressed during the drag operation then perform
            ' a Copy. If not, perform a Move.
            If (e.KeyState And CtrlMask) = CtrlMask Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.Move
            End If
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    ''' <summary>
    ''' Handles the DragDrop event for the TextBox that allows dropping. This event fires
    ''' when the mouse button is released, terminating the drag-and-drop operation.
    ''' </summary>
    Private Sub txtAllowDrop_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles txtAllowDrop.DragDrop
        txtAllowDrop.Text = e.Data.GetData(DataFormats.Text).ToString

        ' If the Ctrl key was not pressed, remove the source text to effect a 
        ' drag-and-drop move.
        If (e.KeyState And CtrlMask) <> CtrlMask Then
            txtSource.Text = ""
        End If
    End Sub


    ''' <summary>
    ''' Exit the application
    ''' </summary>
    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class