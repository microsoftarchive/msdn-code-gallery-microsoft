# Auto user login in ASP.NET site w/ HttpWebRequest (CSASPNETAutoLogin)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- HttpWebRequest
- Auto login
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>CSASPNETAutoLogin Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The project illustrates how to log a user into site using HttpWebRequest Class <br>
without filling username and password box.<br>
<br>
</p>
<h3>Demo</h3>
<p style="font-family:Courier New"><br>
Click the CSASPNETAutoLogin.sln directly and open the CSASPNETAutoLogin website <br>
to test the page directly.<br>
<br>
If you want to have a further test, please follow the demonstration steps <br>
below.<br>
<br>
Step 1: In the Login.aspx page, you fill your username and password,and click<br>
Login button. the page will prompt that you have login this page, and display your
<br>
name and password,it proves that you have login the page successfully, it is <br>
the normal method that we login the site.<br>
<br>
Step 2: In the AutoLogin.aspx page,you also fill your username and password, and<br>
click the AutoLogin button,the page will display the username and password.<br>
But in the codebehind, we take the httpwebrequest to submit the current<br>
usename and password to the login.aspx instead of the current page. it shows<br>
the same information with the login.aspx page, it proves that we submit the <br>
current username and password of autologin.aspx page to the login.aspx successfully.<br>
<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
Step 1. we request the login.aspx by using HttpWebRequest get method and<br>
get the return login page html code. <br>
<br>
Step 2. We substring the login page html code to get the __VIEWSTATE,<br>
__EVENTVALIDATION parameters.<br>
<br>
Step 3. We concat the __VIEWSTATE,__EVENTVALIDATION,Username, password and <br>
loginbutton to the whole string and convert it to byte array.<br>
<br>
Step 4. Submit the request data and get the return data and display it.<br>
<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: HttpWebRequest and HttpWebResponse Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx">http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx</a><br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.httpwebresponse.aspx">http://msdn.microsoft.com/en-us/library/system.net.httpwebresponse.aspx</a><br>
<br>
MSDN: <a target="_blank" href="&lt;a target=" href="http://msdn.microsoft.com/en-us/library/debx8sh9.aspx">
http://msdn.microsoft.com/en-us/library/debx8sh9.aspx</a>'&gt;<a target="_blank" href="http://msdn.microsoft.com/en-us/library/debx8sh9.aspx">http://msdn.microsoft.com/en-us/library/debx8sh9.aspx</a><br>
<a target="_blank" href="&lt;a target=" href="http://msdn.microsoft.com/en-us/library/debx8sh9.aspx">http://msdn.microsoft.com/en-us/library/debx8sh9.aspx</a>'&gt;<a target="_blank" href="http://msdn.microsoft.com/en-us/library/debx8sh9.aspx">http://msdn.microsoft.com/en-us/library/debx8sh9.aspx</a><br>
<br>
Automatic Login into a website in C#.net<br>
<a target="_blank" href="http://forums.asp.net/t/1507150.aspx">http://forums.asp.net/t/1507150.aspx</a><br>
<br>
How to use HttpWebRequest to send POST request to another web server<br>
<a target="_blank" href="http://www.netomatix.com/httppostdata.aspx">http://www.netomatix.com/httppostdata.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
