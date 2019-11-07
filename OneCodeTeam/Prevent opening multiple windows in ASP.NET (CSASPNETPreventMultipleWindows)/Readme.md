# Prevent opening multiple windows in ASP.NET (CSASPNETPreventMultipleWindows)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Popup Window
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>CSASPNETPreventMultipleWindows Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
The project illustrates how to detect and prevent multiple windows or <br>
tab usage in Web Applications.<br>
<br>
Demo the Sample. <br>
<br>
Please follow these demonstration steps below.<br>
<br>
Step 1: Open the CSASPNETPreventMultipleWindows.sln.<br>
<br>
Step 2: Expand the CSASPNETPreventMultipleWindows web application and press <br>
&nbsp; &nbsp; &nbsp; &nbsp;Ctrl &#43; F5 to show the Default.aspx.<br>
<br>
Step 3: We will see two links on the page. First, you can Left click one<br>
&nbsp; &nbsp; &nbsp; &nbsp;of them jump to correct link(Like Nextlink.aspx and Nextlink2.aspx).<br>
<br>
Step 4: Then, you can right click these links and choose Open In New Tab <br>
&nbsp; &nbsp; &nbsp; &nbsp;or Open In New Window,and you will find the link not available.<br>
<br>
Step 5: you can even copy the right url and Paste it in the browser address bar for
<br>
&nbsp; &nbsp; &nbsp; &nbsp;test.but you can not achieve your goal.<br>
<br>
Step 6: Validation finished.<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
Step 1. Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2010 or<br>
&nbsp; &nbsp; &nbsp; &nbsp;Visual Web Developer 2010. Name it as &quot;CSASPNETPreventMultipleWindows&quot;.<br>
<br>
Step 2. Add one folder, &quot;UserControls&quot;.Add two User Controls in this folder,<br>
&nbsp; &nbsp; &nbsp; &nbsp;&quot;DefaultPage.ascx&quot;,&quot;NextPage.ascx&quot;.<br>
<br>
Step 3. Add five web forms in the root directory,&quot;Default.aspx&quot;,&quot;InvalidPage.aspx&quot;<br>
&nbsp; &nbsp; &nbsp; &nbsp;,&quot;Main.aspx&quot;,&quot;NextLink.aspx&quot;,&quot;NextLink2.aspx&quot;.<br>
<br>
Step 4. Move DefaultPage.ascx user control on Default.aspx file and move<br>
&nbsp; &nbsp; &nbsp; &nbsp;NextPage.ascx user control on all other files extension name are &quot;.aspx&quot;.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;In Defalut.aspx.cs page, we have a method generate unique random number:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp; &nbsp; &nbsp; &nbsp;public string GetWindowName()<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string WindowName = Guid.NewGuid().ToString().Replace(&quot;-&quot;, &quot;&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Session[&quot;WindowName&quot;] = WindowName;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return WindowName;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[/code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;And window.name will recieve this unique string,we use window.name to prevent<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;mutiple windows and tabs. <br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;//set window.name property<br>
&nbsp; &nbsp; &nbsp; &nbsp;window.open(&quot;Main.aspx&quot;, &quot;&lt;%=GetWindowName() %&gt;&quot;);<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; //If this window name not equal to sessions,will be goto InvalidPage<br>
&nbsp; &nbsp; &nbsp; if (window.name != &quot;&lt;%=GetWindowName()%&gt;&quot;) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;window.name = &quot;InvalidPage&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;window.open(&quot;InvalidPage.aspx&quot;,&quot;_self&quot;); &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; }<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; [/code]<br>
<br>
Step 5. Write the codes like the sample.you can get more details from comments in<br>
&nbsp; &nbsp; &nbsp; &nbsp;the sample file.<br>
&nbsp; &nbsp; &nbsp; &nbsp; <br>
Step 6. Build the application and you can debug it.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: User control<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/fb3w5b53.aspx">http://msdn.microsoft.com/en-us/library/fb3w5b53.aspx</a><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
