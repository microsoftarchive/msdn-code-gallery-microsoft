'/****************************** Module Header ******************************\
' Module Name:      CallWeatherRibbon.vb
' Project:                   VBExcelCallWebService
' Copyright(c)          Microsoft Corporation.
' 
' The class is the  custom ribbon of excel.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Imports Microsoft.Office.Tools.Ribbon

Public Class CallWeatherRibbon

    Private Sub CallWeatherRibbon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load

    End Sub

    ''' <summary>
    ''' Get Weather method
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnGetWeather_Click(sender As Object, e As RibbonControlEventArgs) Handles btnGetWeather.Click
        If citybox.Text.Trim().Equals(String.Empty) OrElse countrybox.Text.Trim().Equals(String.Empty) Then
            MessageBox.Show("Please input the city or country name firstly.")
            Return
        End If

        ' Call web service to get Weather
        Globals.Sheet1.DisplayWebServiceResult(citybox.Text.Trim(), countrybox.Text.Trim())

    End Sub
End Class
