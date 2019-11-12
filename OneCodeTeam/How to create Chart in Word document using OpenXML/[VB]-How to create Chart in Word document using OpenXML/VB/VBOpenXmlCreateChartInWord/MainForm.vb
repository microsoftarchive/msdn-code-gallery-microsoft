'****************************** Module Header ******************************\
' Module Name:  MainForm.vb
' Project:      VBOpenXmlCreateChartInWord
' Copyright(c)  Microsoft Corporation.
' 
' The class is main Form.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.IO
Imports DocumentFormat.OpenXml.Drawing

Public Class MainForm

    Public Sub New()
        InitializeComponent()
        Me.btnCreateChart.Enabled = False
    End Sub
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Using saveDialog As New SaveFileDialog()
            saveDialog.Filter = "Word document (*.docx)|*.docx"
            saveDialog.InitialDirectory = Environment.CurrentDirectory
            saveDialog.RestoreDirectory = True
            saveDialog.DefaultExt = "docx"
            If saveDialog.ShowDialog() = DialogResult.OK Then
                Dim fs As FileStream = File.Create(saveDialog.FileName)
                tbxFile.Text = saveDialog.FileName
                Me.btnCreateChart.Enabled = True
                fs.Close()
            End If
        End Using
    End Sub

    Private Sub btnCreateChart_Click(sender As Object, e As EventArgs) Handles btnCreateChart.Click
        Dim createWordChart As CreateWordChart = Nothing
        Try
            createWordChart = New CreateWordChart(Me.tbxFile.Text)
            Dim chartAreas As New List(Of ChartSubArea)()
            chartAreas.Add(New ChartSubArea() With { _
                 .Color = SchemeColorValues.Accent1, _
                 .Label = "1st Qtr", _
                 .Value = "8.2" _
            })
            chartAreas.Add(New ChartSubArea() With { _
                 .Color = SchemeColorValues.Accent2, _
                 .Label = "2st Qtr", _
                 .Value = "3.2" _
            })
            chartAreas.Add(New ChartSubArea() With { _
                 .Color = SchemeColorValues.Accent3, _
                 .Label = "3st Qtr", _
                 .Value = "1.4" _
            })
            chartAreas.Add(New ChartSubArea() With { _
                 .Color = SchemeColorValues.Accent4, _
                 .Label = "4st Qtr", _
                 .Value = "1.2" _
            })
            createWordChart.CreateChart(chartAreas)
            MessageBox.Show("Create Chart successfully, you can check your document!")
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        Finally
            If createWordChart IsNot Nothing Then
                createWordChart.Dispose()
            End If
        End Try
    End Sub
End Class
