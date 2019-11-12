# Print a specific part of webpage in ASP.NET
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Printing
- Webpage
## Updated
- 06/13/2013
## Description

<h1>Print a part of the page (CSASPNETPrintPartOfPage)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The project illustrates how to print a specific part of a page. A web form page usually contains many controls and some of them perhaps need not to print, such as button controls, you cannot click them in print page. This sample provides
 a method to avoid printing needless part of page.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step 1: Open the CSASPNETPrintPartOfPage.sln.<span style=""><br>
</span>Step 2: Expand the CSASPNETPrintPartOfPage web application and press Ctrl &#43; F5 to show the Default.aspx.<span style=""><br>
</span>Step 3: You will see many controls in Default.aspx page, <span style="">t</span>here are one &quot;print this page&quot; button and four CheckBoxes in the middle of page.<span style="">
</span></p>
<p class="MsoNormal"><span style=""><img src="84617-image.png" alt="" width="839" height="467" align="middle">
</span><span style=""><br>
</span>Step 4: Choose the CheckBox to select which part of the page you want to print, then click the Button control to print current page. If you do not<span style=""><br>
</span><span style="">&nbsp;</span>have an available printer, you can also choose the Micro<span style="">s</span>oft XPS Document Writer to test this sample.
<span style=""><br>
</span>Step 5: Validation is finished.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step 1. Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 201<span style="">2</span> or Visual Web Developer 201<span style="">2</span>. Name it as &quot;CSASPNETPrintPartOfPage&quot;.<span style=""><br>
</span>Step 2. Add a web form in the root direcory, named it as &quot;Default.aspx&quot;.<span style=""><br>
</span>Step 3. Add a &quot;image&quot; folder in the root direcory, add a picture you want
<span style="">to&nbsp;</span>display in page.<span style=""><br>
</span>Step 4. Create some tables in Default.aspx,and you can fill them with html elements such as image, text, control , etc.
<span style=""><br>
</span>Step 5. Define some public strings to store html tag and deposite them in Default.aspx page.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
public string PrintImageBegin;
&nbsp;&nbsp;&nbsp;&nbsp; public string PrintImageEnd;


 if (chkDisplayImage.Checked)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; { 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PrintImageBegin = string.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; PrintImageEnd = string.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; { 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PrintImageBegin = EnablePirnt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; PrintImageEnd = EndDiv;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">&nbsp;</span>Step 6. Use JavaScript code to print current page depending on the status of Checkbox; assign JavaScript function to button's onclick event. The<span style="">
</span>css and js code:<span style=""> </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;style type=&quot;text/css&quot; media=&quot;print&quot;&gt;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.nonPrintable
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; display: none;
&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; &lt;/style&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>

<pre id="codePreview" class="js">
&lt;script type=&quot;text/javascript&quot;&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; function print_page() {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; window.print();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; &lt;/script&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 7. Build the application and you can debug it.<span style="">
</span></p>
<h2>More Information</h2>
<p class="MsoNormal">MSDN: window.print function<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/ms536672(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms536672(VS.85).aspx</a><span style=""><br>
</span>MSDN: CSS Reference<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/ms531209(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms531209(VS.85).aspx</a><span style="">
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
