'****************************** Module Header ******************************\
' Module Name:  DoubleToMoneyString.vb
' Project:      VBWindowsStoreAppFlightDataFilter
' Copyright (c) Microsoft Corporation.
'
' DoubleToMoneyString converter. 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Imports System
Imports Windows.UI.Xaml.Data

Namespace Converters
    Public Class DoubleToMoneyString
        Implements IValueConverter
        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            Dim price As Double = CDbl(value)
            Return String.Format("{0:C2}", price)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace