# Remote certificate validation for http webrequest using System.Net
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- .NET
- .NET Framework
- .NET Framework 4.5
## Topics
- C#
## Updated
- 01/05/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://aka.ms/onecodesampletopbanner1" alt="">
</a></div>
<h1>Remote certificate validation for http webrequest using <a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Net.aspx" target="_blank" title="Auto generated link to System.Net">System.Net</a></h1>
<h2>Introduction</h2>
<p class="MsoNormal">The C sharp asp.net web application sample code developed in .NET Framework 4.5 helps us to remotely validate the certificate of the URI accessed using the WebRequest class in System.Net.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">&nbsp;</p>
<h5>Step 1:</h5>
<p>Open the &quot;CSharpHTTPClientSSL.sln&quot; file using VS 2015. <br>
<br>
</p>
<h5>Step 2:</h5>
<p>Make sure that you modify the URL according to an https website. <br>
<br>
</p>
<h5>Step 3:</h5>
<p>Execute the code either by clicking the F5 button or by clicking Ctrl &#43; F5.<br>
<br>
</p>
<h5>Step 4:</h5>
<p>Your website comes up without any exception due to remote certification invalidation.
<br>
<br>
</p>
<p>&nbsp;</p>
<h2>Using the Code</h2>
<p class="MsoNormal">&nbsp;</p>
<h5>Below code snippet initializes a new WebRequest instance for the specified URI</h5>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">(HttpWebRequest)WebRequest.Create(<span class="cs__string">&quot;https://www.azure.com&quot;</span>);</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<p>&nbsp;</p>
<h5>Below code verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.</h5>
<h5>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;AcceptAllCertifications(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;System.Security.Cryptography.X509Certificates.X509Certificate&nbsp;certification,&nbsp;System.Security.Cryptography.X509Certificates.X509Chain&nbsp;chain,&nbsp;System.Net.Security.SslPolicyErrors&nbsp;sslPolicyErrors)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">true</span>;&nbsp;
}</pre>
</div>
</div>
</div>
</h5>
<h2>More Information</h2>
<p class="MsoNormal">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
