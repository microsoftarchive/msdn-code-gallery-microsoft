'********************************* Module Header **********************************\
' Module Name:  MaskedTextBoxColumn.vb
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

    Public Class MaskedTextBoxColumn
        Inherits DataGridViewTextBoxColumn

        Private _mask As String
        Private _promptChar As Char
        Private _includePrompt As Boolean
        Private _includeLiterals As Boolean
        Private _validatingType As Type

        Public Sub New()
            MyBase.CellTemplate = New MaskedTextBoxCell
        End Sub

        Private Shared Function TriBool(ByVal value As Boolean) As DataGridViewTriState
            If value Then
                Return DataGridViewTriState.True
            Else
                Return DataGridViewTriState.False
            End If
        End Function

        Public Overrides Property CellTemplate() As System.Windows.Forms.DataGridViewCell
            Get
                Return MyBase.CellTemplate
            End Get
            Set(ByVal value As System.Windows.Forms.DataGridViewCell)
                If value IsNot Nothing AndAlso Not value.GetType().IsAssignableFrom(GetType(MaskedTextBoxCell)) Then
                    Dim s As String = "Cell type is not based upon the MaskedTextBoxCell."
                    Throw New InvalidCastException(s)
                End If
                MyBase.CellTemplate = value
            End Set
        End Property

        Public Overridable Property Mask() As String
            Get
                Return Me._mask
            End Get
            Set(ByVal value As String)
                Dim mtbCell As MaskedTextBoxCell
                Dim dgvCell As DataGridViewCell
                Dim rowCount As Integer

                If Me._mask <> value Then
                    Me._mask = value
                    '
                    ' First, update the value on the template cell.
                    '
                    mtbCell = DirectCast(Me.CellTemplate, MaskedTextBoxCell)
                    mtbCell.Mask = value
                    '
                    ' Now set it on all cells in other rows as well.
                    '
                    If Me.DataGridView IsNot Nothing AndAlso Me.DataGridView.Rows IsNot Nothing Then
                        rowCount = Me.DataGridView.Rows.Count
                        For x As Integer = 0 To rowCount - 1
                            dgvCell = Me.DataGridView.Rows.SharedRow(x).Cells(x)
                            If TypeOf dgvCell Is MaskedTextBoxCell Then
                                mtbCell = CType(dgvCell, MaskedTextBoxCell)
                                mtbCell.Mask = value
                            End If
                        Next
                    End If
                End If
            End Set
        End Property

        Public Overridable Property PromptChar() As Char
            Get
                Return Me._promptChar
            End Get
            Set(ByVal value As Char)
                Dim mtbCell As MaskedTextBoxCell
                Dim dgvCell As DataGridViewCell
                Dim rowCount As Integer
                If Me._promptChar <> value Then
                    Me._promptChar = value
                    '
                    ' First, update the value on the template cell.
                    '
                    mtbCell = CType(Me.CellTemplate, MaskedTextBoxCell)
                    mtbCell.PromptChar = value
                    '
                    ' Now set it on all cells in other rows as well.
                    '
                    If Me.DataGridView IsNot Nothing AndAlso Me.DataGridView.Rows IsNot Nothing Then
                        rowCount = Me.DataGridView.Rows.Count
                        For x As Integer = 0 To rowCount - 1
                            dgvCell = Me.DataGridView.Rows.SharedRow(x).Cells(x)
                            If TypeOf dgvCell Is MaskedTextBoxCell Then
                                mtbCell = CType(dgvCell, MaskedTextBoxCell)
                                mtbCell.PromptChar = value
                            End If
                        Next
                    End If
                End If
            End Set
        End Property

        Public Overridable Property IncludePrompt() As Boolean
            Get
                Return Me._includePrompt
            End Get
            Set(ByVal value As Boolean)
                Dim mtbc As MaskedTextBoxCell
                Dim dgvc As DataGridViewCell
                Dim rowCount As Integer
                If Me._includePrompt <> value Then
                    Me._includePrompt = value

                    '
                    ' First, update the value on the template cell.
                    '
                    mtbc = CType(Me.CellTemplate, MaskedTextBoxCell)
                    mtbc.IncludePrompt = TriBool(value)
                    '
                    ' Now set it on all cells in other rows as well.
                    '
                    If Me.DataGridView IsNot Nothing AndAlso Me.DataGridView.Rows IsNot Nothing Then
                        rowCount = Me.DataGridView.Rows.Count
                        For x As Integer = 0 To rowCount - 1
                            dgvc = Me.DataGridView.Rows.SharedRow(x).Cells(x)
                            If TypeOf dgvc Is MaskedTextBoxCell Then
                                mtbc = CType(dgvc, MaskedTextBoxCell)
                                mtbc.IncludePrompt = TriBool(value)
                            End If
                        Next
                    End If
                End If
            End Set
        End Property

        Public Overridable Property IncludeLiterals() As Boolean
            Get
                Return Me._includeLiterals
            End Get
            Set(ByVal value As Boolean)
                Dim mtbCell As MaskedTextBoxCell
                Dim dgvCell As DataGridViewCell
                Dim rowCount As Integer
                If Me._includeLiterals <> value Then
                    Me._includeLiterals = value
                    '
                    ' First, update the value on the template cell.
                    '
                    mtbCell = CType(Me.CellTemplate, MaskedTextBoxCell)
                    mtbCell.IncludeLiterals = TriBool(value)
                    '
                    ' Now set it on all cells in other rows as well.
                    '
                    If Me.DataGridView IsNot Nothing AndAlso Me.DataGridView.Rows IsNot Nothing Then
                        rowCount = Me.DataGridView.Rows.Count
                        For x As Integer = 0 To rowCount - 1
                            dgvCell = Me.DataGridView.Rows.SharedRow(x).Cells(x)
                            If TypeOf dgvCell Is MaskedTextBoxCell Then
                                mtbCell = CType(dgvCell, MaskedTextBoxCell)
                                mtbCell.IncludeLiterals = TriBool(value)
                            End If
                        Next
                    End If
                End If
            End Set
        End Property

        Public Overridable Property ValidatingType() As Type
            Get
                Return Me._validatingType
            End Get
            Set(ByVal value As Type)
                Dim mtbCell As MaskedTextBoxCell
                Dim dgvCell As DataGridViewCell
                Dim rowCount As Integer
                If Me._validatingType IsNot value Then
                    Me._validatingType = value
                    '
                    ' First, update the value on the template cell.
                    '
                    mtbCell = CType(Me.CellTemplate, MaskedTextBoxCell)
                    mtbCell.ValidatingType = value
                    '
                    ' Now set it on all cells in other rows as well.
                    '
                    If Me.DataGridView IsNot Nothing AndAlso Me.DataGridView.Rows IsNot Nothing Then
                        rowCount = Me.DataGridView.Rows.Count
                        For x As Integer = 0 To rowCount - 1
                            dgvCell = Me.DataGridView.Rows.SharedRow(x).Cells(x)
                            If TypeOf dgvCell Is MaskedTextBoxCell Then
                                mtbCell = CType(dgvCell, MaskedTextBoxCell)
                                mtbCell.ValidatingType = value
                            End If
                        Next
                    End If
                End If
            End Set
        End Property

    End Class

End Namespace
