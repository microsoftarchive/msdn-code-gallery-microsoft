# How to crop image using mouse selection in PictureBox in Windows form
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- .NET
- Windows Forms
- Windows
- Windows Desktop App Development
## Topics
- Visual Studio
- Crop image
- Mouse Selection
## Updated
- 03/31/2016
## Description

<div class="WordSection1">
<h1>Crop the image from Windows Forms PictureBox control (<span class="SpellE">CSWinformCropImage</span>) by Visual Studio 2012</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to crop the image from specific
<span class="SpellE">Picturebox</span> control into destination <span class="SpellE">
Picturebox</span> control using mouse selection or specified coordinates.</p>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.75in; text-indent:-.5in">
<span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>How to use mouse to select an area (rectangle) in a <span class="SpellE">
Picturebox</span> control.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.75in; text-indent:-.5in">
<span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>How to crop the image by the rectangle.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.75in; text-indent:-.5in">
<strong>&nbsp;</strong></p>
<h2>Building the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.75in; text-indent:-.5in">
<span><span>1)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The main aim of the sample is to have a focused rectangle for selection of area on the source image.<span>&nbsp;
</span>The code in the KB - <a href="http://support.microsoft.com/kb/314945">http://support.microsoft.com/kb/314945</a> - explains it well how it can be achieved with some modifications.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>2)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>We change the mouse cursor style to &ldquo;Cross&rdquo; when on source picture box.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>3)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>After a user selects the area on the image to be cropped, we create a Rectangle that is used to draw the new cropped image (Zoomed) on the target picture box.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>4)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>We achieve this drawing using the Graphics object and overload of the
<span class="SpellE"><span class="GramE">Graphics.DrawImage</span></span><span class="GramE">(</span>) method.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>5)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The image in the source picture box can also be cropped by specifying coordinates in the form of X1, Y1, X2, Y2 in a text box.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.75in; text-indent:-.5in">
<span><span>6)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>In both cropping approaches, we modify the <span class="SpellE">
RectCropArea</span> variable.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.75in; text-indent:-.5in">
&nbsp;</p>
<h2>Running the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.75in; text-indent:-.5in">
<span><span>1)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open the &ldquo;CSWinformsCropImageSample.sln&rdquo; <span class="GramE">
file</span> in Visual Studio 2010.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>2)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Run the sample.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>3)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Select the area to be cropped on the source image picture box using mouse.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>4)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>When selected, click the crop button.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.5in">
<span><span>5)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The resulting cropped image appears in the target picture box in zoomed view.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.75in; text-indent:-.5in">
<span><span>6)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Alternative, if you can specify the coordinates in a text for the area to be cropped.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.75in; text-indent:-.5in">
<strong>&nbsp;</strong></p>
<h2>Using the Code</h2>
<p class="MsoNormal">The following code snippet handles the mouse down event.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SrcPicBox_MouseDown(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;MouseEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Make&nbsp;a&nbsp;note&nbsp;that&nbsp;we&nbsp;&quot;have&nbsp;the&nbsp;mouse&quot;.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;bHaveMouse&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Store&nbsp;the&nbsp;&quot;starting&nbsp;point&quot;&nbsp;for&nbsp;this&nbsp;rubber-band&nbsp;rectangle.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptOriginal.X&nbsp;=&nbsp;e.X;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptOriginal.Y&nbsp;=&nbsp;e.Y;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Special&nbsp;value&nbsp;lets&nbsp;us&nbsp;know&nbsp;that&nbsp;no&nbsp;previous</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;rectangle&nbsp;needs&nbsp;to&nbsp;be&nbsp;erased.</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Display&nbsp;coordinates</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lbCordinates.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Coordinates&nbsp;&nbsp;:&nbsp;&nbsp;&quot;</span>&nbsp;&#43;&nbsp;e.X.ToString()&nbsp;&#43;&nbsp;<span class="cs__string">&quot;,&nbsp;&quot;</span>&nbsp;&#43;&nbsp;e.Y.ToString();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptLast.X&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptLast.Y&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Rectangle(<span class="cs__keyword">new</span>&nbsp;Point(e.X,&nbsp;e.Y),&nbsp;<span class="cs__keyword">new</span>&nbsp;Size());&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">The following code snippet handles the mouse up event.</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SrcPicBox_MouseUp(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;MouseEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Set&nbsp;internal&nbsp;flag&nbsp;to&nbsp;know&nbsp;we&nbsp;no&nbsp;longer&nbsp;&quot;have&nbsp;the&nbsp;mouse&quot;.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;bHaveMouse&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;If&nbsp;we&nbsp;have&nbsp;drawn&nbsp;previously,&nbsp;draw&nbsp;again&nbsp;in&nbsp;that&nbsp;spot</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;to&nbsp;remove&nbsp;the&nbsp;lines.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(ptLast.X&nbsp;!=&nbsp;-<span class="cs__number">1</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Point&nbsp;ptCurrent&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Point(e.X,&nbsp;e.Y);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Display&nbsp;coordinates</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lbCordinates.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Coordinates&nbsp;&nbsp;:&nbsp;&nbsp;&quot;</span>&nbsp;&#43;&nbsp;ptOriginal.X.ToString()&nbsp;&#43;&nbsp;<span class="cs__string">&quot;,&nbsp;&quot;</span>&nbsp;&#43;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ptOriginal.Y.ToString()&#43;&nbsp;<span class="cs__string">&quot;&nbsp;And&nbsp;&quot;</span>&nbsp;&#43;&nbsp;e.X.ToString()&nbsp;&#43;&nbsp;<span class="cs__string">&quot;,&nbsp;&quot;</span>&nbsp;&#43;&nbsp;e.Y.ToString();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Set&nbsp;flags&nbsp;to&nbsp;know&nbsp;that&nbsp;there&nbsp;is&nbsp;no&nbsp;&quot;previous&quot;&nbsp;line&nbsp;to&nbsp;reverse.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptLast.X&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptLast.Y&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptOriginal.X&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ptOriginal.Y&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;The following code snippet handles the mouse move event.&nbsp;</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SrcPicBox_MouseMove(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;MouseEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Point&nbsp;ptCurrent&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Point(e.X,&nbsp;e.Y);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;If&nbsp;we&nbsp;&quot;have&nbsp;the&nbsp;mouse&quot;,&nbsp;then&nbsp;we&nbsp;draw&nbsp;our&nbsp;lines.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(bHaveMouse)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;If&nbsp;we&nbsp;have&nbsp;drawn&nbsp;previously,&nbsp;draw&nbsp;again&nbsp;in</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;that&nbsp;spot&nbsp;to&nbsp;remove&nbsp;the&nbsp;lines.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(ptLast.X&nbsp;!=&nbsp;-<span class="cs__number">1</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Display&nbsp;Coordinates</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lbCordinates.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Coordinates&nbsp;&nbsp;:&nbsp;&nbsp;&quot;</span>&nbsp;&#43;&nbsp;ptOriginal.X.ToString()&nbsp;&#43;&nbsp;<span class="cs__string">&quot;,&nbsp;&quot;</span>&nbsp;&#43;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ptOriginal.Y.ToString()&nbsp;&#43;&nbsp;<span class="cs__string">&quot;&nbsp;And&nbsp;&quot;</span>&nbsp;&#43;&nbsp;e.X.ToString()&nbsp;&#43;&nbsp;<span class="cs__string">&quot;,&nbsp;&quot;</span>&nbsp;&#43;&nbsp;e.Y.ToString();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Update&nbsp;last&nbsp;point.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ptLast&nbsp;=&nbsp;ptCurrent;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Draw&nbsp;new&nbsp;lines.</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;e.X&nbsp;-&nbsp;rectCropArea.X;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;normal</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(e.X&nbsp;&gt;&nbsp;ptOriginal.X&nbsp;&amp;&amp;&nbsp;e.Y&nbsp;&gt;&nbsp;ptOriginal.Y)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;e.X&nbsp;-&nbsp;ptOriginal.X;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;e.Y&nbsp;-&nbsp;rectCropArea.Height;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;e.Y&nbsp;-&nbsp;ptOriginal.Y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;<span class="cs__keyword">if</span>&nbsp;(e.X&nbsp;&lt;&nbsp;ptOriginal.X&nbsp;&amp;&amp;&nbsp;e.Y&nbsp;&gt;&nbsp;ptOriginal.Y)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;ptOriginal.X&nbsp;-&nbsp;e.X;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;e.Y&nbsp;-&nbsp;ptOriginal.Y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.X&nbsp;=&nbsp;e.X;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Y&nbsp;=&nbsp;ptOriginal.Y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;<span class="cs__keyword">if</span>&nbsp;(e.X&nbsp;&gt;&nbsp;ptOriginal.X&nbsp;&amp;&amp;&nbsp;e.Y&nbsp;&lt;&nbsp;ptOriginal.Y)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;e.X&nbsp;-&nbsp;ptOriginal.X;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;ptOriginal.Y&nbsp;-&nbsp;e.Y;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.X&nbsp;=&nbsp;ptOriginal.X;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Y&nbsp;=&nbsp;e.Y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;ptOriginal.X&nbsp;-&nbsp;e.X;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;e.Y&nbsp;-&nbsp;rectCropArea.Height;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;ptOriginal.Y&nbsp;-&nbsp;e.Y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.X&nbsp;=&nbsp;e.X;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Y&nbsp;=&nbsp;e.Y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SrcPicBox.Refresh();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">The following code snippet handles the <span class="SpellE">
BtnCrop</span> click event.</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;BtnCrop_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;TargetPicBox.Refresh();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Bitmap&nbsp;sourceBitmap&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Bitmap(SrcPicBox.Image,&nbsp;SrcPicBox.Width,&nbsp;SrcPicBox.Height);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Graphics&nbsp;g&nbsp;=&nbsp;TargetPicBox.CreateGraphics();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(chkCropCordinates.Checked)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lbCordinates.Text&nbsp;=&nbsp;<span class="cs__string">&quot;&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;cordinates&nbsp;=&nbsp;tbCordinates.Text.ToString().Split(<span class="cs__string">','</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;cord0,&nbsp;cord1,&nbsp;cord2,&nbsp;cord3;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cord0&nbsp;=&nbsp;Convert.ToInt32(cordinates[<span class="cs__number">0</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cord1&nbsp;=&nbsp;Convert.ToInt32(cordinates[<span class="cs__number">1</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cord2&nbsp;=&nbsp;Convert.ToInt32(cordinates[<span class="cs__number">2</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cord3&nbsp;=&nbsp;Convert.ToInt32(cordinates[<span class="cs__number">3</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lbCordinates.Text&nbsp;=&nbsp;ex.Message;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>((cord0&nbsp;&lt;&nbsp;cord2&nbsp;&amp;&amp;&nbsp;cord1&nbsp;&lt;cord3))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Rectangle(cord0,&nbsp;cord1,&nbsp;cord2&nbsp;-&nbsp;cord0,&nbsp;cord3&nbsp;-&nbsp;cord1);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;<span class="cs__keyword">if</span>&nbsp;(cord2&nbsp;&lt;&nbsp;cord0&nbsp;&amp;&amp;&nbsp;cord3&nbsp;&gt;&nbsp;cord1)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Rectangle(cord2,&nbsp;cord1,&nbsp;cord0&nbsp;-&nbsp;cord2,&nbsp;cord3&nbsp;-&nbsp;cord1);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;<span class="cs__keyword">if</span>&nbsp;(cord2&nbsp;&gt;&nbsp;cord0&nbsp;&amp;&amp;&nbsp;cord3&nbsp;&lt;&nbsp;cord1)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Rectangle(cord0,&nbsp;cord3,&nbsp;cord2&nbsp;-&nbsp;cord0,&nbsp;cord1&nbsp;-&nbsp;cord3);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Rectangle(cord2,&nbsp;cord3,&nbsp;cord0&nbsp;-&nbsp;cord2,&nbsp;cord1&nbsp;-&nbsp;cord3);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;g.DrawImage(sourceBitmap,&nbsp;<span class="cs__keyword">new</span>&nbsp;Rectangle(<span class="cs__number">0</span>,&nbsp;<span class="cs__number">0</span>,&nbsp;TargetPicBox.Width,&nbsp;TargetPicBox.Height),&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea,&nbsp;GraphicsUnit.Pixel);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;sourceBitmap.Dispose();&nbsp;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<div class="endscriptcode"></div>
<div class="endscriptcode">
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
</div>
<p>&nbsp;</p>
</div>
