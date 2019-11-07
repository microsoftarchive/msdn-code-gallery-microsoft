'********************************* Module Header **********************************\
' Module Name:  MaskedTextBoxEditingControl.vb
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to create a custom DataGridView column.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.

' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/

Namespace VBWinFormDataGridView.CustomDataGridViewColumn

    Public Class MaskedTextBoxEditingControl
        Inherits MaskedTextBox
        Implements IDataGridViewEditingControl

        Private rowIndex As Integer
        Private dataGridView As DataGridView
        Private valueChanged As Boolean = False

        Public Sub New()

        End Sub

        Protected Overrides Sub OnTextChanged(ByVal e As System.EventArgs)
            MyBase.OnTextChanged(e)
            ' Let the DataGridView know about the value change
            NotifyDataGridViewOfValueChange()
        End Sub

        Protected Sub NotifyDataGridViewOfValueChange()
            Me.valueChanged = True
            If Me.dataGridView IsNot Nothing Then
                Me.dataGridView.NotifyCurrentCellDirty(True)
            End If
        End Sub

#Region "IDataGridViewEditingControl Members"
        Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle) Implements System.Windows.Forms.IDataGridViewEditingControl.ApplyCellStyleToEditingControl
            Me.Font = dataGridViewCellStyle.Font
            Me.ForeColor = dataGridViewCellStyle.ForeColor
            Me.BackColor = dataGridViewCellStyle.BackColor
            Me.TextAlign = translateAlignment(dataGridViewCellStyle.Alignment)
        End Sub

        Public Property EditingControlDataGridView() As System.Windows.Forms.DataGridView Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlDataGridView
            Get
                Return Me.dataGridView
            End Get
            Set(ByVal value As System.Windows.Forms.DataGridView)
                Me.dataGridView = value
            End Set
        End Property

        Public Property EditingControlFormattedValue() As Object Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlFormattedValue
            Get
                Return Me.Text
            End Get
            Set(ByVal value As Object)
                Me.Text = value.ToString()
            End Set
        End Property

        Public Property EditingControlRowIndex() As Integer Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlRowIndex
            Get
                Return Me.rowIndex
            End Get
            Set(ByVal value As Integer)
                Me.rowIndex = value
            End Set
        End Property

        Public Property EditingControlValueChanged() As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlValueChanged
            Get

            End Get
            Set(ByVal value As Boolean)

            End Set
        End Property

        Public Function EditingControlWantsInputKey(ByVal keyData As System.Windows.Forms.Keys, ByVal dataGridViewWantsInputKey As Boolean) As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlWantsInputKey
            Select Case keyData And Keys.KeyCode
                Case Keys.Right
                    If Not (Me.SelectionLength = 0 AndAlso Me.SelectionStart = Me.ToString().Length) Then
                        Return True
                    End If
                    Exit Select

                Case Keys.Left
                    If Not (Me.SelectionLength = 0 AndAlso Me.SelectionStart = 0) Then
                        Return True
                    End If
                    Exit Select

                Case Keys.Home
                Case Keys.End
                    If Me.SelectionLength <> Me.ToString().Length Then
                        Return True
                    End If
                    Exit Select

                Case Keys.Prior
                Case Keys.Next
                    If Me.valueChanged Then
                        Return True
                    End If
                    Exit Select

                Case Keys.Delete
                    If Me.SelectionLength > 0 OrElse Me.SelectionStart < Me.ToString().Length Then
                        Return True
                    End If
                    Exit Select
            End Select

            Return Not dataGridViewWantsInputKey
        End Function

        Public ReadOnly Property EditingPanelCursor() As System.Windows.Forms.Cursor Implements System.Windows.Forms.IDataGridViewEditingControl.EditingPanelCursor
            Get
                Return Cursors.IBeam
            End Get
        End Property

        Public Function GetEditingControlFormattedValue(ByVal context As System.Windows.Forms.DataGridViewDataErrorContexts) As Object Implements System.Windows.Forms.IDataGridViewEditingControl.GetEditingControlFormattedValue
            Return Me.Text
        End Function

        Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) Implements System.Windows.Forms.IDataGridViewEditingControl.PrepareEditingControlForEdit
            If selectAll Then
                Me.SelectAll()
            Else
                Me.SelectionStart = Me.ToString().Length
            End If
        End Sub

        Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.RepositionEditingControlOnValueChange
            Get
                Return False
            End Get
        End Property
#End Region

        Private Shared Function translateAlignment(ByVal align As DataGridViewContentAlignment) As HorizontalAlignment
            Select Case align
                Case DataGridViewContentAlignment.TopLeft, _
                DataGridViewContentAlignment.MiddleLeft, DataGridViewContentAlignment.BottomLeft
                    Return HorizontalAlignment.Left

                Case DataGridViewContentAlignment.TopCenter, _
                DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.BottomCenter
                    Return HorizontalAlignment.Center

                Case DataGridViewContentAlignment.TopRight, _
                DataGridViewContentAlignment.MiddleRight, DataGridViewContentAlignment.BottomRight
                    Return HorizontalAlignment.Right
            End Select
            Throw New ArgumentException("Error: Invalid Content Alignment!")
        End Function
    End Class

End Namespace