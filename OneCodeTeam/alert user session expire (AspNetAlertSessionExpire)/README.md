# alert user session expire (AspNetAlertSessionExpire)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- Session
## Updated
- 05/10/2012
## Description
========================================================================<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; CSASPNETAlertSessionExpired Overview<br>
========================================================================<br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
Summary:<br>
<br>
The project illustrates how to design a simple user control, which is used to <br>
alert the user when the session is about to expired. <br>
<br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
Demo the Sample. <br>
<br>
Open the CSASPNETAlertSessionExpired.sln with Visual Studio2010, extend the <br>
CSASPNETAlertSessionExpired web application node and press F5 to test the application.<br>
<br>
If you want to have a further test, please follow the demonstration step below.<br>
<br>
Step 1: Press F5 to open the default.aspx.<br>
<br>
Step 2: By default, we can see a &quot;GetSessionState&quot; button, you can click the <br>
button to query the SessionState.<br>
<br>
Step 3: The session is set expired in 1 minute, So idle the page for 30 seconds.<br>
<br>
Step 4: After 30 seconds, you will see an alert message &quot;Your session will be expired<br>
after 30 seconds, would you like to extend the session time?&quot; You can choose <br>
&quot;Yes&quot; or &quot;No&quot;.<br>
<br>
Step 5: If you choose &quot;Yes&quot;, the session will be reset. You can click the &quot;GetSessionState&quot;
<br>
button to query the session status, you will see the SessionState: Existing.<br>
<br>
Step 6: If you choose &quot;No&quot;, the &quot;GetSessionState&quot; button will be disabled temporarily.<br>
When the session is really expired you can click the &quot;GetSessionState&quot; button, <br>
the page will redirect to the SessionExpired page.<br>
<br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
Code Logical:<br>
<br>
We use jQuery, ASP.NET AJAX at client side.<br>
In this sample, we add a SessionExpired user control and Script Manager on<br>
the Master page. It will display an alert message if user idle for long time, user
<br>
can choose whether to extend the session before it's expired or not.<br>
<br>
Step 1. &nbsp;Create an ASP.NET Web Application. In this sample it is <br>
&nbsp; &nbsp; &nbsp; &nbsp; &quot;CSASPNETAlertSessionExpired&quot;.<br>
<br>
Step 2. &nbsp;Right-click the CSASPNETAlertSessionExpired , and click Add -&gt; New Item -&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; New Folder &quot;Controls&quot;.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Right-click The Controls directory, and click Add-&gt; New Item -&gt; Web<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;User Control name as &quot;SessionExpired.ascx&quot;:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * SessionExpired.ascx.cs: <br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1、The SessionExpired class inherits from
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ICallHandlerEvent which is used to indicate that the control can be<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; the target of a callback event on the server. Then it will extend<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; the Session's lifetime.
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 2、Register the Session's timeout value to the client so that user<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; can extend the Session's lifetime before it expired.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 3、Verify the Session is expired or not.<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; * SessionExpired.ascx:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; It will get the Session's timeout value and request the server<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; interval by async method.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; if (!IsPostBack)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Session[&quot;SessionForTest&quot;] = &quot;SessionForTest&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; [/CODE]<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; Add the Session state judgment like:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; if (Session[&quot;SessionForTest&quot;] == null)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;lbSessionState.Text = SessionStates.Expired.ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;lbSessionState.Text = SessionStates.Exist.ToString();<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; [/CODE]<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<br>
Step 3. &nbsp;Right-click the CSASPNETAlertSessionExpired , and click Add -&gt; New Item -&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; New Folder &quot;Util&quot;.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Create a menu type which is used to output the session state consistent.<br>
<br>
Step 4. &nbsp;Open the Site.Master,(If there is no Site.Master, create one.)<br>
&nbsp; &nbsp; &nbsp; &nbsp; add a user control reference and<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; a script reference like below.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&lt;uc:SessionExpired ID=&quot;ucSessionExpired&quot; runat=&quot;server&quot; /&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [/CODE]<br>
<br>
Step 5. &nbsp;Open the Default.aspx(If there is no Default.aspx, create one which
<br>
&nbsp; &nbsp; &nbsp; &nbsp; is a Web Form using Master Page)<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Add the scripts and css reference &nbsp;like below:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;script &nbsp;type=&quot;text/javascript&quot; &nbsp;language=&quot;javascript&quot; src=&quot;Scripts/jquery-1.4.1-vsdoc.js&quot;&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/script&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &lt;script type=&quot;text/javascript&quot; language=&quot;javascript&quot; src=&quot;Scripts/SessionExpired.js&quot;&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/script&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &lt;link href=&quot;Styles/SessionExpired.css&quot; &nbsp;rel=&quot;stylesheet&quot; type=&quot;text/css&quot; /&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;We use these references to make this sample to write code easier<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; and looks better.<br>
<br>
Step 6. &nbsp;Open the Web.config(If there is no Web.config, create one.)<br>
&nbsp; &nbsp; &nbsp; &nbsp; Set the Session timeout value like below:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;system.web&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;sessionState timeout=&quot;1&quot;&gt;&lt;/sessionState&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &lt;/system.web&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [/CODE]<br>
<br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
References:<br>
<br>
MSDN: How to: Implement Callbacks in ASP.NET Web Pages<br>
http://msdn.microsoft.com/en-us/library/ms366518.aspx<br>
<br>
MSDN: How to: Save Values in Session State<br>
http://msdn.microsoft.com/en-us/library/6ad7zeeb(VS.90).aspx<br>
<br>
MSDN: ASP.NET Session State Overview<br>
http://msdn.microsoft.com/en-us/library/ms178581.aspx<br>
<br>
MSDN: Working with ASP.NET Master Pages Programmatically<br>
http://msdn.microsoft.com/en-us/library/c8y19k6h.aspx<br>
<br>
<br>
/////////////////////////////////////////////////////////////////////////////<br>
