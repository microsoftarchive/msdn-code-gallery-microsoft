'****************************** Module Header ******************************'
' Module Name:  Solution1.vb
' Project:      VBAutomateWord
' Copyright (c) Microsoft Corporation.
' 
' Solution1.AutomateWord demonstrates automating Microsoft Word application 
' by using Microsoft Word Primary Interop Assembly (PIA) and explicitly 
' assigning each COM accessor object to a new variable that you would 
' explicitly call Marshal.FinalReleaseComObject to release it at the end. 
' When you use this solution, it is important to avoid calls that tunnel into 
' the object model because they will orphan Runtime Callable Wrapper (RCW) on 
' the heap that you will not be able to access in order to call 
' Marshal.ReleaseComObject. You need to be very careful. For example, 
' 
'   Dim oDoc As Word.Document = oWord.Documents.Add()
'  
' Calling oWord.Documents.Add creates an RCW for the Documents object. If you 
' invoke these accessors via tunneling as this code does, the RCW for 
' Documents is created on the GC heap, but the reference is created under the 
' hood on the stack and are then discarded. As such, there is no way to call 
' MarshalFinalReleaseComObject on this RCW. To get such kind of RCWs released, 
' you would either need to force a garbage collection as soon as the calling 
' function is off the stack (see Solution2.AutomateWord), or you would need 
' to explicitly assign each accessor object to a variable and free it.
' 
'   Dim oDocs As Word.Documents = oWord.Documents
'   Dim oDoc As Word.Document = oDocs.Add()
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Reflection
Imports System.IO

Imports Word = Microsoft.Office.Interop.Word
Imports System.Runtime.InteropServices

#End Region


Class Solution1

    Public Shared Sub AutomateWord()

        Dim oWord As Word.Application = Nothing
        Dim oDocs As Word.Documents = Nothing
        Dim oDoc As Word.Document = Nothing
        Dim oParas As Word.Paragraphs = Nothing
        Dim oPara As Word.Paragraph = Nothing
        Dim oParaRng As Word.Range = Nothing
        Dim oFont As Word.Font = Nothing

        Try
            ' Create an instance of Microsoft Word and make it invisible.
            oWord = New Word.Application()
            oWord.Visible = False
            Console.WriteLine("Word.Application is started")

            ' Create a new Document and add it to document collection.
            oDoc = oWord.Documents.Add()
            Console.WriteLine("A new document is created")

            ' Insert a paragraph.
            Console.WriteLine("Insert a paragraph")

            oParas = oDoc.Paragraphs
            oPara = oParas.Add()
            oParaRng = oPara.Range
            oParaRng.Text = "Heading 1"
            oFont = oParaRng.Font
            oFont.Bold = 1
            oParaRng.InsertParagraphAfter()

            ' Save the document as a docx file and close it.
            Console.WriteLine("Save and close the document")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample1.docx"

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
            Console.WriteLine("Solution1.AutomateWord throws the error: {0}", _
                              ex.Message)
        Finally

            ' Clean up the unmanaged Excel COM resources by explicitly call 
            ' Marshal.FinalReleaseComObject on all accessor objects. 
            ' See http://support.microsoft.com/kb/317109.

            If Not oFont Is Nothing Then
                Marshal.FinalReleaseComObject(oFont)
                oFont = Nothing
            End If
            If Not oParaRng Is Nothing Then
                Marshal.FinalReleaseComObject(oParaRng)
                oParaRng = Nothing
            End If
            If Not oPara Is Nothing Then
                Marshal.FinalReleaseComObject(oPara)
                oPara = Nothing
            End If
            If Not oParas Is Nothing Then
                Marshal.FinalReleaseComObject(oParas)
                oParas = Nothing
            End If
            If Not oDoc Is Nothing Then
                Marshal.FinalReleaseComObject(oDoc)
                oDoc = Nothing
            End If
            If Not oDocs Is Nothing Then
                Marshal.FinalReleaseComObject(oDocs)
                oDocs = Nothing
            End If
            If Not oWord Is Nothing Then
                Marshal.FinalReleaseComObject(oWord)
                oWord = Nothing
            End If

        End Try

    End Sub

End Class