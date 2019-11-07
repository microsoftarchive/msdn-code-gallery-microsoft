'************************************* Module Header **************************************\
' Module Name:  MultipleLayeredColumnHeader
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to display multiple layer column headers on the DataGridView.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/

Namespace VBWinFormDataGridView.MultipleLayeredColumnHeader

    Public Class MainForm

        Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.dataGridView1.Columns.Add("JanWin", "Win")
            Me.dataGridView1.Columns.Add("JanLoss", "Loss")
            Me.dataGridView1.Columns.Add("FebWin", "Win")
            Me.dataGridView1.Columns.Add("FebLoss", "Loss")
            Me.dataGridView1.Columns.Add("MarWin", "Win")
            Me.dataGridView1.Columns.Add("MarLoss", "Loss")

            For j As Integer = 0 To Me.dataGridView1.ColumnCount - 1
                Me.dataGridView1.Columns(j).Width = 45
            Next

            ' Enable resizing on the column headers
            Me.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing

            ' Adjust the height for the column headers
            Me.dataGridView1.ColumnHeadersHeight = Me.dataGridView1.ColumnHeadersHeight * 2

            ' Adjust the text alignment on the column headers to make the text display
            ' at the center of the bottom
            Me.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter

            ' Handle the CellPainting event to draw text for each header cell

        End Sub

        Private Sub dataGridView1_CellPainting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles dataGridView1.CellPainting
            If e.RowIndex = -1 AndAlso e.ColumnIndex > -1 Then
                e.PaintBackground(e.CellBounds, False)
                Dim r2 As Rectangle = e.CellBounds
                r2.Y += e.CellBounds.Height / 2
                r2.Height = e.CellBounds.Height / 2
                e.PaintContent(r2)
                e.Handled = True
            End If
        End Sub

        Private Sub dataGridView1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles dataGridView1.Paint
            ' Data for the "merged" header cells
            Dim monthes As String() = {"January", "February", "March"}
            For j As Integer = 0 To Me.dataGridView1.ColumnCount - 1 Step 2
                ' Get the column header cell bounds
                Dim r1 As Rectangle = Me.dataGridView1.GetCellDisplayRectangle(j, -1, True)

                r1.X += 1
                r1.Y += 1
                r1.Width = r1.Width * 2 - 2
                r1.Height = r1.Height / 2 - 2

                Using br As SolidBrush = New SolidBrush(Me.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor)
                    e.Graphics.FillRectangle(br, r1)
                End Using

                Using p As Pen = New Pen(SystemColors.InactiveBorder)
                    e.Graphics.DrawLine(p, r1.X, r1.Bottom, r1.Right, r1.Bottom)
                End Using

                Using format As StringFormat = New StringFormat()
                    Using br As SolidBrush = New SolidBrush(Me.dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor)
                        format.Alignment = StringAlignment.Center
                        format.LineAlignment = StringAlignment.Center
                        e.Graphics.DrawString(monthes(j / 2), Me.dataGridView1.ColumnHeadersDefaultCellStyle.Font, _
                                              br, r1, format)
                    End Using
                End Using
            Next
        End Sub
    End Class

End Namespace