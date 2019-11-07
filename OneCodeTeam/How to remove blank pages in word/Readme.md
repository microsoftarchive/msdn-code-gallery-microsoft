# How to remove blank pages in word
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Office
- Office Development
## Topics
- Word
- Remove blank pages
## Updated
- 06/13/2013
## Description

<h1>How to Remove blank pages in Word document (<span style="">VB</span>WordRemoveBlankPage)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to remove blank pages in Word document automatically. Customers should choose a word document firstly, then click &quot;Remove Blank Page&quot; button to remove blank pages in word document.</p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before you run the sample, you should make sure Microsoft Office 2010 installed on your computer.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1. Open &quot;VBWordRemoveBlankPage<span style="">.sln</span>&quot; file to open the project and press F5 to run the project. The form resembles the following:</p>
<p class="MsoNormal"><span style=""><img src="84680-image.png" alt="" width="453" height="166" align="middle">
</span></p>
<p class="MsoNormal">Step2. Click &quot;Select&quot; button to choose a word document with blank pages from your file system. When you click &quot;Select&quot; button, there is an open file dialog to let you choose word document. After you choose an existing
 word document and click ok button in the open file dialog, you can see the path of word document that you selected show in textbox control.</p>
<p class="MsoNormal"><span style=""><img src="84681-image.png" alt="" width="450" height="163" align="middle">
</span></p>
<p class="MsoNormal">Step3. Click &quot;Remove Blank Page&quot; button to remove blank pages in word document.
<span style="">If the operation is successfully, you will receive the following form:
</span></p>
<p class="MsoNormal"><span style=""><img src="84682-image.png" alt="" width="507" height="174" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"></p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step1. Create Windows Forms application from the template in Visual Studio.</p>
<p class="MsoNormal">Step2. Add Microsoft.Office.Interop<span style="">.Word.dll assemble and add the
</span>Microsoft.Office.Interop<span style="">.Word namespace</span> into codes.</p>
<p class="MsoNormal">Step3. Coding the click event of &quot;Select&quot; button.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
''' &lt;summary&gt;
    ''' Open word document
    ''' &lt;/summary&gt;
    ''' &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
    ''' &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
    Private Sub btnOpenWord_Click(sender As Object, e As EventArgs) Handles btnOpenWord.Click
        Using openfileDialog As New OpenFileDialog()
            openfileDialog.Filter = &quot;Word document(*.doc,*.docx)|*.doc;*.docx&quot;
            If openfileDialog.ShowDialog() = DialogResult.OK Then
                txbWordPath.Text = openfileDialog.FileName
                wordPath = openfileDialog.FileName
            End If
        End Using
    End Sub

</pre>
<pre id="codePreview" class="vb">
''' &lt;summary&gt;
    ''' Open word document
    ''' &lt;/summary&gt;
    ''' &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
    ''' &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
    Private Sub btnOpenWord_Click(sender As Object, e As EventArgs) Handles btnOpenWord.Click
        Using openfileDialog As New OpenFileDialog()
            openfileDialog.Filter = &quot;Word document(*.doc,*.docx)|*.doc;*.docx&quot;
            If openfileDialog.ShowDialog() = DialogResult.OK Then
                txbWordPath.Text = openfileDialog.FileName
                wordPath = openfileDialog.FileName
            End If
        End Using
    End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal">Step4. Coding the click event of &quot;Remove Blank Page&quot; button.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
''' &lt;summary&gt;
   ''' Click event of Remove Blank Page button
   ''' &lt;/summary&gt;
   ''' &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
   ''' &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
   Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
       If Not File.Exists(txbWordPath.Text) Then
           MessageBox.Show(&quot;Please Select valid path of word document!&quot;, &quot;Error&quot;, MessageBoxButtons.OK, MessageBoxIcon.[Error])
           Return
       End If


       ' Remove blank Page in word document
       If RemoveBlankPage() = True Then
           MessageBox.Show(&quot;Remove blank page successfully!&quot;)
       End If
   End Sub


   ''' &lt;summary&gt;
   ''' Remove Blank Page in Word document
   ''' &lt;/summary&gt;
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
           MessageBox.Show(&quot;Exception Occur, error message is: &quot; & ex.Message)
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

</pre>
<pre id="codePreview" class="vb">
''' &lt;summary&gt;
   ''' Click event of Remove Blank Page button
   ''' &lt;/summary&gt;
   ''' &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
   ''' &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
   Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
       If Not File.Exists(txbWordPath.Text) Then
           MessageBox.Show(&quot;Please Select valid path of word document!&quot;, &quot;Error&quot;, MessageBoxButtons.OK, MessageBoxIcon.[Error])
           Return
       End If


       ' Remove blank Page in word document
       If RemoveBlankPage() = True Then
           MessageBox.Show(&quot;Remove blank page successfully!&quot;)
       End If
   End Sub


   ''' &lt;summary&gt;
   ''' Remove Blank Page in Word document
   ''' &lt;/summary&gt;
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
           MessageBox.Show(&quot;Exception Occur, error message is: &quot; & ex.Message)
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

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><span style=""><span style="">&nbsp;</span>Office Dev Center
</span></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/office/aa905340">http://msdn.microsoft.com/en-us/office/aa905340</a>
</p>
<p class="MsoNormal">Word Object Model Overview</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/kw65a0we(v=VS.80).aspx">http://msdn.microsoft.com/en-us/library/kw65a0we(v=VS.80).aspx</a>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
