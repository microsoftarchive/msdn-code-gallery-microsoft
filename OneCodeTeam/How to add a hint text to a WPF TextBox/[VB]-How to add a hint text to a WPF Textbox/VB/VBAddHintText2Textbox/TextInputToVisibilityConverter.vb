Imports System.Windows
Imports System.Windows.Data


'****************************** Module Header ******************************\
'Module Name:    TextInputToVisibilityConverter.cs
'Project:        VBAddHintText2Textbox
'Copyright (c) Microsoft Corporation

' The project illustrates how to check whether a file is in use or not.

'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
'All other rights reserved.

'*****************************************************************************/

Public Class TextInputToVisibilityConverter
    Implements IMultiValueConverter

    Public Function Convert1(ByVal values() As Object, _
                     ByVal targetType As System.Type, _
                     ByVal parameter As Object, _
                     ByVal culture As System.Globalization.CultureInfo) As Object _
                     Implements System.Windows.Data.IMultiValueConverter.Convert

        ' Test for non-null
        If TypeOf values(0) Is Boolean AndAlso TypeOf values(1) Is Boolean Then
            Dim hasText As Boolean = Not CBool(values(0))
            Dim hasFocus As Boolean = CBool(values(1))

            If hasFocus OrElse hasText Then
                Return Visibility.Collapsed
            End If
        End If

        Return Visibility.Visible

    End Function

    Public Function ConvertBack1(ByVal value As Object, _
                                 ByVal targetTypes() As System.Type, _
                                 ByVal parameter As Object, _
                                 ByVal culture As System.Globalization.CultureInfo) As Object() _
                                 Implements System.Windows.Data.IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class

