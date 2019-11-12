'/****************************** Module Header ******************************\
' Module Name:  MainForm.vb
' Project:      VBOpenXmlExcelToXml
' Copyright(c)  Microsoft Corporation.
' 
' This is Main Form.
' Uers can manipulate the form to convert excel docuemnt to XML format string. 
' Then Show the xml in TextBox control
' Users also can save the xml string as xml file on client 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Imports System.IO


Public Class MainForm

    Public Sub New()
        InitializeComponent()
        Me.btnSaveAs.Enabled = False
    End Sub

    ''' <summary>
    '''  Open an dialog to let users select Excel file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnBrowser_Click(sender As Object, e As EventArgs) Handles btnBrowser.Click
        ' Initializes a OpenFileDialog instance 
        Using openfileDialog As New OpenFileDialog()
            openfileDialog.RestoreDirectory = True
            openfileDialog.Filter = "Excel files(*.xlsx;*.xls)|*.xlsx;*.xls"

            If openfileDialog.ShowDialog() = DialogResult.OK Then
                tbExcelName.Text = openfileDialog.FileName
            End If
        End Using
    End Sub

    ''' <summary>
    '''  Convert Excel file to Xml format and view in Listbox control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnConvert_Click(sender As Object, e As EventArgs) Handles btnConvert.Click
        tbXmlView.Clear()
        Dim excelfileName As String = tbExcelName.Text

        If String.IsNullOrEmpty(excelfileName) OrElse Not File.Exists(excelfileName) Then
            MessageBox.Show("The Excel file is invalid! Please select a valid file.")
            Return
        End If

        Try
            Dim xmlFormatstring As String = New ConvertExcelToXml().GetXML(excelfileName)
            If String.IsNullOrEmpty(xmlFormatstring) Then
                MessageBox.Show("The content of Excel file is Empty!")
                Return
            End If

            tbXmlView.Text = xmlFormatstring

            ' If txbXmlView has text, set btnSaveAs button to be enable
            btnSaveAs.Enabled = True
        Catch ex As Exception
            MessageBox.Show("Error occurs! The error message is: " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    '''  Save the XMl format string as Xml file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnSaveAs_Click(sender As Object, e As EventArgs) Handles btnSaveAs.Click
        ' Initializes a SaveFileDialog instance 
        Using savefiledialog As New SaveFileDialog()
            savefiledialog.RestoreDirectory = True
            savefiledialog.DefaultExt = "xml"
            savefiledialog.Filter = "All Files(*.xml)|*.xml"
            If savefiledialog.ShowDialog() = DialogResult.OK Then
                Dim filestream As Stream = savefiledialog.OpenFile()
                Dim streamwriter As New StreamWriter(filestream)
                streamwriter.Write("<?xml version='1.0'?>" + Environment.NewLine + tbXmlView.Text)
                streamwriter.Close()
            End If
        End Using
    End Sub

End Class
