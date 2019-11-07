# How to create a HTTP Custom Client
## Requires
- Visual Studio 2017
## License
- Apache License, Version 2.0
## Technologies
- C#
- .NET
- .NET Framework
- .NET Framework 4.5.1
- Languages
## Topics
- C#
## Updated
- 06/29/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<h1>Http Custom client for sending a Request to a Web Server and Printing the Response in C#</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The C# sample code of console application developed in visual studio 2017 helps us to send a Request to a Web Server and Printing the Response in C#.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1: Open the &quot;HttpCustomClients.sln&quot; file using VS 2017.
<br>
<br>
<img src="175031-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step2: Build the code eith by cliking &quot;ctrl&#43; shift&#43; B&quot; or by pressing F6 button. <br>
<br>
<img src="175032-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step3: Execute the code by pressing F5. <br>
<br>
<img src="175033-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step4: In console you will see the response we got from the web server <br>
<br>
<img src="175034-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Below is the code snippet which Request to a Web Server and Printing the Response in C#</p>
<p class="MsoNormal"></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">static void Main(string[] args)
{
	HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri(&quot;http://localhost/mayur.htm&quot;));//Create a HttpWebRequest object 

	httpWebRequest.Method = &quot;GET&quot;;//Set the Method 

	httpWebRequest.KeepAlive = true;//Set Keep Alive

	HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();//Get the Response

.............
.............
.............

 }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Main(<span class="cs__keyword">string</span>[]&nbsp;args)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpWebRequest&nbsp;httpWebRequest&nbsp;=&nbsp;(HttpWebRequest)&nbsp;WebRequest.Create(<span class="cs__keyword">new</span>&nbsp;Uri(<span class="cs__string">&quot;http://localhost/mayur.htm&quot;</span>));<span class="cs__com">//Create&nbsp;a&nbsp;HttpWebRequest&nbsp;object&nbsp;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpWebRequest.Method&nbsp;=&nbsp;<span class="cs__string">&quot;GET&quot;</span>;<span class="cs__com">//Set&nbsp;the&nbsp;Method&nbsp;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpWebRequest.KeepAlive&nbsp;=&nbsp;<span class="cs__keyword">true</span>;<span class="cs__com">//Set&nbsp;Keep&nbsp;Alive</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpWebResponse&nbsp;httpWebResponse&nbsp;=&nbsp;(HttpWebResponse)httpWebRequest.GetResponse();<span class="cs__com">//Get&nbsp;the&nbsp;Response</span>&nbsp;
&nbsp;
.............&nbsp;
.............&nbsp;
.............&nbsp;
&nbsp;
&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
