# Excel 2010: Sort and Filter Programmatically Using Excel.ListObjectSortFilter
## License
- Apache License, Version 2.0
## Technologies
- Excel 2010
- Office 2010
## Topics
- Sorting
- Office 2010 101 code samples
## Updated
- 08/03/2011
## Description

<h1>Introduction</h1>
<p><span style="font-size:small">This sample shows how to use the <strong>ListObject
</strong>object to sort and filter a table programmatically in a Microsoft Excel 2010 workbook.</span></p>
<p><span style="font-size:small">This code snippet is part of the Office 2010 101 code samples project. This sample, along with others, is offered here to incorporate directly in your code.</span></p>
<p><span style="font-size:small">Each code sample consists of approximately 5 to 50 lines of code demonstrating a distinct feature or feature set, in either VBA or both VB and C# (created in Visual Studio 2010). Each sample includes comments describing the
 sample, and setup code so that you can run the code with expected results or the comments will explain how to set up the environment so that the sample code runs.)</span></p>
<p><span style="font-size:small">Microsoft&reg; Office 2010 gives you the tools needed to create powerful applications. The Microsoft Visual Basic for Applications (VBA) code samples can assist you in creating your own applications that perform specific functions
 or as a starting point to create more complex solutions.</span></p>
<h1><span>Building the Sample</span></h1>
<p><span style="font-size:small">In Excel 2010, in a new workbook, copy all this code into the Sheet1 class module. Place the cursor in the ListObjectSortFilterDemo procedure, and then press F8 to start debugging, then Shift&#43;F8 to single-step through the code
 (stepping over any called procedures). Arrange the VBA and Excel windows side by side so you can see the results of running the code.</span></p>
<p><span style="font-size:20px; font-weight:bold">Description</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<div class="preview">
<pre class="vb"><span class="visualBasic__com">'&nbsp;Work&nbsp;with&nbsp;ListObject&nbsp;members:</span>&nbsp;
<span class="visualBasic__com">'&nbsp;&nbsp;Sort</span>&nbsp;
<span class="visualBasic__com">'&nbsp;&nbsp;AutoFilter</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Sub</span>&nbsp;ListObjectSortFilterDemo()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Step&nbsp;over&nbsp;this&nbsp;procedure.&nbsp;It's&nbsp;not&nbsp;terribly&nbsp;interesting.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;FillRandomData&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;lo&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;ListObject&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;lo&nbsp;=&nbsp;ListObjects.Add(&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SourceType:=xlSrcRange,&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Source:=Range(<span class="visualBasic__string">&quot;A1:F13&quot;</span>),&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;XlListObjectHasHeaders:=xlYes)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lo.Name&nbsp;=&nbsp;<span class="visualBasic__string">&quot;SampleData&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;so&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Sort&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;lo.Sort&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;.SortFields&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Clear&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Add&nbsp;Range(<span class="visualBasic__string">&quot;SampleData[[#All],&nbsp;[Total]]&quot;</span>),&nbsp;SortOn:=xlSortOnValues,&nbsp;Order:=xlAscending&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Header&nbsp;=&nbsp;xlYes&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Orientation&nbsp;=&nbsp;xlSortColumns&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Apply&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Set&nbsp;up&nbsp;a&nbsp;simple&nbsp;filter,&nbsp;giving&nbsp;values&nbsp;between&nbsp;150&nbsp;and&nbsp;200:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lo.Range.AutoFilter&nbsp;Field:=<span class="visualBasic__number">6</span>,&nbsp;Criteria1:=<span class="visualBasic__string">&quot;&gt;150&quot;</span>,&nbsp;<span class="visualBasic__keyword">Operator</span>:=xlAnd,&nbsp;Criteria2:=<span class="visualBasic__string">&quot;&lt;200&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Now&nbsp;clear&nbsp;the&nbsp;filter:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lo.Range.AutoFilter&nbsp;Field:=<span class="visualBasic__number">6</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Filter&nbsp;for&nbsp;the&nbsp;top&nbsp;10&nbsp;percent,&nbsp;which&nbsp;should&nbsp;only&nbsp;be&nbsp;one&nbsp;item:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lo.Range.AutoFilter&nbsp;Field:=<span class="visualBasic__number">6</span>,&nbsp;<span class="visualBasic__keyword">Operator</span>:=xlTop10Percent&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Delete&nbsp;the&nbsp;List&nbsp;object&nbsp;so&nbsp;you&nbsp;can&nbsp;run&nbsp;the&nbsp;code&nbsp;again&nbsp;if&nbsp;you&nbsp;like.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lo.Delete&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Sub</span>&nbsp;FillRandomData()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;No&nbsp;need&nbsp;to&nbsp;stop&nbsp;through&nbsp;this&nbsp;procedure.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Range(<span class="visualBasic__string">&quot;A1&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;F1&quot;</span>).Value&nbsp;=&nbsp;Array(<span class="visualBasic__string">&quot;Month&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;North&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;South&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;East&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;West&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;Total&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Fill&nbsp;in&nbsp;twelve&nbsp;rows&nbsp;with&nbsp;random&nbsp;data.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;i&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;j&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;<span class="visualBasic__number">12</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Cells(i&nbsp;&#43;&nbsp;<span class="visualBasic__number">1</span>,&nbsp;<span class="visualBasic__number">1</span>).Value&nbsp;=&nbsp;MonthName(i,&nbsp;<span class="visualBasic__keyword">True</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;j&nbsp;=&nbsp;<span class="visualBasic__number">2</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;<span class="visualBasic__number">5</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Cells(i&nbsp;&#43;&nbsp;<span class="visualBasic__number">1</span>,&nbsp;j)&nbsp;=&nbsp;Round(Rnd&nbsp;*&nbsp;<span class="visualBasic__number">100</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;j&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Range(<span class="visualBasic__string">&quot;F2&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;F13&quot;</span>).Formula&nbsp;=&nbsp;<span class="visualBasic__string">&quot;=SUM(B2:E2)&quot;</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
</pre>
</div>
</div>
</div>
<h1><span>Source Code Files</span></h1>
<ul>
<li><span style="font-size:small"><em><a id="25895" href="/site/view/file/25895/1/Excel.ListObjectSortFilter.txt">Excel.ListObjectSortFilter.txt</a>&nbsp;- Download this sample only.</em></span>
</li><li><span style="font-size:small"><em><a id="25896" href="/site/view/file/25896/1/Office%202010%20101%20Code%20Samples.zip">Office 2010 101 Code Samples.zip</a>&nbsp;- Download all the samples.</em></span>
</li></ul>
<h1>More Information</h1>
<ul>
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/aa905411">Excel Developer Center on MSDN</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/hh360994">101 Code Samples for Office 2010 Developers</a></span>
</li></ul>
