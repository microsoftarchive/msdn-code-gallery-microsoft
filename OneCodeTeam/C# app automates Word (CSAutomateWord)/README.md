# C# app automates Word (CSAutomateWord)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Office
## Topics
- Automation
- Word
## Updated
- 03/01/2012
## Description

<h1><span style="font-family:������">CONSOLE APPLICATION</span> (<span style="font-family:������">CSAutomateWord</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The <span class="SpellE"><span style="">CS</span>AutomateWord</span> example demonstrates the use of
<span style="">C#</span> codes to create a Microsoft Word instance, create a new document, insert a paragraph and a table, save the document, close the Microsoft Word application and then clean up unmanaged COM resources.</p>
<p class="MsoNormal">Office automation is based on Component Object Model (COM). When you call a COM object of Office from managed code, a Runtime Callable Wrapper (RCW) is automatically created. The RCW marshals calls between the .NET application and the
 COM object. The RCW keeps a reference count on the COM object. If all references have not been released on the RCW, the COM object of Office
</p>
<p class="MsoNormal"><span class="GramE">does</span> not quit and may cause the Office application not to quit after your automation. In order to make sure that the Office application quits cleanly, the sample demonstrates two solutions.</p>
<p class="MsoNormal">Solution1.AutomateWord demonstrates automating Microsoft Word application by using Microsoft Word Primary Interop Assembly (PIA) and explicitly assigning each COM accessor object to a new variable that you would explicitly call
<span class="SpellE">Marshal.FinalReleaseComObject</span> to release it at the end.</p>
<p class="MsoNormal">Solution2.AutomateWord demonstrates automating Microsoft Word application by using Microsoft Word PIA and forcing a garbage collection as soon as the automation function is off the stack (at which point the RCW objects are no longer rooted)
 to clean up RCWs and release COM objects.<span style="">&nbsp; </span></p>
<h2>Running the Sample </h2>
<p class="MsoNormal">The following steps walk through a demonstration of the Word automation sample that starts a Microsoft Word instance, creates a new document, inserts a paragraph and a table, saves the document, and quits the Microsoft Word application
 cleanly.</p>
<p class="MsoNormal">Step1. After you successfully build the sample project in Visual Studio 2010, you will get the application: CSAutomateWord.exe.</p>
<p class="MsoNormal">Step2. Open Windows Task Manager (Ctrl&#43;Shift&#43;Esc) to confirm that no winword.exe is running.
</p>
<p class="MsoNormal">Step3. Run the application. It should print the following content in the console window if no error is thrown.</p>
<p class="MsoNormal"><span style=""><img src="53046-image.png" alt="" width="576" height="376" align="middle">
</span></p>
<p class="MsoNormal">Then, you will see two new documents in the directory of the application:
</p>
<p class="MsoNormal">Sample1.docx and Sample2.docx. Both documents have the following content.</p>
<p class="MsoNormal"><span style=""><img src="53047-image.png" alt="" width="576" height="394" align="middle">
</span></p>
<p class="MsoNormal">Sample2.docx additionally has this table in the document.</p>
<p class="MsoNormal"><span style=""><img src="53048-image.png" alt="" width="576" height="531" align="middle">
</span></p>
<p class="MsoNormal">Step4. In Windows Task Manager, confirm that the winword.exe process does not exist, i.e. the Microsoft Word intance was closed and cleaned up properly.</p>
<h2><span style="">Using the code </span></h2>
<p class="MsoNormal">Step1. Create a Console application and reference the Word Primary
<span class="SpellE">Interop</span> Assembly (PIA). To reference the Word PIA, right-click the project
<span class="GramE">file</span><span style=""> </span>and click the &quot;Add Reference...&quot; button. In the Add Reference dialog, navigate to the .NET tab, find <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/Microsoft.Office.Interop.Word.aspx" target="_blank" title="Auto generated link to Microsoft.Office.Interop.Word">Microsoft.Office.Interop.Word</a> 12.0.0.0 and click OK.</p>
<p class="MsoNormal">Step2. Import and rename the Excel interop namepace:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
using Word = <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/Microsoft.Office.Interop.Word.aspx" target="_blank" title="Auto generated link to Microsoft.Office.Interop.Word">Microsoft.Office.Interop.Word</a>;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step3. Start up a Word application by creating a Word.Application object.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
oWord = new Word.Application();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step4. Get the Documents collection from Application.Documents and call its Add function to create a new document. The Add function returns a Document object.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
// Create a new Document and add it to document collection.               
oDoc = oWord.Documents.Add(ref missing, ref missing, ref missing, ref missing);
Console.WriteLine(&quot;A new document is created&quot;);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step5. Insert a paragraph.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
    oParas = oDoc.Paragraphs;
    oPara = oParas.Add(ref missing);
    oParaRng = oPara.Range;
    oParaRng.Text = &quot;Heading 1&quot;;
    oFont = oParaRng.Font;
    oFont.Bold = 1;
    oParaRng.InsertParagraphAfter();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step6. Insert a table.</p>
<p class="MsoNormal">The following code has the problem that it invokes accessors which will also create RCWs and reference them. For example, calling Document.Bookmarks.Item creates an RCW for the Bookmarks object. If you invoke these accessors via tunneling
 as this code does, the RCWs are created on the GC heap, but the references are created under the hood on the stack and are then discarded. As such, there is no way to call MarshalFinalReleaseComObject on those RCWs. To get them to release, you would either
 need to force a garbage collection as soon as the calling function is off the stack (at which point these objects are no longer rooted) and then call GC.WaitForPendingFinalizers, or you would need to change the syntax to explicitly assign these accessor objects
 to a variable that you would then explicitly call Marshal.FinalReleaseComObject on.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
oBookmarkRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
    oTable = oDoc.Tables.Add(oBookmarkRng, 5, 2, ref missing, ref missing);
    oTable.Range.ParagraphFormat.SpaceAfter = 6;
    for (int r = 1; r &lt;= 5; r&#43;&#43;)
    {
        for (int c = 1; c &lt;= 2; c&#43;&#43;)
        {
            oTable.Cell(r, c).Range.Text = &quot;r&quot; &#43; r &#43; &quot;c&quot; &#43; c;
        }
    }


    // Change width of columns 1 & 2
    oTable.Columns[1].Width = oWord.InchesToPoints(2);
    oTable.Columns[2].Width = oWord.InchesToPoints(3);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step7. Save the document as a docx file and close it.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
object fileName = Path.GetDirectoryName(
  Assembly.GetExecutingAssembly().Location) &#43; &quot;\\Sample1.docx&quot;;
    object fileFormat = Word.WdSaveFormat.wdFormatXMLDocument;
    oDoc.SaveAs(ref fileName, ref fileFormat, ref missing,
  ref missing, ref missing, ref missing, ref missing,
  ref missing, ref missing, ref missing, ref missing,
  ref missing, ref missing, ref missing, ref missing,
  ref missing);
    ((Word._Document)oDoc).Close(ref missing, ref missing, ref missing);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step8. Quit the Word application.<span style=""> </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
((Word._Application)oWord).Quit(ref notTrue, ref missing, ref missing);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step9. Clean up the unmanaged COM resource. To get Word terminated rightly, we need to call Marshal.FinalReleaseComObject() on each COM object we used.We can either explicitly call Marshal.FinalReleaseComObject on all accessor objects:</p>
<p class="MsoNormal">and/or force a garbage collection as soon as the calling function is off the stack (at which point these objects are no longer rooted) and then call GC.WaitForPendingFinalizers.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
// See Solution2.AutomateWord
GC.Collect();
GC.WaitForPendingFinalizers();
// GC needs to be called twice in order to get the Finalizers called 
// - the first time in, it simply makes a list of what is to be 
// finalized, the second time in, it actually is finalizing. Only 
// then will the object do its automatic ReleaseComObject.
GC.Collect();
GC.WaitForPendingFinalizers();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/bb244391.aspx">MSDN: Word 2007 Developer Reference</a></p>
<p class="MsoNormal"><a href="http://support.microsoft.com/kb/316384">How to automate Microsoft Word to create a new document by using Visual C#</a></p>
<p class="MsoNormal"><a href="http://blogs.msdn.com/geoffda/archive/2007/09/07/the-designer-process-that-would-not-terminate-part-2.aspx">The Designer Process That Would Not Terminate (Part 2)</a>
<b></b></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
