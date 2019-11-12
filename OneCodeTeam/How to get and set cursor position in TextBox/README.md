# How to get and set cursor position in TextBox
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Cursor
## Updated
- 07/05/2013
## Description

<h1>How to get and set cursor position in TextBox (JSASPNETDiposeTextboxCursor)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The project illustrates how to get and set cursor position in TextBox and Textarea. This sample can executed in IE,FireFox,Chrome,etc browsers.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Please follow these demonstration steps below.</p>
<p class="MsoNormal">Step 1. <span style="">&nbsp;</span>Open the JSASPNETDiposeTextboxCursor.sln. Expand the JSASPNETDiposeTextboxCursor web application, open MainPage.aspx page and press Ctrl &#43; F5 to show it.</p>
<p class="MsoNormal">Step 2. We will see two links on the page. You can click one of them jump to Text.aspx page or Textarea.aspx page.</p>
<p class="MsoNormal">Step 3. Click the target text and get the position in the Textbox below, and you can input an effective value in Set Cursor Position Textbox and click button to set cursor of text.</p>
<p class="MsoNormal">Step 4. When you click the content of textarea to get the cursor position. In IE browser,you can get the coordinates of cursor position, but in FireFox and Chrome, you can get preceding string of cursor in one Textbox,and get rear string
 of cursor in another.you can also write a value to set cursor in textarea like step 3.</p>
<p class="MsoNormal"><span style="">&nbsp;</span>[Note]</p>
<p class="MsoNormal">In Textarea.aspx page, when you begin to get the position of cursor, it will return the forepart of the string in X: TextBox and back part of the string in Y: TextBox. Because most of people like to press enter key to change a new line,
 if you use textarea.value to retrieve the text. You will find enter key is &quot;\r\n&quot; in textarea's text, though the textarea.value cannot know which column the user will press enter key to change lines, so it may trigger some problems of get X and Y
 values. There we separate string by cursor and store them in TextBox controls.</p>
<p class="MsoNormal">[/Note]</p>
<p class="MsoNormal">Step 5. After finish above steps, execute the application with Firefox and Chrome browser.</p>
<p class="MsoNormal">Step 6. Validation finished.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step 1. Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2010 or Visual Web Developer 2010. Name it as &quot;JSASPNETDiposeTextboxCursor &quot;.</p>
<p class="MsoNormal">Step 2. Create three web form files and name them as &quot;MainPage.aspx&quot;,&quot;Text.aspx&quot;, &quot;Textarea.aspx&quot;.</p>
<p class="MsoNormal">Step 3. Open the MainPage.aspx file,add two links in the page and specify them to another two web forms.
</p>
<p class="MsoNormal">Step 4. Open the text.aspx file, add a table, three textboxes, a button and some labels in page.Write the javascript according to the description in sample file withthe same file name.Then you need to add some events in textbox and buttons.</p>
<h3>The following code is used to show how to create events in TextBox and Button controls</h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;input type=&quot;Text&quot; id=&quot;TextBox1&quot; value=&quot;aaaa     &quot; name=&quot;TextBox1&quot; 
  cols=&quot;50&quot; rows=&quot;10&quot; onmouseup=&quot;GetCursorPosition(this)&quot; onmousedown=
 &quot;GetCursorPosition(this)&quot; onkeyup=&quot;GetCursorPosition(this)&quot; 
  onkeydown=&quot;GetCursorPosition(this)&quot; onfocus=&quot;GetCursorPosition(this)&quot;&lt;/input&gt;


&lt;input type=&quot;button&quot; value=&quot;set cursor position&quot; onclick=&quot;JudgeInputAndSetPosition()&quot;&gt;
&lt;/input&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal">Step 5. Open the Textarea.aspx file, add a textarea,three textbox, a button and some labels in page, write the JavaScript according to the sample files. And the event in tag &lt;body&gt;, textarea, button.</p>
<h3>The following code is used to show how to add event handlers </h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;body onload=&quot;LoadPostion(document.forms[0].TextBox1)&quot;&gt;


&lt;textarea id=&quot;TextBox1&quot; name=&quot;TextBox1&quot; cols=&quot;50&quot; rows=&quot;10&quot; 
onmouseup=&quot;GetCursorPosition()&quot;  onmousedown=&quot;GetCursorPosition()&quot;
onkeyup=&quot;GetCursorPosition()&quot;  onkeydown=&quot;GetCursorPosition()&quot; 
onfocus=&quot;GetCursorPosition()&quot;&gt;.... &lt;/textarea&gt;


&lt;input type=&quot;button&quot; value=&quot;set cursor position&quot; onclick=&quot;JudgeInputAndSetPosition()&quot;&gt;
&lt;/input&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></p>
<p class="MsoNormal"><span class="GramE">Step 6.</span> Build the application and you can debug it.</p>
<p class="MsoNormal"></p>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/ms536419(VS.85).aspx">MSDN:
<span class="SpellE">createRange</span> in <span class="SpellE">javascript</span></a></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/ms535904(VS.85).aspx">MSDN: object
<span class="SpellE">textarea</span></a></p>
<p class="MsoNormal"><a href="http://geekswithblogs.net/svanvliet/archive/2005/03/24/textarea-cursor-position-with-javascript.aspx"><span class="SpellE">MSDN<span class="GramE">:how</span></span> to set cursor in
<span class="SpellE">textarea</span></a></p>
<p class="MsoNormal"></p>
<p class="MsoNormal"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
