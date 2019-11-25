'**************************** Module Header ******************************\ 
' Module Name:   MainForm.vb
' Project:       VBWordRemoveBlankPage
' Copyright (c)  Microsoft Corporation. 
'  
' The Class is Main Form
' Customers can manipulate the form to remove blank page of word document
'  
' This source is subject to the Microsoft Public License. 
' See http://www.microsoft.com/en-us/openness/licenses.aspx. 
' All other rights reserved. 
'  
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED  
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
'**************************************************************************/


Imports System.IO
Imports System.Runtime.InteropServices
Imports Word = Microsoft.Office.Interop.Word

Public Class MainForm

    'the path of word document
    Private wordPath As String = Nothing

    ''' <summary>
    ''' Open word document
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnOpenWord_Click(sender As Object, e As EventArgs) Handles btnOpenWord.Click
        Using openfileDialog As New OpenFileDialog()
            openfileDialog.Filter = "Word document(*.doc,*.docx)|*.doc;*.docx"
            If openfileDialog.ShowDialog() = DialogResult.OK Then
                txbWordPath.Text = openfileDialog.FileName
                wordPath = openfileDialog.FileName
            End If
        End Using
    End Sub

    ''' <summary>
    ''' Click event of Remove Blank Page button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        If Not File.Exists(txbWordPath.Text) Then
            MessageBox.Show("Please Select valid path of word document!", "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return
        End If

        ' Remove blank Page in word document
        If RemoveBlankPage() = True Then
            MessageBox.Show("Remove blank page successfully!")
        End If
    End Sub

    ''' <summary>
    ''' Remove Blank Page in Word document
    ''' </summary>
    Private Function RemoveBlankPage() As Boolean
        Dim wordapp As Word.Application = Nothing
        Dim doc As Word.Document = Nothing
        Dim paragraphs As Word.Paragraphs = Nothing
        Try
            ' Start Word APllication and set it be invisible
            wordapp = New Word.Application()
            wordapp.Visible = False
            doc = wordapp.Documents.Open(wordPath)
            paragraphs = doc.Paragraphs
            For Each paragraph As Word.Paragraph In paragraphs
                If paragraph.Range.Text.Trim() = String.Empty Then
                    paragraph.Range.[Select]()
                    wordapp.Selection.Delete()
                End If
            Next

            ' Save the document and close document
            doc.Save()
            DirectCast(doc, Word._Document).Close()

            ' Quit the word application

            DirectCast(wordapp, Word._Application).Quit()
        Catch ex As Exception
            MessageBox.Show("Exception Occur, error message is: " & ex.Message)
            Return False
        Finally
            ' Clean up the unmanaged Word COM resources by explicitly
            ' call Marshal.FinalReleaseComObject on all accessor objects
            If paragraphs IsNot Nothing Then
                Marshal.FinalReleaseComObject(paragraphs)
                paragraphs = Nothing
            End If
            If doc IsNot Nothing Then
                Marshal.FinalReleaseComObject(doc)
                doc = Nothing
            End If
            If wordapp IsNot Nothing Then
                Marshal.FinalReleaseComObject(wordapp)
                wordapp = Nothing
            End If
        End Try

        Return True
    End Function

End Class
