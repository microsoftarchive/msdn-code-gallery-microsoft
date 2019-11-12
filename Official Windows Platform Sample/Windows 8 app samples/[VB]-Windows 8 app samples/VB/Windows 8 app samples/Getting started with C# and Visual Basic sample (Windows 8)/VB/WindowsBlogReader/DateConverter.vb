'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Globalization.DateTimeFormatting

Public Class DateConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As String) _
        As Object Implements IValueConverter.Convert
        If value Is Nothing Then
            Throw New ArgumentNullException("value", "Value cannot be null.")
        End If
        If Not GetType(DateTime).Equals(value.GetType()) Then
            Throw New ArgumentException("Value must be of type DateTime.", "value")
        End If

        Dim dt As DateTime = DirectCast(value, DateTime)

        If parameter Is Nothing Then
            ' Date "7/27/2011 9:30:59 AM" returns "7/27/2011"
            Return DateTimeFormatter.ShortDate.Format(dt)

        ElseIf DirectCast(parameter, String) = "day" Then
            ' Date "7/27/2011 9:30:59 AM" returns "27"
            Dim dateFormatter As DateTimeFormatter = New DateTimeFormatter("{day.integer(2)}")
            Return dateFormatter.Format(dt)

        ElseIf DirectCast(parameter, String) = "month" Then
            ' Date "7/27/2011 9:30:59 AM" returns "JUL"
            Dim dateFormatter As DateTimeFormatter = New DateTimeFormatter("{month.abbreviated(3)}")
            Return dateFormatter.Format(dt).ToUpper()

        ElseIf DirectCast(parameter, String) = "year" Then
            ' Date "7/27/2011 9:30:59 AM" returns "2011"
            Dim dateFormatter As DateTimeFormatter = New DateTimeFormatter("{year.full}")
            Return dateFormatter.Format(dt)

        Else
            ' Requested format is unknown. Return in the original format.
            Return dt.ToString()

        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As String) _
        As Object Implements Windows.UI.Xaml.Data.IValueConverter.ConvertBack
        Dim StrValue As String = DirectCast(value, String)
        Dim Result As Date
        If DateTime.TryParse(StrValue, Result) Then
            Return Result
        End If
        Return DependencyProperty.UnsetValue
    End Function
End Class