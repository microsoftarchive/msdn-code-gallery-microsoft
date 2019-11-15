# Windows Forms localization demo (CSWinFormLocalization)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- Windows Forms
## Topics
- Globalization and Localization
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WINDOWS FORMS APPLICATION : CSWinFormLocalization Project Overview<br>
</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
The Windows Forms Localization sample demonstrates how to localize <br>
Windows Forms application.<br>
&nbsp; <br>
</p>
<h3>Creation:</h3>
<p style="font-family:Courier New"><br>
1. Create a new Windows Application named &quot;CSWinFormLocalization&quot;. <br>
<br>
2. In the Properties window, set the form's Localizable property to true.<br>
<br>
3. Drag two Button controls and one Label control from the Windows Forms tab <br>
&nbsp; of the Toolbox to the form, and set their Text property as follows:<br>
&nbsp; <br>
&nbsp; button1: &quot;Hello World!&quot;,<br>
&nbsp; button2: &quot;I'm a button.&quot;,<br>
&nbsp; label1 : &quot;I'm a label&quot;.<br>
&nbsp; <br>
4. Set the form's Language property to Chinese (Simplified Chinese).<br>
<br>
5. Set the Text property for the three controls as follows:<br>
<br>
&nbsp; button1: &quot;你好，世界！&quot;, <br>
&nbsp; button2: &quot;我是一个按钮。&quot;, <br>
&nbsp; label1 : &quot;我是一个标签。&quot;.<br>
<br>
6. Save and build the solution.<br>
<br>
7. Click the Show All Files button in Solution Explorer. <br>
<br>
&nbsp; The resource files appear underneath Form1.cs. <br>
&nbsp; Form1.resx is the resource file for the default culture.<br>
&nbsp; Form1.zh-CHS.resx is the resource file for Simplified Chinese as spoken in PRC.
<br>
<br>
8. Press Ctrl&#43;F5 to run the application, the buttons and label will display in <br>
&nbsp; English or Simplified Chinese depending on the UI language of your o<br>
&nbsp; perating system. <br>
&nbsp; <br>
9. If you want the form always display in Simplified Chinese, you can set the <br>
&nbsp; UI culture to Simplified Chinese before calling the InitializeComponent method.<br>
<br>
&nbsp; &nbsp;public Form1()<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Sets the UI culture to Simplified Chinese. <br>
&nbsp; &nbsp; &nbsp; &nbsp;Thread.CurrentThread.CurrentUICulture = new CultureInfo(&quot;zh-CHS&quot;);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;InitializeComponent();<br>
&nbsp; &nbsp;}<br>
&nbsp; &nbsp;<br>
&nbsp; &nbsp;<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
1. Walkthrough: Localizing Windows Forms<br>
&nbsp; <a target="_blank" href="http://msdn.microsoft.com/en-us/library/y99d1cd3.aspx">
http://msdn.microsoft.com/en-us/library/y99d1cd3.aspx</a><br>
&nbsp; <br>
2. Windows Forms General FAQ.<br>
&nbsp; <a target="_blank" href="http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/77a66f05-804e-4d58-8214-0c32d8f43191">
http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/77a66f05-804e-4d58-8214-0c32d8f43191</a><br>
&nbsp; <br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
