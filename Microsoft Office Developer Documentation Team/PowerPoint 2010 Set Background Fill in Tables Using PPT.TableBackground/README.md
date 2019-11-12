# PowerPoint 2010: Set Background Fill in Tables Using PPT.TableBackground
## License
- Apache License, Version 2.0
## Technologies
- PowerPoint 2010
- Office 2010
## Topics
- Office 2010 101 code samples
- formatting tables
## Updated
- 08/05/2011
## Description

<h1>Introduction</h1>
<p><span style="font-size:small">This sample shows how to set the background fill properties for each cell of a table in a Microsoft PowerPoint 2010 presentation.</span></p>
<p><span style="font-size:small">This code snippet is part of the Office 2010 101 code samples project. This sample, along with others, is offered here to incorporate directly in your code.</span></p>
<p><span style="font-size:small">Each code sample consists of approximately 5 to 50 lines of code demonstrating a distinct feature or feature set, in either VBA or both VB and C# (created in Visual Studio 2010). Each sample includes comments describing the
 sample, and setup code so that you can run the code with expected results or the comments will explain how to set up the environment so that the sample code runs.)</span></p>
<p><span style="font-size:small">Microsoft&reg; Office 2010 gives you the tools needed to create powerful applications. The Microsoft Visual Basic for Applications (VBA) code samples can assist you in creating your own applications that perform specific functions
 or as a starting point to create more complex solutions.</span></p>
<h1><span>Building the Sample</span></h1>
<p><span style="font-size:small">Although PowerPoint 2007 added a TableBackground class&nbsp;to support the Table.Background property, it doesn't allow you to set formatting of the table background, because of limitations of the object model. Without a way
 to emulate the No Fill option in the user interface, there's no way to change the background settings.</span></p>
<p><span style="font-size:small">This example works around that limitation by setting properties for each cell individually. Unfortunately, this requires a separate procedure for each type of setting you want to change.</span></p>
<p><span style="font-size:small">In this example, copy the code into a module in a new PowerPoint presentation. Put your cursor inside the WorkWithTableBackground procedure, and press F8 to start debugging, and then Shift&#43;F8 to continue. Tile the VBA and PowerPoint
 windows so you can watch both at the same time.</span></p>
