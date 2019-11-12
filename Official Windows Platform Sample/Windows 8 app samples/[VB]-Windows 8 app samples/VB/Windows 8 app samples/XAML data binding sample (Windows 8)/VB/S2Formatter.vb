Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Windows.UI.Xaml.Data

#Region "Scenario2 Value Converter"
Public Class S2Formatter
    Implements IValueConverter
    'Convert the slider value into Grades
    Public Function Convert(value As Object, type As System.Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
        Dim _value As Integer
        Dim _grade As String = String.Empty
        'try parsing the value to int
        If Int32.TryParse(value.ToString, _value) Then
            If _value < 50 Then
                _grade = "F"
            ElseIf _value < 60 Then
                _grade = "D"
            ElseIf _value < 70 Then
                _grade = "C"
            ElseIf _value < 80 Then
                _grade = "B"
            ElseIf _value < 90 Then
                _grade = "A"
            ElseIf _value < 100 Then
                _grade = "A+"
            ElseIf _value = 100 Then
                _grade = "SUPER STAR!"
            End If
        End If

        Return _grade
    End Function

    Public Function ConvertBack(value As Object, type As System.Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
        'doing one-way binding so this is not required.
    End Function
End Class
#End Region
