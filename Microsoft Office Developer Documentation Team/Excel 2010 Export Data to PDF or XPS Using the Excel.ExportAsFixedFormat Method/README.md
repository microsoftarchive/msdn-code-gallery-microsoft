# Excel 2010: Export Data to PDF or XPS Using the Excel.ExportAsFixedFormat Method
## License
- Apache License, Version 2.0
## Technologies
- Excel 2010
- Office 2010
## Topics
- Office 2010 101 code samples
- Range Object
## Updated
- 08/03/2011
## Description

<h1>Introduction</h1>
<p><span style="font-size:small">This sample shows how to use the <strong>ExportAsFixedFormat
</strong>method to export a range of data in a Microsoft Excel 2010 worksheet to PDF or XPS format.</span></p>
<p><span style="font-size:small">This code snippet is part of the Office 2010 101 code samples project. This sample, along with others, is offered here to incorporate directly in your code.</span></p>
<p><span style="font-size:small">Each code sample consists of approximately 5 to 50 lines of code demonstrating a distinct feature or feature set, in either VBA or both VB and C# (created in Visual Studio 2010). Each sample includes comments describing the
 sample, and setup code so that you can run the code with expected results or the comments will explain how to set up the environment so that the sample code runs.)</span></p>
<p><span style="font-size:small">Microsoft&reg; Office 2010 gives you the tools needed to create powerful applications. The Microsoft Visual Basic for Applications (VBA) code samples can assist you in creating your own applications that perform specific functions
 or as a starting point to create more complex solutions.</span></p>
<h1><span>Building the Sample</span></h1>
<p><span style="font-size:small">In Excel 2010, in a new workbook, copy all this code into the Sheet1 class module. Place the cursor in the TestExportAsFixedFormat procedure, and then press F8 to single-step through the code.</span></p>
<p><span style="font-size:20px; font-weight:bold">Description</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Sub</span>&nbsp;TestExportAsFixedFormat()&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;For&nbsp;information&nbsp;on&nbsp;the&nbsp;final&nbsp;parameter,&nbsp;see&nbsp;this&nbsp;page:</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;http://msdn.microsoft.com/en-us/library/aa338206.aspx</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;rng&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Range&nbsp;
&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;rng&nbsp;=&nbsp;Range(<span class="visualBasic__string">&quot;A1:E10&quot;</span>)&nbsp;
&nbsp;&nbsp;SetupRangeData&nbsp;rng&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;fileName&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Change&nbsp;this&nbsp;file&nbsp;name&nbsp;to&nbsp;meet&nbsp;your&nbsp;own&nbsp;needs:</span>&nbsp;
&nbsp;&nbsp;fileName&nbsp;=&nbsp;<span class="visualBasic__string">&quot;C:\Temp\Export.pdf&quot;</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Many&nbsp;of&nbsp;these&nbsp;properties&nbsp;are&nbsp;optional,&nbsp;and&nbsp;are&nbsp;included</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;here&nbsp;only&nbsp;to&nbsp;demonstrate&nbsp;how&nbsp;you&nbsp;might&nbsp;use&nbsp;them.&nbsp;The</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Type&nbsp;parameter&nbsp;can&nbsp;be&nbsp;one&nbsp;of&nbsp;xlTypePDF&nbsp;and&nbsp;xlTypeXLS;</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;the&nbsp;Quality&nbsp;parameter&nbsp;can&nbsp;be&nbsp;one&nbsp;of&nbsp;xlQualityStandard&nbsp;and</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;xlQualityMinimum.&nbsp;Setting&nbsp;the&nbsp;OpenAfterPublish&nbsp;property</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;to&nbsp;True&nbsp;will&nbsp;fail&nbsp;if&nbsp;you&nbsp;don't&nbsp;have&nbsp;a&nbsp;default&nbsp;viewer</span>&nbsp;
&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;installed&nbsp;and&nbsp;configured.</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;rng.ExportAsFixedFormat&nbsp;Type:=xlTypePDF,&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;fileName:=fileName,&nbsp;Quality:=xlQualityStandard,&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;IncludeDocProperties:=<span class="visualBasic__keyword">True</span>,&nbsp;IgnorePrintAreas:=<span class="visualBasic__keyword">True</span>,&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;From:=<span class="visualBasic__number">1</span>,&nbsp;<span class="visualBasic__keyword">To</span>:=<span class="visualBasic__number">1</span>,&nbsp;OpenAfterPublish:=<span class="visualBasic__keyword">True</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Sub</span>&nbsp;SetupRangeData(rng&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Range)&nbsp;
&nbsp;&nbsp;rng.Formula&nbsp;=&nbsp;<span class="visualBasic__string">&quot;=RANDBETWEEN(1,&nbsp;100)&quot;</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
</pre>
</div>
</div>
</div>
<h1><span>Source Code Files</span></h1>
<ul>
<li><span style="font-size:small"><em><a id="25885" href="/site/view/file/25885/1/Excel.ExportAsFixedFormat.txt">Excel.ExportAsFixedFormat.txt</a>- Download this sample only.</em></span>
</li><li><span style="font-size:small"><em><a id="25886" href="/site/view/file/25886/1/Office%202010%20101%20Code%20Samples.zip">Office 2010 101 Code Samples.zip</a>- Download all the samples.</em><em></em></span>
</li></ul>
<h1>More Information</h1>
<ul>
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/aa905411">Excel Developer Center on MSDN</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/hh360994">101 Code Samples for Office 2010 Developers</a></span>
</li></ul>
