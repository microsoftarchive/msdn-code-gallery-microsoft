# HTTP Listener ConsoleApp to Parse the JSON from the Request Body
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- .NET
- .NET Framework
- .NET Framework 4.5
## Topics
- ConsoleApp
## Updated
- 08/27/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<h1>HTTP Listener ConsoleApp to Parse the JSON from the Request Body</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The C sharp sample code of console application developed in .NET Framework 4.5 helps us to create an HTTP listener which listens to the incoming request and parses the JSON from its Request Body. The application also has an optional sample
 snippet which can log the parsed JSON to the SQL server database.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1: Open the &quot;CSharp_HTTPListener_ParseJsonRequest.sln&quot; file using VS 2013 or above.
<br>
Step2: Modify the listenning port. You also need to modify the JSON fields in the code which you are reading from the request body. If you are planning to save the pasrsed JSON request to the SQL server database then you have to modify the SQL connection string.
<br>
Step3: Build the code either by cliking &quot;ctrl&#43; shift&#43; B&quot; or by pressing F6 button.
<br>
Step4: Execute the code either by clicking the F5 button or Ctrl &#43; F5 <br>
Step5: A console appears which indicates that it is waiting for the requests. <br>
Step6: Launch the browser or any request composer and access the application over http://localhost/ and ensure that you send a request with JSON Content within its request body.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Below is the code snippet which creates HTTPListener class</p>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp">var&nbsp;httpListener&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpListener&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Prefixes&nbsp;=&nbsp;{&nbsp;<span class="cs__string">&quot;http://&#43;:80/&quot;</span>&nbsp;},&nbsp;
&nbsp;
};</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>Below is the code snippet that waits till the user sends his request and reads the request stream</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp">httpListener.Start();&nbsp;
var&nbsp;context&nbsp;=&nbsp;httpListener.GetContext();&nbsp;
var&nbsp;request&nbsp;=&nbsp;context.Request;&nbsp;
<span class="cs__keyword">while</span>&nbsp;(<span class="cs__keyword">true</span>)&nbsp;
{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;System.IO.Stream&nbsp;body&nbsp;=&nbsp;request.InputStream;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;System.Text.Encoding&nbsp;encoding&nbsp;=&nbsp;request.ContentEncoding;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;System.IO.StreamReader&nbsp;reader&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.IO.StreamReader.aspx" target="_blank" title="Auto generated link to System.IO.StreamReader">System.IO.StreamReader</a>(body,&nbsp;encoding);&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<h2>More Information</h2>
<p>The following article will provide a background on HTTPListener:<br>
<a href="https://msdn.microsoft.com/en-in/library/system.net.httplistener(v=vs.110).aspx">https://msdn.microsoft.com/en-in/library/system.net.httplistener(v=vs.110).aspx</a></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
