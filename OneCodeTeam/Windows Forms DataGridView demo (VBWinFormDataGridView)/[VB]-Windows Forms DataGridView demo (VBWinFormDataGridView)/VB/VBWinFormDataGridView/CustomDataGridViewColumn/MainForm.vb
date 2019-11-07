'************************************* Module Header **************************************\
' Module Name:  MaskedTextBoxColumn.vb
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates the use of custom column definitions within the Windows Forms 
' DataGridView control.
' 
' The Employee ID, SSN, State and Zip Code columns use MaskedTextBox controls for format 
' and validate their input.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.

' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/

Imports System.Text

Namespace VBWinFormDataGridView.CustomDataGridViewColumn

    Public Class MainForm
        Inherits Form

        Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim dgvTextBoxColumn As DataGridViewTextBoxColumn
            Dim mtbColumn As MaskedTextBoxColumn
            '
            ' Employee name.
            '
            dgvTextBoxColumn = New DataGridViewTextBoxColumn()
            dgvTextBoxColumn.HeaderText = "Name"
            dgvTextBoxColumn.Width = 120
            Me.employeesDataGridView.Columns.Add(dgvTextBoxColumn)

            '
            ' Employee ID -- this will be of the format:
            ' [A-Z][0-9][0-9][0-9][0-9][0-9]
            '
            ' this is well sutied to using a MaskedTextBox column.
            '
            mtbColumn = New MaskedTextBoxColumn()
            mtbColumn.HeaderText = "Employee ID"
            mtbColumn.Mask = "L00000"
            mtbColumn.Width = 75
            Me.employeesDataGridView.Columns.Add(mtbColumn)

            '
            ' [American] Social Security number, of the format:
            ' ###-##-####
            ' 
            mtbColumn = New MaskedTextBoxColumn()
            mtbColumn.HeaderText = "SSN"
            mtbColumn.Mask = "000-00-0000"
            mtbColumn.Width = 75
            Me.employeesDataGridView.Columns.Add(mtbColumn)

            '
            ' Address
            '
            dgvTextBoxColumn = New DataGridViewTextBoxColumn()
            dgvTextBoxColumn.HeaderText = "Address"
            dgvTextBoxColumn.Width = 150
            Me.employeesDataGridView.Columns.Add(dgvTextBoxColumn)

            '
            ' City
            '
            dgvTextBoxColumn = New DataGridViewTextBoxColumn()
            dgvTextBoxColumn.HeaderText = "City"
            dgvTextBoxColumn.Width = 75
            Me.employeesDataGridView.Columns.Add(dgvTextBoxColumn)

            '
            ' State
            '
            mtbColumn = New MaskedTextBoxColumn()
            mtbColumn.HeaderText = "State"
            mtbColumn.Mask = "LL"
            mtbColumn.Width = 40
            Me.employeesDataGridView.Columns.Add(mtbColumn)

            '
            ' Zip Code #####-#### (+4 optional)
            '
            mtbColumn = New MaskedTextBoxColumn()
            mtbColumn.HeaderText = "Zip Code"
            mtbColumn.Mask = "00000-0000"
            mtbColumn.Width = 75
            mtbColumn.ValidatingType = GetType(ZipCode)
            Me.employeesDataGridView.Columns.Add(mtbColumn)

            '
            ' Department Code
            '
            dgvTextBoxColumn = New DataGridViewTextBoxColumn()
            dgvTextBoxColumn.HeaderText = "Department"
            dgvTextBoxColumn.ValueType = GetType(Integer)
            dgvTextBoxColumn.Width = 75
            Me.employeesDataGridView.Columns.Add(dgvTextBoxColumn)
        End Sub
    End Class

#Region "ZipCode Class"
    Public Class ZipCode
        Private zipCode As Integer
        Private plusFour As Integer

        Public Sub New()
            Me.zipCode = 0
            Me.plusFour = 0
        End Sub

        Public Sub New(ByVal in_zipCode As String)

        End Sub

        Public Sub New(ByVal in_ivalue)
            Me.zipCode = in_ivalue
            Me.plusFour = 0
        End Sub

        Public Shared Narrowing Operator CType(ByVal s As String) As ZipCode
            Return New ZipCode(s)
        End Operator

        Public Shared Narrowing Operator CType(ByVal i As Integer) As ZipCode
            Return New ZipCode(i)
        End Operator

        Protected Shared Sub parseFromString(ByVal in_string As String, ByRef out_zipCode As Integer, _
                                             ByRef out_plusFour As Integer)
            Dim zc As Integer = 0, pf = 0
            Dim charray As Char()
            Dim x As Integer = 0
            If in_string Is Nothing OrElse in_string.Equals("") Then
                Throw New ArgumentException("Invalid String")
            End If

            charray = in_string.Trim().ToCharArray()

            For x = 0 To 4
                If Not (Char.IsDigit(charray(x))) Then
                    Throw New ArgumentException("Invalid String")
                End If
                zc = zc * 10 + numfromchar(charray(x))
            Next

            While x < charray.Length AndAlso Not (Char.IsDigit(charray(x)))
                x = x + 1
            End While

            If x < charray.Length Then
                For y As Integer = x To 3
                    If Not (Char.IsDigit(charray(y))) Then
                        Throw New ArgumentException("Invalid ZipCode String")
                    End If
                    pf = pf * 10 + numfromchar(charray(y))
                Next
            End If

            out_zipCode = zc
            out_plusFour = pf
        End Sub

        Private Shared Function numfromchar(ByVal c As Char)
            Select Case c
                Case "0"c
                    Return 0
                Case "1"c
                    Return 1
                Case "2"c
                    Return 2
                Case "3"c
                    Return 3
                Case "4"c
                    Return 4
                Case "5"c
                    Return 5
                Case "6"c
                    Return 6
                Case "7"c
                    Return 7
                Case "8"c
                    Return 8
                Case "9"c
                    Return 9
                Case Else
                    Throw New ArgumentException("invalid digit")
            End Select
        End Function

        Public Overrides Function ToString() As String
            Dim sb As StringBuilder = New StringBuilder(10)
            sb.Append(zipCode.ToString("00000"))
            sb.Append("_")
            sb.Append(plusFour.ToString("0000"))
            Return sb.ToString()
        End Function
    End Class
#End Region

End Namespace