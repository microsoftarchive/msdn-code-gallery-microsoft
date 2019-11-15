# Desktop Application developed in C# to upload a file using FTPWebRequest
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- .NET
- .NET Framework 4
- .NET Framework
## Topics
- C#
## Updated
- 01/05/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://aka.ms/onecodesampletopbanner1" alt="">
</a></div>
<h1>Desktop Application developed in C# to upload a file using FTPWebRequest</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This C# sample code of desktop application developed in .NET Framework 4.0 helps us to upload a file using System.Net.FtpWebRequest. This also makes sure that the request goes over SSL using EnableSsl property available in this class.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1: Open the &quot;FileUpload_FTPWebRequest.sln&quot; file using VS 2012 or above.
<br>
Step2: Modify the FTP URL and the folder structure (Radio buttons) of the server where you would like the file to be uploaded.
<br>
Step3: Modify the username and password of the user who would have the access to upload the file to the FTP server
<br>
Step4: Build the code either by clicking &quot;ctrl&#43; shift&#43; B&quot; or by pressing F6 button.
<br>
Step5: Execute the code either by clicking the F5 button or by clicking Ctrl &#43; F5</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Here&nbsp;is the code snippet for Using the <a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.NET.aspx" target="_blank" title="Auto generated link to System.NET">System.NET</a> FTPWebRequest class</p>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>


<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Dim</span>&nbsp;ftp&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;FtpWebRequest&nbsp;=&nbsp;<span class="visualBasic__keyword">DirectCast</span>(FtpWebRequest.Create(<span class="visualBasic__string">&quot;ftp://localhost/httpdocs/&quot;</span>&nbsp;&#43;&nbsp;DocumentDirectory.Text&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;/&quot;</span>&nbsp;&#43;&nbsp;fileName),&nbsp;FtpWebRequest)</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>Here&nbsp;is the code snippet that validates the remote server certificate</p>
<h5>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>


<div class="preview">
<pre class="js">ServicePointManager.ServerCertificateValidationCallback&nbsp;=&nbsp;New&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Net.Security.RemoteCertificateValidationCallback.aspx" target="_blank" title="Auto generated link to System.Net.Security.RemoteCertificateValidationCallback">System.Net.Security.RemoteCertificateValidationCallback</a>(AddressOf&nbsp;AcceptAllCertifications)&nbsp;&nbsp;</pre>
</div>
</div>
</div>
</h5>
<h2>More Information</h2>
<pre>The following article will provide a background on <a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Net.FtpWebRequest.aspx" target="_blank" title="Auto generated link to System.Net.FtpWebRequest">System.Net.FtpWebRequest</a> and its available properties:
https://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest(v=vs.110).aspx
https://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.method(v=vs.110).aspx</pre>
<p class="MsoNormal">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
