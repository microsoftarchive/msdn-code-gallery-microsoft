# Extract embedded files from Office documents (CSOfficeDocumentFileExtractor)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Office
## Topics
- Office Development
## Updated
- 09/11/2012
## Description

<h1>Extract Embedded File from Office 2007 format file (CSOfficeDocumentFileExtrator)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">Office documents can embed other files into it. However, the object model does not expose a way to extract the embedded files from the document. This sample demonstrates how to extract the embedded files from an Office 2007 format file.</p>
<p class="MsoNormal">The files embedded inside the Office 2007 format file is located under /&lt;application/embeddings/ folder. Using the System.IO.Packaging classes, we can extract the files.</p>
<p class="MsoNormal">If the embedded file is an Office 2007 format file like a Word document or an Excel workbook, it will be stored as such. However, other files will be stored in Structured Storage format with the name oleObjectX.bin. Most of the files
 will be stored as Ole10Native format. The Ole10Native file format has the following structure.</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>First 4 Bytes &ndash; Unknown</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Next 2 Bytes &ndash; Usually 2 (02 00)</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>From 7<sup>th</sup> Byte, the name of the embedded file starts.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The original full path of the embedded file starts after that. Scan the path till null character.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Next 4 bytes are unknown</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Next 4 bytes represents the length of the temporary file path before it got inserted to the document. This will be in little endian format and we need to convert it.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The temporary file path starts after that. We can either skip this using the length retrieved or scan the path till null character.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Next 4 bytes represents the size of the embedded file in little endian format. We need to convert it.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The actual file contents starts from here. Read the file till the length retrieved previously.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The next 4 bytes gives the length of the temporary location of the file in Unicode.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Temporary location of the file in Unicode starts from here.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Finally the source file path in Unicode starts.</p>
<p class="MsoNormal">The sample uses Structured Storage APIs. Most of the Interfaces, Classes and Enumerations are defined in the System.Runtime.InteropServices.ComTypes namespace. A few like the ones given below are defined/declared in Ole10Native.cs file.</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span><span>1)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>IEnumSTATSTG &ndash; Interface</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>2)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>STATFLAG &ndash; Enumeration</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>3)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>IStorage &ndash; Interface</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>4)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>STGM &ndash; Enumeration</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>5)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>StgIsStorageFile &ndash; Method (Ole32.dll)</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>6)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>StgOpenStorage &ndash; Method (Ole32.dll)</p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span><img src="65255-image.png" alt="" width="624" height="549" align="middle">
</span></p>
<p class="MsoNormal">Steps to run the sample</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span><span>1)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Enter the path of the file which contains the embedded files. Optionally, one can use browse button beside the text box to select the file from the system.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>2)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>After entering the file path, click on Scan button to check which files are embedded inside the document.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>3)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Select the files to be extracted.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>4)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Give the path of the folder which stores the extracted contents. By default, the Extract to path points to C:\Temp. Optionally on can use the browse button beside the text box to select the file from the system.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>5)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Click on Extract Selected Files button to extract the embedded objects from the document</p>
<h2>Using the Code</h2>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">The following code checks for the file type and then retrieve the names of the embedded files.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">Package pkg = Package.Open(fileName);


System.IO.FileInfo fi = new System.IO.FileInfo(fileName);


string extension = fi.Extension.ToLower();


if ((extension == &quot;.docx&quot;) || (extension == &quot;.dotx&quot;) || (extension == &quot;.docm&quot;) || (extension == &quot;.dotm&quot;))
 {
&nbsp;&nbsp;&nbsp;&nbsp; embeddingPartString = &quot;/word/embeddings/&quot;;
 }
 else if ((extension == &quot;.xlsx&quot;) || (extension == &quot;.xlsm&quot;) || (extension == &quot;.xltx&quot;) || (extension == &quot;.xltm&quot;))
&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; embeddingPartString = &quot;/excel/embeddings/&quot;;
&nbsp; }
&nbsp; else
&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; embeddingPartString = &quot;/ppt/embeddings/&quot;;
&nbsp; }


&nbsp; // Get the embedded files names.
&nbsp; foreach(PackagePart pkgPart in pkg.GetParts())
&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (pkgPart.Uri.ToString().StartsWith(embeddingPartString))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string fileName1 = pkgPart.Uri.ToString().Remove(0, embeddingPartString.Length);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; chkdLstEmbeddedFiles.Items.Add(fileName1);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp; }
&nbsp; pkg.Close();

</pre>
<pre id="codePreview" class="csharp">Package pkg = Package.Open(fileName);


System.IO.FileInfo fi = new System.IO.FileInfo(fileName);


string extension = fi.Extension.ToLower();


if ((extension == &quot;.docx&quot;) || (extension == &quot;.dotx&quot;) || (extension == &quot;.docm&quot;) || (extension == &quot;.dotm&quot;))
 {
&nbsp;&nbsp;&nbsp;&nbsp; embeddingPartString = &quot;/word/embeddings/&quot;;
 }
 else if ((extension == &quot;.xlsx&quot;) || (extension == &quot;.xlsm&quot;) || (extension == &quot;.xltx&quot;) || (extension == &quot;.xltm&quot;))
&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; embeddingPartString = &quot;/excel/embeddings/&quot;;
&nbsp; }
&nbsp; else
&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; embeddingPartString = &quot;/ppt/embeddings/&quot;;
&nbsp; }


&nbsp; // Get the embedded files names.
&nbsp; foreach(PackagePart pkgPart in pkg.GetParts())
&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (pkgPart.Uri.ToString().StartsWith(embeddingPartString))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string fileName1 = pkgPart.Uri.ToString().Remove(0, embeddingPartString.Length);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; chkdLstEmbeddedFiles.Items.Add(fileName1);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp; }
&nbsp; pkg.Close();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
