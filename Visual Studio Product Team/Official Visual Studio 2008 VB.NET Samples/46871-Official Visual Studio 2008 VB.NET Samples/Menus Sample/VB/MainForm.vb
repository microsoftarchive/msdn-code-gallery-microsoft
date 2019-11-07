' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

#Region "Event Handlers"

    Private Sub MenuItem_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles FileMenu.MouseEnter, NewMenuItem.MouseEnter, Option1.MouseEnter, MoreOptions.MouseEnter, MoreOptions1.MouseEnter, MoreOptions2.MouseEnter, MoreOptions3.MouseEnter, Option2.MouseEnter, Option3.MouseEnter, OpenMenuItem.MouseEnter, ViewToolStripMenuItem1.MouseEnter, StatusStripOption.MouseEnter, CheckedListMenu.MouseEnter, AddOptionMenuItem.MouseEnter, RemoveOptionMenuItem.MouseEnter

        Dim selected As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        SelectedItem.Text = selected.Text

    End Sub

    Private Sub MenuItem_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles FileMenu.MouseLeave, NewMenuItem.MouseLeave, Option1.MouseLeave, MoreOptions.MouseLeave, MoreOptions1.MouseLeave, MoreOptions2.MouseLeave, MoreOptions3.MouseLeave, Option2.MouseLeave, Option3.MouseLeave, OpenMenuItem.MouseLeave, ViewToolStripMenuItem1.MouseLeave, StatusStripOption.MouseLeave, CheckedListMenu.MouseLeave, AddOptionMenuItem.MouseLeave, RemoveOptionMenuItem.MouseLeave
        SelectedItem.Text = ""
    End Sub

    Private Sub NewMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewMenuItem.Click
        Me.BackColor = Color.White
    End Sub

    Private Sub MenuOption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        For Each item As Object In CheckedListMenu.DropDownItems
            If (TypeOf item Is ToolStripMenuItem) Then
                Dim itemObject As ToolStripMenuItem = CType(item, ToolStripMenuItem)
                itemObject.Checked = False
            End If
        Next

        Dim selectedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        selectedItem.Checked = True
    End Sub

    Private Sub AddOptionMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddOptionMenuItem.Click
        AddOption()
    End Sub

    Private Sub RemoveOptionMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveOptionMenuItem.Click
        Dim itemToRemove As ToolStripMenuItem = Nothing

        If CheckedListMenu.DropDownItems.Count > 3 Then
            itemToRemove = CType(CheckedListMenu.DropDownItems(CheckedListMenu.DropDownItems.Count - 1), ToolStripMenuItem)
            Dim removeAt As Integer = CheckedListMenu.DropDownItems.Count - 1
            If itemToRemove.Checked And CheckedListMenu.DropDownItems.Count > 4 Then
                Dim itemToCheck As ToolStripMenuItem = CType(CheckedListMenu.DropDownItems(CheckedListMenu.DropDownItems.Count - 2), ToolStripMenuItem)
                itemToCheck.Checked = True
            End If
            CheckedListMenu.DropDownItems.RemoveAt(removeAt)
        End If
    End Sub

    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim combo As ToolStripComboBox = CType(sender, ToolStripComboBox)
        Select Case combo.SelectedIndex
            Case 0
                MenuStrip1.Dock = DockStyle.Top
            Case 1
                MenuStrip1.Dock = DockStyle.Bottom
            Case 2
                MenuStrip1.Dock = DockStyle.Left
                MenuStrip1.Width = 50
            Case 3
                MenuStrip1.Dock = DockStyle.Right
                MenuStrip1.Width = 50
            Case Else
                MenuStrip1.Dock = DockStyle.Top
        End Select
    End Sub

    Private Sub DropDownColorItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim menuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Select Case menuItem.Name
            Case "Blue"
                Me.BackColor = Color.Blue
            Case "Red"
                Me.BackColor = Color.Red
            Case "Green"
                Me.BackColor = Color.Green
        End Select
    End Sub

    Private Sub ColorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim button As ToolStripButton = CType(sender, ToolStripButton)
        Select Case button.Name
            Case "Blue"
                Me.BackColor = Color.Blue
            Case "Red"
                Me.BackColor = Color.Red
            Case "Green"
                Me.BackColor = Color.Green
        End Select
    End Sub

    Private Sub SplitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.BackColor = Color.Blue
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CreateInitialMenus()
    End Sub

    Private Sub StatusStripOption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusStripOption.Click
        If StatusStripOption.Checked Then
            MainStatusStrip.Visible = True
        Else
            MainStatusStrip.Visible = False
        End If
    End Sub

