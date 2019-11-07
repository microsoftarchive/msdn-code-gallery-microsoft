'***************************** Module Header ******************************\
' Module Name:	Form1.vb
' Project:		VBWordDocCompare2012
' Copyright (c) Microsoft Corporation.
' 
' This code sample shows how to compare two word documents and show the difference.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports MsWord = Microsoft.Office.Interop.Word

Public Class Form1
    Inherits Form

#Region "Declare variables"

    Private wordApp As MsWord.Application = Nothing
    Private [readOnly] As Object = Nothing
    Private missing As Object = Nothing

    Private doc1 As MsWord.Document = Nothing
    Private doc2 As MsWord.Document = Nothing
    Private doc As MsWord.Document = Nothing

    Private documentFilter As String = "Microsoft Word Document (.doc)|.docx|All files (*.*)|*.*"

#End Region

    Public Sub New()
        InitializeComponent()

        ' Initialize the private variables
        wordApp = New MsWord.Application()
        [readOnly] = True
        missing = System.Reflection.Missing.Value
    End Sub

    ' Open first word document
    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        openFileDialog1.Filter = documentFilter
        Dim result1 As DialogResult = openFileDialog1.ShowDialog()
        If result1 = DialogResult.OK Then
            TextBox1.Text = openFileDialog1.FileName
            TextBox1.[ReadOnly] = True
            doc1 = wordApp.Documents.Open(TextBox1.Text, missing, [readOnly], missing, missing, missing, _
             missing, missing, missing, missing, missing, missing, _
             missing, missing, missing, missing)
        End If
    End Sub

    ' Open second word document
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        OpenFileDialog2.Filter = documentFilter
        Dim result2 As DialogResult = OpenFileDialog2.ShowDialog()
        If result2 = DialogResult.OK Then
            TextBox2.Text = OpenFileDialog2.FileName
            TextBox2.[ReadOnly] = True
            doc2 = wordApp.Documents.Open(TextBox2.Text, missing, [readOnly], missing, missing, missing, _
             missing, missing, missing, missing, missing, missing, _
             missing, missing, missing, missing)
        End If
    End Sub

    ' Compare the two word documents
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        doc = wordApp.CompareDocuments(doc1, doc2, MsWord.WdCompareDestination.wdCompareDestinationNew, MsWord.WdGranularity.wdGranularityWordLevel, True, True, _
                True, True, True, True, True, True, True, True, "", False)

        ' Close first document
        doc1.Close(missing, missing, missing)

        ' Close second document
        doc2.Close(missing, missing, missing)

        ' Show the compared document
        wordApp.Visible = True

        ' Quit the application
        Close()
    End Sub

    
End Class
