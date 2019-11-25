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
- 11/20/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://aka.ms/onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><a name="OLE_LINK1"></a><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">Auto redirect to login page when Session is expired</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
The project illustrates how to develop an asp.net code-sample that will be&nbsp;redirect to login page when page Session is expired or time out automatically.&nbsp;It will ask the user to extend the Session at one minutes beforeexpiring. If the user does not
 has any actions, the web page will be redirectedto login page automatically, and also note that it need to work in one ormore browser's tabs.&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
Please follow these demonstration steps below.</p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<strong>Step 1:</strong> Open the CSASPNETAutoRedirectLoginPage.sln.</p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<strong>Step 2: </strong>Expand the CSASPNETAutoRedirectLoginPage web application and press Ctrl &#43; F5 to show the LoginPage.aspx page.</p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<strong>Step 3:</strong> You will see login page and input the user name and pass word, and then click the login button to redirect the user page.</p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<strong>Step 4: </strong>Now you can open more user pages or refresh the user page for persisting user's activity.&nbsp;</p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<strong>Step 5: </strong>When you stop operating the pages and wait for 1 minute, the all web pages of code-sample will be redirected the login page automatically. For easy testing, the code-sample set the session's timeout to 1 minute.</p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<strong>Step 6: </strong>Validation finished.</p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<strong>Step 1: </strong>Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2010 or Visual Web Developer 2010. Name it as &quot;CSASPNETAutoRedirectLoginPage&quot;.</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<strong>Step 2:&nbsp; </strong>Add a web form named &quot;LoginUser.aspx&quot; for user login in website, and this page can prevent users who want to skip the login step by visit specified pages.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Page_Load(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Prevent&nbsp;the&nbsp;users&nbsp;who&nbsp;try&nbsp;to&nbsp;skip&nbsp;the&nbsp;login&nbsp;step&nbsp;by&nbsp;visit&nbsp;specified&nbsp;page.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(!Page.IsPostBack)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Session.Abandon();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(Request.QueryString[<span class="cs__string">&quot;info&quot;</span>]&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;message&nbsp;=&nbsp;Request.QueryString[<span class="cs__string">&quot;info&quot;</span>].ToString();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(message&nbsp;==&nbsp;<span class="cs__string">&quot;0&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Write(<span class="cs__string">&quot;you&nbsp;need&nbsp;login&nbsp;first&nbsp;to&nbsp;visit&nbsp;user&nbsp;page.&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
&nbsp;
<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;User&nbsp;login&nbsp;method.</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;sender&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;e&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;btnLogin_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;username&nbsp;=&nbsp;tbUserName.Text.Trim();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(tbUserName.Text.Trim()&nbsp;==&nbsp;<span class="cs__string">&quot;username&quot;</span>&nbsp;&amp;&amp;&nbsp;tbPassword.Text.Trim()&nbsp;==&nbsp;<span class="cs__string">&quot;password&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Session[<span class="cs__string">&quot;username&quot;</span>]&nbsp;=&nbsp;username;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Redirect(<span class="cs__string">&quot;UserPage1.aspx&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Write(<span class="cs__string">&quot;User&nbsp;name&nbsp;or&nbsp;pass&nbsp;word&nbsp;error.&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<strong>Step 3:</strong> Add two user pages and named them as &quot;UserPage1.aspx&quot;, &quot;UserPage2.aspx&quot;. These two user pages are use to observe the web application and redirect the login page automatically. You can also open multiple tabs or windows in your browser.&nbsp;<br>
<strong>Step 4: </strong>Add a user control on the root dictionary and drag and drop it in the every user page for redirecting to the login page when session is expired or time out. The user control's JavaScript code and C# code will be like this:</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>

<div class="preview">
<pre class="js">&lt;script&nbsp;type=<span class="js__string">&quot;text/javascript&quot;</span>&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;timeRefresh;&nbsp;
<span class="js__statement">var</span>&nbsp;timeInterval;&nbsp;
<span class="js__statement">var</span>&nbsp;currentTime;&nbsp;
<span class="js__statement">var</span>&nbsp;expressTime;&nbsp;
&nbsp;
expressTime&nbsp;=&nbsp;<span class="js__string">&quot;&lt;%=ExpressDate&nbsp;%&gt;&quot;</span>;&nbsp;
currentTime&nbsp;=&nbsp;<span class="js__string">&quot;&lt;%=LoginDate&nbsp;%&gt;&quot;</span>;&nbsp;
setCookie(<span class="js__string">&quot;express&quot;</span>,&nbsp;expressTime);&nbsp;
timeRefresh&nbsp;=&nbsp;setInterval(<span class="js__string">&quot;Refresh()&quot;</span>,&nbsp;<span class="js__num">1000</span>);&nbsp;
&nbsp;
<span class="js__sl_comment">//&nbsp;Refresh&nbsp;this&nbsp;page&nbsp;to&nbsp;check&nbsp;session&nbsp;is&nbsp;expire&nbsp;or&nbsp;timeout.</span>&nbsp;
<span class="js__operator">function</span>&nbsp;Refresh()&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;current&nbsp;=&nbsp;getCookie(<span class="js__string">&quot;express&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;date&nbsp;=&nbsp;current.split(<span class="js__string">&quot;&nbsp;&quot;</span>)[<span class="js__num">0</span>];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;time&nbsp;=&nbsp;current.split(<span class="js__string">&quot;&nbsp;&quot;</span>)[<span class="js__num">1</span>];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;scriptDate&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__object">Date</span>();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;year&nbsp;=&nbsp;scriptDate.getFullYear();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;month&nbsp;=&nbsp;scriptDate.getMonth()&nbsp;&#43;&nbsp;<span class="js__num">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;day&nbsp;=&nbsp;scriptDate.getDate();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;hour&nbsp;=&nbsp;scriptDate.getHours();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;min&nbsp;=&nbsp;scriptDate.getMinutes();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;second&nbsp;=&nbsp;scriptDate.getSeconds();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(<span class="js__object">Date</span>.UTC(year,&nbsp;month,&nbsp;day,&nbsp;hour,&nbsp;min,&nbsp;second)&nbsp;&gt;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__object">Date</span>.UTC(date.split(<span class="js__string">&quot;-&quot;</span>)[<span class="js__num">0</span>],&nbsp;date.split(<span class="js__string">&quot;-&quot;</span>)[<span class="js__num">1</span>],&nbsp;date.split(<span class="js__string">&quot;-&quot;</span>)[<span class="js__num">2</span>],&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;time.split(<span class="js__string">&quot;:&quot;</span>)[<span class="js__num">0</span>],&nbsp;time.split(<span class="js__string">&quot;:&quot;</span>)[<span class="js__num">1</span>],&nbsp;time.split(<span class="js__string">&quot;:&quot;</span>)[<span class="js__num">2</span>]))&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;clearInterval(timeRefresh);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Redirect();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span>&nbsp;
&nbsp;
<span class="js__operator">function</span>&nbsp;Redirect()&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;window.location.replace(<span class="js__string">&quot;LoginPage.aspx&quot;</span>);&nbsp;
<span class="js__brace">}</span>&nbsp;
&nbsp;
<span class="js__sl_comment">//&nbsp;Retrieve&nbsp;cookie&nbsp;by&nbsp;name.</span>&nbsp;
<span class="js__operator">function</span>&nbsp;getCookie(name)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;arg&nbsp;=&nbsp;name&nbsp;&#43;&nbsp;<span class="js__string">&quot;=&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;aLen&nbsp;=&nbsp;arg.length;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;cLen&nbsp;=&nbsp;document.cookie.length;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;i&nbsp;=&nbsp;<span class="js__num">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">while</span>&nbsp;(i&nbsp;&lt;&nbsp;cLen)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;j&nbsp;=&nbsp;i&nbsp;&#43;&nbsp;aLen;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(document.cookie.substring(i,&nbsp;j)&nbsp;==&nbsp;arg)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;getCookieVal(j);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;i&nbsp;=&nbsp;document.cookie.indexOf(<span class="js__string">&quot;&nbsp;&quot;</span>,&nbsp;i)&nbsp;&#43;&nbsp;<span class="js__num">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(i&nbsp;==&nbsp;<span class="js__num">0</span>)&nbsp;<span class="js__statement">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>;&nbsp;
<span class="js__brace">}</span>&nbsp;
&nbsp;
<span class="js__operator">function</span>&nbsp;getCookieVal(offset)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;endStr&nbsp;=&nbsp;document.cookie.indexOf(<span class="js__string">&quot;;&quot;</span>,&nbsp;offSet);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(endStr&nbsp;==&nbsp;-<span class="js__num">1</span>)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;endStr&nbsp;=&nbsp;document.cookie.length;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;unescape(document.cookie.substring(offSet,&nbsp;endStr));&nbsp;
<span class="js__brace">}</span>&nbsp;
&nbsp;
<span class="js__sl_comment">//&nbsp;Assign&nbsp;values&nbsp;to&nbsp;cookie&nbsp;variable.</span>&nbsp;
<span class="js__operator">function</span>&nbsp;setCookie(name,&nbsp;value)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;document.cookie&nbsp;=&nbsp;name&nbsp;&#43;&nbsp;<span class="js__string">&quot;=&quot;</span>&nbsp;&#43;&nbsp;escape(value);&nbsp;
<span class="js__brace">}</span>&nbsp;
&lt;/script&gt;</pre>
</div>
</div>
</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;loginDate;&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;expressDate;&nbsp;
<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Page_Load(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Check&nbsp;session&nbsp;is&nbsp;expire&nbsp;or&nbsp;timeout.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(Session[<span class="cs__string">&quot;username&quot;</span>]&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Redirect(<span class="cs__string">&quot;LoginPage.aspx?info=0&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Get&nbsp;user&nbsp;login&nbsp;time&nbsp;or&nbsp;last&nbsp;activity&nbsp;time.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DateTime&nbsp;date&nbsp;=&nbsp;DateTime.Now;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;loginDate&nbsp;=&nbsp;date.ToString(<span class="cs__string">&quot;u&quot;</span>,&nbsp;DateTimeFormatInfo.InvariantInfo).Replace(<span class="cs__string">&quot;Z&quot;</span>,&nbsp;<span class="cs__string">&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;sessionTimeout&nbsp;=&nbsp;Session.Timeout;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DateTime&nbsp;dateExpress&nbsp;=&nbsp;date.AddMinutes(sessionTimeout);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;expressDate&nbsp;=&nbsp;dateExpress.ToString(<span class="cs__string">&quot;u&quot;</span>,&nbsp;DateTimeFormatInfo.InvariantInfo).Replace(<span class="cs__string">&quot;Z&quot;</span>,&nbsp;<span class="cs__string">&quot;&quot;</span>);&nbsp;
}</pre>
</div>
</div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<strong>Step 5: </strong>Build the application and you can debug it.</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:12pt; font-weight:bold">More information</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<a href="http://msdn.microsoft.com/en-us/library/y6wb1a0e.aspx">MSDN: ASP.NET User Controls</a></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<a href="http://msdn.microsoft.com/en-us/library/system.web.httpcookie.aspx">MSDN: HttpCookie Class</a></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