#End Region



    Private Sub AddOption()

        Dim newOption As New ToolStripMenuItem()
        newOption.Checked = False
        newOption.CheckOnClick = True
        newOption.Text = "Option " & (CheckedListMenu.DropDownItems.Count - 2).ToString()

        If CheckedListMenu.DropDownItems.Count = 3 Then
            newOption.Checked = True
        End If

        ' Add the event handlers for the Click and MouseEnter events of the new option.
        AddHandler newOption.Click, AddressOf Me.MenuOption_Click
        AddHandler newOption.MouseEnter, AddressOf Me.MenuItem_MouseEnter
        AddHandler newOption.MouseLeave, AddressOf Me.MenuItem_MouseLeave

        CheckedListMenu.DropDownItems.Add(newOption)

    End Sub

    Private Sub CreateInitialMenus()

        ' Add three options to the checked list menu.
        For i As Integer = 1 To 3
            AddOption()
        Next

        ' Add a Label to the MenuStrip.
        Dim menuLabel As New ToolStripLabel()
        menuLabel.Text = "ComboBox:"

        MenuStrip1.Items.Add(menuLabel)

        ' Add a combo box and fill it with items.
        Dim menuComboBox As New ToolStripComboBox()
        menuComboBox.Items.Add("Top")
        menuComboBox.Items.Add("Bottom")
        menuComboBox.Items.Add("Left")
        menuComboBox.Items.Add("Right")
        menuComboBox.SelectedIndex = 0
        menuComboBox.ToolTipText = "Select Raft Location"

        AddHandler menuComboBox.SelectedIndexChanged, AddressOf Me.ComboBox_SelectedIndexChanged
        MenuStrip1.Items.Add(menuComboBox)

        Dim button1 As New ToolStripButton()
        Dim button2 As New ToolStripButton()
        Dim button3 As New ToolStripButton()

        button1.Image = My.Resources.SampleImage1
        button2.Image = My.Resources.SampleImage2
        button3.Image = My.Resources.SampleImage3

        button1.ToolTipText = "Blue"
        button2.ToolTipText = "Red"
        button3.ToolTipText = "Green"

        button1.Name = "Blue"
        button2.Name = "Red"
        button3.Name = "Green"

        AddHandler button1.Click, AddressOf Me.ColorButton_Click
        AddHandler button2.Click, AddressOf Me.ColorButton_Click
        AddHandler button3.Click, AddressOf Me.ColorButton_Click

        MenuStrip1.Items.Add(button1)
        MenuStrip1.Items.Add(button2)
        MenuStrip1.Items.Add(button3)

        Dim colorSplitButton As New ToolStripSplitButton
        colorSplitButton.Text = "Blue"

        AddHandler colorSplitButton.ButtonClick, AddressOf Me.SplitButton_Click

        Dim colorOption1 As New ToolStripMenuItem
        Dim colorOption2 As New ToolStripMenuItem
        Dim colorOption3 As New ToolStripMenuItem

        colorOption1.Text = "Blue"
        colorOption2.Text = "Red"
        colorOption3.Text = "Green"

        colorOption1.ToolTipText = "Blue"
        colorOption2.ToolTipText = "Red"
        colorOption3.ToolTipText = "Green"

        colorOption1.Name = "Blue"
        colorOption2.Name = "Red"
        colorOption3.Name = "Green"

        colorOption1.Image = My.Resources.SampleImage1
        colorOption2.Image = My.Resources.SampleImage2
        colorOption3.Image = My.Resources.SampleImage3


        AddHandler colorOption1.Click, AddressOf Me.DropDownColorItem_Click
        AddHandler colorOption2.Click, AddressOf Me.DropDownColorItem_Click
        AddHandler colorOption3.Click, AddressOf Me.DropDownColorItem_Click

        colorSplitButton.DropDownItems.Add(colorOption1)
        colorSplitButton.DropDownItems.Add(colorOption2)
        colorSplitButton.DropDownItems.Add(colorOption3)

        MenuStrip1.Items.Add(colorSplitButton)


    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
