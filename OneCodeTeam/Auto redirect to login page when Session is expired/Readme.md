# Auto redirect to login page when Session is expired
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Session
## Updated
- 07/05/2013
## Description
========================================================================<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; VBASPNETAutoRedirectLoginPage Overview<br>
========================================================================<br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
Use:<br>
<br>
The project illustrates how to develop an asp.net code-sample that will be <br>
redirect to login page when page Session is expired or time out automatically. <br>
It will ask the user to extend the Session at one minutes before<br>
expiring. If the user does not has any actions, the web page will be redirected <br>
to login page automatically, and also note that it need to work in one or<br>
more browser's tabs. <br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
Demo the Sample. <br>
<br>
Please follow these demonstration steps below.<br>
<br>
Step 1: Open the CSASPNETAutoRedirectLoginPage.sln.<br>
<br>
Step 2: Expand the CSASPNETAutoRedirectLoginPage web application and press <br>
&nbsp; &nbsp; &nbsp; &nbsp;Ctrl &#43; F5 to show the LoginPage.aspx page.<br>
<br>
Step 3: You will see login page and input the user name and pass word, and then<br>
&nbsp; &nbsp; &nbsp; &nbsp;click the login button to redirect the user page.<br>
<br>
Step 4: Now you can open more user pages or refresh the user page for persisting<br>
&nbsp; &nbsp; &nbsp; &nbsp;user's activity. <br>
<br>
Step 5: When you stop operating the pages and wait for 1 minute, the all web<br>
&nbsp; &nbsp; &nbsp; &nbsp;pages of code-sample will be redirected the login page automatically. For
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;easy testing, the code-sample set the session's timeout to 1 minute.<br>
<br>
Step 5: Validation finished.<br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
Code Logical:<br>
<br>
Step 1. Create a VB &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2010 or<br>
&nbsp; &nbsp; &nbsp; &nbsp;Visual Web Developer 2010. Name it as &quot;VBASPNETAutoRedirectLoginPage&quot;.<br>
<br>
Step 2. Add a web form named &quot;LoginUser.aspx&quot; for user login in website,<br>
&nbsp; &nbsp; &nbsp; &nbsp;and this page can prevent users who want to skip the login step<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;by visit specified pages.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The login page code will be like this:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Prevent the users who try to skip the login step by visit specified page.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If Not Page.IsPostBack Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Session.Abandon()<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If Request.QueryString(&quot;info&quot;) IsNot Nothing Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim message As String = Request.QueryString(&quot;info&quot;).ToString()<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If message = &quot;0&quot; Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Write(&quot;<strong>you need login first to visit user page.</strong>&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;End Sub<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' User login method.<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim username As String = tbUserName.Text.Trim()<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If tbUserName.Text.Trim() = &quot;username&quot; AndAlso tbPassword.Text.Trim() = &quot;password&quot; Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Session(&quot;username&quot;) = username<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Redirect(&quot;UserPage2.aspx&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Write(&quot;<strong>User name or pass word error.</strong>&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;End Sub<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[/code]<br>
<br>
Step 3. Add two user pages and named them as &quot;UserPage1.aspx&quot;, &quot;UserPage2.aspx&quot;.<br>
&nbsp; &nbsp; &nbsp; &nbsp;These two user pages are use to observe the web application and redirect the login<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;page automatically. You can also open multiple tabs or windows in your browser.
<br>
<br>
Step 4. Add a user control on the root dictionary and drag and drop it in the <br>
&nbsp; &nbsp; &nbsp; &nbsp;every user page for redirecting to the login page when session is expired or<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;time out. The user control's JavaScript code and C# code will be like this:<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;JavaScript code:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;script type=&quot;text/javascript&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;var timeRefresh;<br>
&nbsp; &nbsp; &nbsp; &nbsp;var timeInterval;<br>
&nbsp; &nbsp; &nbsp; &nbsp;var currentTime;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var expressTime;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;expressTime = &quot;&lt;%=ExpressDate %&gt;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp;currentTime = &quot;&lt;%=LoginDate %&gt;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp;setCookie(&quot;express&quot;, expressTime);<br>
&nbsp; &nbsp; &nbsp; &nbsp;timeRefresh = setInterval(&quot;Refresh()&quot;, 1000);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Refresh this page to check session is expire or timeout.<br>
&nbsp; &nbsp; &nbsp; &nbsp;function Refresh() {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var current = getCookie(&quot;express&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var date = current.split(&quot; &quot;)[0];<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var time = current.split(&quot; &quot;)[1];<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var scriptDate = new Date();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var year = scriptDate.getFullYear();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var month = scriptDate.getMonth() &#43; 1;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var day = scriptDate.getDate();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var hour = scriptDate.getHours();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var min = scriptDate.getMinutes();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var second = scriptDate.getSeconds();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (Date.UTC(year, month, day, hour, min, second) &gt;=<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Date.UTC(date.split(&quot;-&quot;)[0], date.split(&quot;-&quot;)[1], date.split(&quot;-&quot;)[2],<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;time.split(&quot;:&quot;)[0], time.split(&quot;:&quot;)[1], time.split(&quot;:&quot;)[2])) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;clearInterval(timeRefresh);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Redirect();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; }<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; function Redirect() {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; window.location.replace(&quot;LoginPage.aspx&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; }<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; // Retrieve cookie by name.<br>
&nbsp; &nbsp; &nbsp; &nbsp; function getCookie(name) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var arg = name &#43; &quot;=&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var aLen = arg.length;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var cLen = document.cookie.length;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var i = 0;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;while (i &lt; cLen) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; var j = i &#43; aLen;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (document.cookie.substring(i, j) == arg) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return getCookieVal(j);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; i = document.cookie.indexOf(&quot; &quot;, i) &#43; 1;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (i == 0) break;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return;<br>
&nbsp; &nbsp; &nbsp; &nbsp; }<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; function getCookieVal(offset) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; var endStr = document.cookie.indexOf(&quot;;&quot;, offSet);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (endStr == -1) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; endStr = document.cookie.length;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return unescape(document.cookie.substring(offSet, endStr));<br>
&nbsp; &nbsp; &nbsp; &nbsp; }<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Assign values to cookie variable.<br>
&nbsp; &nbsp; &nbsp; &nbsp;function setCookie(name, value) {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; document.cookie = name &#43; &quot;=&quot; &#43; escape(value);<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/script&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[/code]<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The VB code:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[code]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Public LoginDate As String<br>
&nbsp; &nbsp; &nbsp; &nbsp;Public ExpressDate As String<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Check session is expire or timeout.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If Session(&quot;username&quot;) Is Nothing Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Redirect(&quot;LoginPage.aspx?info=0&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Get user login time or last activity time.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim [date] As DateTime = DateTime.Now<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;LoginDate = [date].ToString(&quot;u&quot;, DateTimeFormatInfo.InvariantInfo).Replace(&quot;Z&quot;, &quot;&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim sessionTimeout As Integer = Session.Timeout<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim dateExpress As DateTime = [date].AddMinutes(sessionTimeout)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;ExpressDate = dateExpress.ToString(&quot;u&quot;, DateTimeFormatInfo.InvariantInfo).Replace(&quot;Z&quot;, &quot;&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp;End Sub<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[/code]<br>
<br>
Step 5. Build the application and you can debug it.<br>
/////////////////////////////////////////////////////////////////////////////<br>
References:<br>
<br>
MSDN: ASP.NET User Controls<br>
http://msdn.microsoft.com/en-us/library/y6wb1a0e.aspx<br>
<br>
MSDN: HttpCookie Class<br>
http://msdn.microsoft.com/en-us/library/system.web.httpcookie.aspx<br>
/////////////////////////////////////////////////////////////////////////////<br>
