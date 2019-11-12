'/****************************** Module Header ******************************\
' Module Name: Sheet1.vb
' Project:     VBExcelCallWebService
' Copyright(c) Microsoft Corporation.
' 
' This Class calls web service to get information about the weather.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports VBExcelCallWebService.WeatherWebService
Imports System.Xml.Linq

Public Class Sheet1

    Private Sub Sheet1_Startup() Handles Me.Startup

        ' Add a new Name Range control to cell A1-A8
        ' Bind data to the NameRange control 
        Me.Controls.AddNamedRange(Me.Range("A1", "A8"), "Data")

    End Sub

    Private Sub Sheet1_Shutdown() Handles Me.Shutdown

    End Sub

    ''' <summary>
    '''  Call Web service and display the results to the NameRange control
    ''' </summary>
    ''' <param name="city">Search City</param>
    ''' <param name="country">Search Country</param>
    Public Sub DisplayWebServiceResult(city As String, country As String)
        ' Get Name Range and Clear current display
        Dim range As NamedRange = DirectCast(Me.Controls("Data"), NamedRange)
        range.Clear()

        ' Initialize the value of x 
        Dim x As Integer = 0

        Try
            ' Initialize a new instance of Service Client 
            Using weatherclien As New GlobalWeatherSoapClient()
                ' Call Web service method to Get Weather Data
                Dim xmlweatherresult As String = weatherclien.GetWeather(city, country)

                ' Load an XElement from a string that contains XML data
                Dim xmldata = XElement.Parse(xmlweatherresult)

                ' Query the Name and value of Weather
                Dim query = From weather In xmldata.Elements()
                            Select weather.Name, weather.Value

                If query.Count() > 0 Then
                    For Each item In query
                        ' Use RefersToR1C1 property to change the range that a NameRange control refers to
                        range.RefersToR1C1 = [String].Format("=R1C1:R{0}C2", query.Count())

                        ' Update data  in range.
                        ' Excel uses 1 as the base for index.
                        DirectCast(range.Cells(x + 1, 1), Excel.Range).Value2 = item.Name.ToString()
                        DirectCast(range.Cells(x + 1, 2), Excel.Range).Value2 = item.Value.ToString()
                        x += 1
                        If x = query.Count() - 1 Then
                            Exit For
                        End If
                    Next
                End If
            End Using
        Catch
            Me.Range("A1").Value2 = "Input City or Country is error, Please check them again"

            ' -16776961 is represent for red
            Me.Range("A1").Font.Color = -16776961
        End Try
    End Sub

End Class
