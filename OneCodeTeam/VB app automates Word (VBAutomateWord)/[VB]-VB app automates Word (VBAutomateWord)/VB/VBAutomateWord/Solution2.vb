'****************************** Module Header ******************************'
' Module Name:  Solution2.vb
' Project:      VBAutomateWord
' Copyright (c) Microsoft Corporation.
' 
' Solution2.AutomateWord demonstrates automating Microsoft Word application 
' by using Microsoft Word Primary Interop Assembly (PIA) and forcing a 
' garbage collection as soon as the automation function is off the stack (at 
' which point the Runtime Callable Wrapper (RCW) objects are no longer 
' rooted) to clean up RCWs and release COM objects.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Import directives"

Imports System.Reflection
Imports System.IO

Imports Word = Microsoft.Office.Interop.Word

#End Region


Class Solution2

    Public Shared Sub AutomateWord()

        AutomateWordImpl()


        ' Clean up the unmanaged Word COM resources by forcing a garbage 
        ' collection as soon as the calling function is off the stack (at 
        ' which point these objects are no longer rooted).

        GC.Collect()
        GC.WaitForPendingFinalizers()
        ' GC needs to be called twice in order to get the Finalizers called 
        ' - the first time in, it simply makes a list of what is to be 
        ' finalized, the second time in, it actually the finalizing. Only 
        ' then will the object do its automatic ReleaseComObject.
        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub


    Private Shared Sub AutomateWordImpl()

        Try
            ' Create an instance of Microsoft Word and make it invisible.
            Dim oWord As New Word.Application
            oWord.Visible = False
            Console.WriteLine("Word.Application is started")

            ' Create a new Document and add it to document collection.
            Dim oDoc As Word.Document = oWord.Documents.Add()
            Console.WriteLine("A new document is created")

            ' Insert a paragraph.
            Console.WriteLine("Insert a paragraph")

            Dim oPara As Word.Paragraph = oDoc.Paragraphs.Add()
            oPara.Range.Text = "Heading 1"
            oPara.Range.Font.Bold = 1
            oPara.Range.InsertParagraphAfter()

            ' Insert a table.
            Console.WriteLine("Insert a table")

            Dim oBookmarkRng As Word.Range = oDoc.Bookmarks.Item("\endofdoc").Range

            Dim oTable As Word.Table = oDoc.Tables.Add(oBookmarkRng, 5, 2)
            oTable.Range.ParagraphFormat.SpaceAfter = 6

            For r As Integer = 1 To 5
                For c As Integer = 1 To 2
                    oTable.Cell(r, c).Range.Text = "r" & r & "c" & c
                Next
            Next

            ' Change width of columns 1 & 2
            oTable.Columns(1).Width = oWord.InchesToPoints(2)
            oTable.Columns(2).Width = oWord.InchesToPoints(3)

            ' Save the document as a docx file and close it.
            Console.WriteLine("Save and close the document")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample2.docx"

            ' Saves the document with a new name or format. 
            ' Some of the arguments for this method correspond to 
            ' the options in the Save As dialog box. 
            ' For details,please refer to
            ' :http://msdn.microsoft.com/en-us/library/microsoft.office.tools.word.document.saveas(VS.80).aspx
            oDoc.SaveAs(fileName, Word.WdSaveFormat.wdFormatXMLDocument)
            oDoc.Close()

            ' Quit the Word application.
            Console.WriteLine("Quit the Word application")
            oWord.Quit(False)

        Catch ex As Exception
            Console.WriteLine("Solution2.AutomateWord throws the error: {0}", _
                              ex.Message)
        End Try

    End Sub

End Class