<p><span style="font-size:20px; font-weight:bold">Description</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Sub</span>&nbsp;WorkWithTableBackground()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;pres&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Presentation&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;pres&nbsp;=&nbsp;ActivePresentation&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Create&nbsp;a&nbsp;slide&nbsp;with&nbsp;a&nbsp;table&nbsp;on&nbsp;it:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;sld&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Slide&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;sld&nbsp;=&nbsp;pres.Slides.Add(<span class="visualBasic__number">2</span>,&nbsp;ppLayoutTable)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;tbl&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Table&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;tbl&nbsp;=&nbsp;sld.Shapes.AddTable(<span class="visualBasic__number">4</span>,&nbsp;<span class="visualBasic__number">4</span>).Table&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Fill&nbsp;the&nbsp;table&nbsp;with&nbsp;data:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;FillTable&nbsp;tbl&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Retrieve&nbsp;Background&nbsp;property:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;tb&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;TableBackground&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;tb&nbsp;=&nbsp;tbl.Background&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Should&nbsp;be&nbsp;able&nbsp;to&nbsp;do&nbsp;this:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;tb.Fill.ForeColor.ObjectThemeColor&nbsp;=&nbsp;msoThemeColorAccent2&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;But&nbsp;that&nbsp;doesn't&nbsp;work,&nbsp;because&nbsp;when&nbsp;you&nbsp;create&nbsp;a&nbsp;table,</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;you&nbsp;must&nbsp;also&nbsp;set&nbsp;its&nbsp;fill&nbsp;to&nbsp;None&nbsp;(as&nbsp;you&nbsp;can&nbsp;in&nbsp;the&nbsp;user&nbsp;interface).</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;There&nbsp;doesn't&nbsp;appear&nbsp;to&nbsp;be&nbsp;any&nbsp;way&nbsp;to&nbsp;do&nbsp;that&nbsp;from&nbsp;VBA.&nbsp;Therefore,</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;you&nbsp;must&nbsp;rely&nbsp;on&nbsp;a&nbsp;procedure&nbsp;like&nbsp;SetTableColor&nbsp;or&nbsp;SetTableTexture,</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;which&nbsp;handles&nbsp;each&nbsp;cell&nbsp;individually.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;This&nbsp;procedure&nbsp;sets&nbsp;the&nbsp;cell&nbsp;color:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SetTableColor&nbsp;tbl,&nbsp;msoThemeColorAccent2&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;This&nbsp;procedure&nbsp;sets&nbsp;the&nbsp;cell&nbsp;texture:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SetTableTexture&nbsp;tbl,&nbsp;msoTextureCork&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Sub</span>&nbsp;FillTable(tbl&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Table)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Fill&nbsp;a&nbsp;table&nbsp;with&nbsp;sample&nbsp;data.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;row&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;col&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;col&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;tbl.Columns.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbl.Cell(<span class="visualBasic__number">1</span>,&nbsp;col).Shape.TextFrame.TextRange.Text&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Heading&nbsp;&quot;</span>&nbsp;&amp;&nbsp;col&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;col&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;shp&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Shape&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;row&nbsp;=&nbsp;<span class="visualBasic__number">2</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;tbl.Rows.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;col&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;tbl.Columns.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;shp&nbsp;=&nbsp;tbl.Cell(row,&nbsp;col).Shape&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;shp.TextFrame.TextRange.Text&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Cell&nbsp;&quot;</span>&nbsp;&amp;&nbsp;row&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;,&nbsp;&quot;</span>&nbsp;&amp;&nbsp;col&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;shp.Fill.Visible&nbsp;=&nbsp;msoFalse&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;col&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;row&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Sub</span>&nbsp;SetTableColor(tbl&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Table,&nbsp;themeColor&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;MsoThemeColorIndex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Iterate&nbsp;through&nbsp;rows&nbsp;and&nbsp;columns&nbsp;and&nbsp;set&nbsp;the&nbsp;color</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;for&nbsp;each&nbsp;cell&nbsp;individually.&nbsp;You&nbsp;must&nbsp;use&nbsp;code&nbsp;like&nbsp;this</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;cause&nbsp;you&nbsp;cannot&nbsp;override&nbsp;the&nbsp;built-in&nbsp;background&nbsp;for&nbsp;a&nbsp;table.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;row&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;col&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;col&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;tbl.Columns.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;row&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;tbl.Rows.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;Fill&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbl.Cell(row,&nbsp;col).Shape.Fill.ForeColor.ObjectThemeColor&nbsp;=&nbsp;themeColor&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;row&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;col&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Sub</span>&nbsp;SetTableTexture(tbl&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Table,&nbsp;texture&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;MsoPresetTexture)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Iterate&nbsp;through&nbsp;rows&nbsp;and&nbsp;columns&nbsp;and&nbsp;set&nbsp;the&nbsp;texture</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;for&nbsp;each&nbsp;cell&nbsp;individually.&nbsp;You&nbsp;must&nbsp;use&nbsp;code&nbsp;like&nbsp;this</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;cause&nbsp;you&nbsp;cannot&nbsp;override&nbsp;the&nbsp;built-in&nbsp;background&nbsp;for&nbsp;a&nbsp;table.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;row&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;col&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;col&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;tbl.Columns.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;row&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;tbl.Rows.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;Fill&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbl.Cell(row,&nbsp;col).Shape.Fill.PresetTextured&nbsp;texture&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;row&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;col&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<h1><span>Source Code Files</span></h1>
<ul>
<li><span style="font-size:small"><em><em><a id="26177" href="/site/view/file/26177/1/PPT.TableBackground.txt">PPT.TableBackground.txt</a>&nbsp;- Download this sample only.<br>
</em></em></span></li><li><span style="font-size:small"><em><em><a id="26178" href="/site/view/file/26178/1/Office%202010%20101%20Code%20Samples.zip">Office 2010 101 Code Samples.zip</a>&nbsp;- Download all the samples.</em></em></span>
</li></ul>
<h1>More Information</h1>
<ul>
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/aa905465">PowerPoint Developer Center on MSDN</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/hh360994">101 Code Samples for Office 2010 Developers</a></span>
</li></ul>
