'********************************* Module Header **********************************\
' Module Name:  MaskedTextBoxCell.vb
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to create a custom DataGridView column.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/

Namespace VBWinFormDataGridView.CustomDataGridViewColumn
    Public Class MaskedTextBoxCell
        Inherits DataGridViewTextBoxCell

        Private _mask As String
        Private _promptChar As Char
        Private _includePrompt As DataGridViewTriState
        Private _includeLiterals As DataGridViewTriState
        Private _validatingType As Type

        Public Sub New()
            Me._mask = ""
            Me._promptChar = "_"c
            Me._includePrompt = DataGridViewTriState.NotSet
            Me._includeLiterals = DataGridViewTriState.NotSet
            Me._validatingType = GetType(String)
        End Sub

        Public Overrides Sub InitializeEditingControl(ByVal rowIndex As Integer, _
                ByVal initialFormattedValue As Object, ByVal dataGridViewCellStyle As DataGridViewCellStyle)

            Dim mtbEditingCtrl As MaskedTextBoxEditingControl
            Dim mtbColumn As MaskedTextBoxColumn
            Dim dgvColumn As DataGridViewColumn

            MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, _
                                          dataGridViewCellStyle)

            mtbEditingCtrl = CType(DataGridView.EditingControl, MaskedTextBoxEditingControl)

            '
            ' Set up props that are specific to the MaskedTextBox
            '

            dgvColumn = Me.OwningColumn   ' this.DataGridView.Columns[this.ColumnIndex];
            If TypeOf dgvColumn Is MaskedTextBoxColumn Then

                mtbColumn = CType(dgvColumn, MaskedTextBoxColumn)
            End If
            '
            ' get the mask from this instance or the parent column.
            '
            If String.IsNullOrEmpty(Me._mask) Then

                mtbEditingCtrl.Mask = mtbColumn.Mask

            Else
                mtbEditingCtrl.Mask = Me._mask
                '
                ' Prompt char.
                '
                mtbEditingCtrl.PromptChar = Me._promptChar
                '
                ' IncludePrompt
                '
            End If
            If Me._includePrompt = DataGridViewTriState.NotSet Then
                'mtbEditingCtrl.IncludePrompt = mtbcol.IncludePrompt
            Else
                'mtbEditingCtrl.IncludePrompt = BoolFromTri(Me.includePrompt)
                '
                ' IncludeLiterals
                '
            End If

            If Me._includeLiterals = DataGridViewTriState.NotSet Then
                'mtbEditingCtrl.IncludeLiterals = mtbcol.IncludeLiterals
            Else
                'mtbEditingCtrl.IncludeLiterals = BoolFromTri(Me.includeLiterals)
            End If
            '
            ' Finally, the validating type ...
            '
            If Me.ValidatingType Is Nothing Then
                mtbEditingCtrl.ValidatingType = mtbColumn.ValidatingType
            Else
                mtbEditingCtrl.ValidatingType = Me.ValidatingType
                mtbEditingCtrl.Text = CType(Me.Value, String)
            End If
        End Sub

        Public Overrides ReadOnly Property EditType() As System.Type
            Get
                Return GetType(MaskedTextBoxEditingControl)
            End Get
        End Property

        Public Overridable Property Mask() As String
            Get
                Return Me._mask
            End Get
            Set(ByVal value As String)
                Me._mask = value
            End Set
        End Property

        Public Overridable Property PromptChar() As Char
            Get
                Return Me._promptChar
            End Get
            Set(ByVal value As Char)
                Me._promptChar = value
            End Set
        End Property

        Public Overridable Property IncludePrompt() As DataGridViewTriState
            Get
                Return Me._includePrompt
            End Get
            Set(ByVal value As DataGridViewTriState)
                Me._includePrompt = value
            End Set
        End Property

        Public Overridable Property IncludeLiterals() As DataGridViewTriState
            Get
                Return Me._includeLiterals
            End Get
            Set(ByVal value As DataGridViewTriState)
                Me._includeLiterals = value
            End Set
        End Property

        Public Overridable Property ValidatingType() As Type
            Get
                Return Me._validatingType
            End Get
            Set(ByVal value As Type)
                Me._validatingType = value
            End Set
        End Property

        Protected Shared Function BoolFromTri(ByVal tri As DataGridViewTriState) As Boolean
            If tri = DataGridViewTriState.True Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

End Namespace
