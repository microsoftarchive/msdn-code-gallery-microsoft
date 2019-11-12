# XML syntax highlighting in RichTextBox (CSRichTextBoxSyntaxHighlighting)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows Forms
## Topics
- Syntax Highlight
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>Windows APPLICATION: CSRichTextBoxSyntaxHighlighting Overview </h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New">The sample demonstrates how to format XML and highlight the elements in
<br>
RichTextBoxControl.<br>
<br>
RichTextBoxControl can process RTF(Rich Text Format) file, which is a proprietary<br>
document file format with published specification developed by Microsoft Corporation.<br>
<br>
A simple RTF file is like <br>
<br>
{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052<br>
{\fonttbl{\f0\fnil Courier New;}}<br>
{\colortbl ;\red0\green0\blue255;\red139\green0\blue0;\red255\green0\blue0;\red0\green0\blue0;}<br>
\viewkind4\uc1\pard\cf1\f0\fs24 <br>
&lt;?\cf2 xml \cf3 version\cf1 =\cf0 &quot;\cf1 1.0\cf0 &quot; \cf3 encoding\cf1 =\cf0 &quot;\cf1 utf-8\cf0 &quot;\cf1 ?&gt;\par<br>
&lt;\cf2 html\cf1 &gt;\par<br>
&nbsp; &nbsp;&lt;\cf2 head\cf1 &gt;\par<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;\cf2 title\cf1 &gt;\par<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;\cf4 My home page\par<br>
&nbsp; &nbsp; &nbsp; &nbsp;\cf1 &lt;/\cf2 title\cf1 &gt;\par<br>
&nbsp; &nbsp;&lt;/\cf2 head\cf1 &gt;\par<br>
&nbsp; &nbsp;&lt;\cf2 body \cf3 bgcolor\cf1 =\cf0 &quot;\cf1 000000\cf0 &quot; \cf3 text\cf1 =\cf0 &quot;\cf1 ff0000\cf0 &quot; \cf1 &gt;\par<br>
&nbsp; &nbsp; &nbsp; &nbsp;\cf4 Hello World!\par<br>
&nbsp; &nbsp;\cf1 &lt;/\cf2 body\cf1 &gt;\par<br>
&lt;/\cf2 html\cf1 &gt;\par<br>
}<br>
<br>
It contains 2 parts:Header and Content.The colortbl in header includes all the color
<br>
definitions used in the file. \cfN means the Foreground color and \par means a new
<br>
paragraph.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Step1. Build this project in VS2010. <br>
<br>
Step2. Run CSRichTextBoxSyntaxHighlighting .exe.<br>
<br>
Step3. Paste following Xml script to the RichTextBox in the UI.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;&lt;html&gt;&lt;head&gt;&lt;title&gt;My home page&lt;/title&gt;&lt;/head&gt;&lt;body bgcolor=&quot;000000&quot; text=&quot;ff0000&quot;&gt;Hello World!&lt;/body&gt;&lt;/html&gt;<br>
<br>
<br>
Step4. Click the &quot;Process&quot; button, then the text in the RichTextBox will be changed to
<br>
&nbsp; &nbsp; &nbsp; following XML with colors.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &lt;html&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;head&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;title&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;My home page<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/title&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/head&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;body bgcolor=&quot;000000&quot; text=&quot;ff0000&quot; &gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Hello World!<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/body&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &lt;/html&gt;<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Design a class XMLViewerSettings that defines the colors used in the XmlViewer, and
<br>
&nbsp; some constants that specify the color order in the RTF.<br>
<br>
2. Design a class XMLViewer that inherits System.Windows.Forms.RichTextBox. It is used
<br>
&nbsp; to display an Xml in a specified format. <br>
&nbsp; <br>
&nbsp; RichTextBox uses the Rtf format to show the test. The XMLViewer will convert the Xml<br>
&nbsp; to Rtf with some formats specified in the XMLViewerSettings, and then set the Rtf
<br>
&nbsp; property to the value.<br>
&nbsp; &nbsp; &nbsp; <br>
3. The CharacterEncoder class supplies a static(Shared) method to encode some <br>
&nbsp; special characters in Xml and Rtf, such as '&lt;', '&gt;', '&quot;', '&', ''', '\',<br>
&nbsp; '{' and '}' .<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
RichTextBox Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.windows.forms.richtextbox.aspx">http://msdn.microsoft.com/en-us/library/system.windows.forms.richtextbox.aspx</a><br>
<br>
Rich Text Format (RTF) Specification, version 1.6<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa140277(office.10).aspx">http://msdn.microsoft.com/en-us/library/aa140277(office.10).aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
