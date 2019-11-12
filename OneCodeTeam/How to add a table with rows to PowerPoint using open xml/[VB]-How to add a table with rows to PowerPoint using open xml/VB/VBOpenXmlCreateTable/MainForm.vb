'/****************************** Module Header ******************************\
' Module Name:  MainForm.vb
' Project:      VBOpenXmlCreateTable
' Copyright(c)  Microsoft Corporation.
' 
' This is the main form of this application. 
' It is used to initialize the UI and handle the events.
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
Imports DocumentFormat.OpenXml.Packaging

Public Class MainForm

    Public Sub New()
        InitializeComponent()
        Me.btnInserTable.Enabled = False
    End Sub

    ''' <summary>
    ''' Handle the Click event of Open button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        SelectPowerPointFile()

        ' After select an existing PowerPoint file, make "Inser Table" button to be enable
        Me.btnInserTable.Enabled = True
    End Sub

    ''' <summary>
    ''' Show an OpenFileDialog to select a Word document.
    ''' </summary>
    ''' <returns>
    ''' The file name.
    ''' </returns>
    Private Function SelectPowerPointFile() As String
        Dim fileName As String = Nothing
        Using dialog As New OpenFileDialog()
            dialog.Filter = "PowerPoint document (*.pptx)|*.pptx"
            dialog.InitialDirectory = Environment.CurrentDirectory

            ' Retore the directory before closing
            dialog.RestoreDirectory = True
            If dialog.ShowDialog() = DialogResult.OK Then
                fileName = dialog.FileName
                Me.txbSource.Text = dialog.FileName
            End If
        End Using

        Return fileName
    End Function

    Private Sub btnInserTable_Click(sender As Object, e As EventArgs) Handles btnInserTable.Click
        Dim filePath As String = txbSource.Text.Trim()
        If String.IsNullOrEmpty(filePath) OrElse Not File.Exists(filePath) Then
            MessageBox.Show("The File is invalid,Please select an existing file again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' Open the source document as read/write
            ' In the Open XML SDK, the PresentationDocument class represents a presentation document package.
            ' To work with a presentation document, first create an instance of the Presentation Class
            ' and then work with the instance.
            ' To create the class instance from the document call the Open(string, boolean) method.
            Using presentationDocument__1 As PresentationDocument = PresentationDocument.Open(filePath, True)
                ' Start create table with rows in powerPoint document
                ' If create successfully, we can see a table in the last slide of the powerpoint
                InsertTableToPowerPoint.CreateTableInLastSlide(presentationDocument__1)
            End Using

            MessageBox.Show("Insert Table successfully")
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        End Try
    End Sub

End Class
