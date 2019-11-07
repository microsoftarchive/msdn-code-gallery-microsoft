# How to compare two Word document and show the difference
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Office
- Office Development
## Topics
- Compare word document
- show difference
## Updated
- 09/21/2016
## Description

<h1><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em><a href="http://blogs.msdn.com/b/onecode"></a></h1>
<h1>The project illustrates how to compare two Word document and show the difference&nbsp;</h1>
<h2>Introduction</h2>
<p>The project illustrates how we can compare two Word documents and show the difference between them.</p>
<p>Lots of developers ask about this in the MSDN forums, so we created the code sample to address the frequently asked programming scenario.</p>
<p><a href="http://social.msdn.microsoft.com/Forums/en-US/os_binaryfile/thread/6e50e43c-3d83-4d6c-b0d7-ed42818b134b">http://social.msdn.microsoft.com/Forums/en-US/os_binaryfile/thread/6e50e43c-3d83-4d6c-b0d7-ed42818b134b</a></p>
<p><a href="http://social.msdn.microsoft.com/forums/en-us/vsto/thread/7660C6FD-9CC0-4423-A37C-3559F72DBB99">http://social.msdn.microsoft.com/forums/en-us/vsto/thread/7660C6FD-9CC0-4423-A37C-3559F72DBB99</a></p>
<p><a href="http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/d6068d81-e7cb-4894-8aec-27c6bef932d8">http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/d6068d81-e7cb-4894-8aec-27c6bef932d8</a></p>
<p>&nbsp;</p>
<h2>Building the Project</h2>
<ul>
<li><strong>Creating a Windows Form Application (CSWordDocCompare2010)</strong> </li></ul>
<ol>
<li>In the Visual Studio 2012 create a Windows Form Application </li><li>We are using Microsoft.Office.Interop.Word.dll with version 12.0.0.0 </li><li>It can compare MS word documents only among .doc/.docx files only </li><li>In this example, considered two word documents, and attaching for reference </li></ol>
<h1>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">// read 1st document
doc1 = wordApp.Documents.Open(textBox1.Text, missing, readOnly, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);

// read 2nd document
doc2 = wordApp.Documents.Open(textBox2.Text, missing, readOnly, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);

// compare the two documents
doc = wordApp.CompareDocuments(doc1, doc2, MsWord.WdCompareDestination.wdCompareDestinationNew,           MsWord.WdGranularity.wdGranularityWordLevel, true, true, true, true, true, true, true, true, true, true, &quot;&quot;, false);
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__com">//&nbsp;read&nbsp;1st&nbsp;document</span>&nbsp;
doc1&nbsp;=&nbsp;wordApp.Documents.Open(textBox1.Text,&nbsp;missing,&nbsp;readOnly,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing);&nbsp;
&nbsp;
<span class="cs__com">//&nbsp;read&nbsp;2nd&nbsp;document</span>&nbsp;
doc2&nbsp;=&nbsp;wordApp.Documents.Open(textBox2.Text,&nbsp;missing,&nbsp;readOnly,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing,&nbsp;missing);&nbsp;
&nbsp;
<span class="cs__com">//&nbsp;compare&nbsp;the&nbsp;two&nbsp;documents</span>&nbsp;
doc&nbsp;=&nbsp;wordApp.CompareDocuments(doc1,&nbsp;doc2,&nbsp;MsWord.WdCompareDestination.wdCompareDestinationNew,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MsWord.WdGranularity.wdGranularityWordLevel,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__string">&quot;&quot;</span>,&nbsp;<span class="cs__keyword">false</span>);&nbsp;
</pre>
</div>
</div>
</div>
</h1>
<h2>Running the Sample</h2>
<p>&nbsp; &nbsp; 1. Run the application</p>
<p><img id="124488" src="124488-1.png" alt="" width="347" height="227"></p>
<p>&nbsp; &nbsp; 2. Click #1 button</p>
<p>Select your 1<sup>st</sup>&nbsp;document via the dialog box</p>
<p>&nbsp; &nbsp; 3. Click #2 button</p>
<p>Select your 2<sup>nd</sup>&nbsp;document via the dialog box</p>
<p><img id="124489" src="124489-2.png" alt="" width="350" height="227"></p>
<p>&nbsp; &nbsp; 4. Click Compare! Button</p>
<p>&nbsp; &nbsp; 5. Application will vanish</p>
<p>&nbsp; &nbsp; 6. You can see the compared version opened now</p>
<p><img id="124490" src="124490-3.png" alt="" width="957" height="725"></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640"><img src="-onecodelogo" alt=""></a></div>
