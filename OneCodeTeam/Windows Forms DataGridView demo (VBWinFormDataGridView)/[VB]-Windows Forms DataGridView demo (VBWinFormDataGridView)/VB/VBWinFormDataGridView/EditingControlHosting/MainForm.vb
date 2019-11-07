'************************************* Module Header **************************************\
' Module Name:  EditingControlHosting
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to host a control in the current DataGridViewCell  for 
' editing.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/

Namespace VBWinFormDataGridView.EditingControlHosting

    Public Class MainForm
        Private maskedTextBoxForEditing As MaskedTextBox
        Private IsKeyPressHandled As Boolean

        Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.maskedTextBoxForEditing = New MaskedTextBox()

            ' The "000-00-0000" mask allows only digits can be input
            Me.maskedTextBoxForEditing.Mask = "000-00-0000"

            ' Hide the MaskedTextBox
            Me.maskedTextBoxForEditing.Visible = False

            ' Add the MaskedTextBox to the DataGridView's control collection
            Me.dataGridView1.Controls.Add(Me.maskedTextBoxForEditing)

            ' Add a DataGridViewTextBoxColumn to the 
            Dim tc As DataGridViewTextBoxColumn = New DataGridViewTextBoxColumn()
            tc.HeaderText = "Mask Column"
            tc.Name = "MaskColumn"
            Me.dataGridView1.Columns.Add(tc)

            ' Add some empty rows for testing purpose
            For j As Integer = 0 To 29
                Me.dataGridView1.Rows.Add()
            Next

            ' Handle the CellBeginEdit event to show the MaskedTextBox on
            ' the current editing cell
        End Sub

        Private Sub dataGridView1_CellBeginEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles dataGridView1.CellBeginEdit
            ' If the current cell is on the "MaskColumn", we use the MaskedTextBox control
            ' for editing instead of the default TextBox control
            If e.ColumnIndex = Me.dataGridView1.Columns("MaskColumn").Index Then
                ' Calculate the cell bounds of the current cell
                Dim rect As Rectangle = Me.dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, True)
                ' Adjust the MaskedTextBox's size and location to fit the cell
                Me.maskedTextBoxForEditing.Size = rect.Size
                Me.maskedTextBoxForEditing.Location = rect.Location
                ' Set value for the MaskedTextBox
                If Me.dataGridView1.CurrentCell.Value IsNot Nothing Then
                    Me.maskedTextBoxForEditing.Text = Me.dataGridView1.CurrentCell.Value.ToString()
                End If

                ' Show the MaskedTextBox
                Me.maskedTextBoxForEditing.Visible = True
            End If
        End Sub

        Private Sub dataGridView1_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dataGridView1.CellEndEdit
            ' When finish editing on the "MaskColumn", we replace the cell value with
            ' the text typed in the MaskedTextBox, and hide the MaskedTextBox
            If e.ColumnIndex = Me.dataGridView1.Columns("MaskColumn").Index Then
                Me.dataGridView1.CurrentCell.Value = Me.maskedTextBoxForEditing.Text
                Me.maskedTextBoxForEditing.Text = ""
                Me.maskedTextBoxForEditing.Visible = False
            End If
        End Sub

        Private Sub dataGridView1_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles dataGridView1.Scroll
            If Me.dataGridView1.IsCurrentCellInEditMode = True Then
                ' Adjust the location for the MaskedTextBox while scrolling
                Dim rect As Rectangle = Me.dataGridView1.GetCellDisplayRectangle(Me.dataGridView1.CurrentCell.ColumnIndex, Me.dataGridView1.CurrentCell.RowIndex, True)
                Console.WriteLine(rect.ToString())
                Console.WriteLine(Me.dataGridView1.CurrentCellAddress.ToString())
                Console.WriteLine("")

                If rect.X <= 0 OrElse rect.Y <= 0 Then
                    Me.maskedTextBoxForEditing.Visible = False
                Else
                    Me.maskedTextBoxForEditing.Location = rect.Location
                End If
            End If
        End Sub

        Private Sub dataGridView1_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles dataGridView1.EditingControlShowing
            If Not (Me.IsKeyPressHandled AndAlso Me.dataGridView1.CurrentCell.ColumnIndex = Me.dataGridView1.Columns("MaskColumn").Index) Then
                Dim tb As TextBox = CType(e.Control, TextBox)
                AddHandler tb.KeyPress, AddressOf tb_KeyPress
                Me.IsKeyPressHandled = True
            End If
        End Sub

        Private Sub tb_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
            If Me.dataGridView1.CurrentCell.ColumnIndex = Me.dataGridView1.Columns("MaskColumn").Index Then
                ' Prevent the key char to be input in the editing control
                e.Handled = True

                ' Set focus to the MaskedTextBox for editing.
                Me.maskedTextBoxForEditing.Focus()
            End If
        End Sub
    End Class

End Namespace