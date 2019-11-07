# .NET HTTPModule to remove the Server Header from the response
## Requires
- Visual Studio 2017
## License
- Apache License, Version 2.0
## Technologies
- .NET
- .NET Framework
## Topics
- VB.Net
- CSharp
## Updated
- 08/23/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<h1>CSharp.NET HTTPModule to remove the Server Header from the response</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The CSharp.NET HTTPModule to remove the Server Header from the response of the application hosted on IIS 7.x and above. This module hooks up in the PreSendRequestHeaders event and before sending the response back to the client it ensures
 that the Server header is removed.</p>
<h2>Running the Sample</h2>
<p>Step1: Make sure that you have IIS 7.X and above version on your server <br>
Step2: Open the CSharpResponseHeaderRemoval.sln from VS 2015 or above and host it on IIS.
<br>
Step3: You need to add the below to your web.config so that this module (dll) will be invoked.&nbsp;</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">&lt;system.webServer&gt;
    &lt;modules&gt;
        &lt;add name=&quot;CSharpResponseHeaderRemoval&quot; type=&quot;CSharpResponseHeaderRemoval.header&quot; /&gt;
    &lt;/modules&gt;
&lt;/system.webServer&gt;</pre>
<pre class="hidden">&lt;system.webServer&gt;
  &lt;modules&gt;
     &lt;add name=&quot;header&quot; type=&quot;header&quot; /&gt;
  &lt;/modules&gt;
&lt;/system.webServer&gt;</pre>
<div class="preview">
<pre class="csharp">&lt;system.webServer&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;modules&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;add&nbsp;name=<span class="cs__string">&quot;CSharpResponseHeaderRemoval&quot;</span>&nbsp;type=<span class="cs__string">&quot;CSharpResponseHeaderRemoval.header&quot;</span>&nbsp;/&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;/modules&gt;&nbsp;
&lt;/system.webServer&gt;</pre>
</div>
</div>
</div>
<p></p>
<p>Step4: Then access the home page of the application and you can see that the Server name from the Response header will be removed.</p>
<h2>Using the Code</h2>
<p>Below is the code snippet that makes sure that we are registered the necessary events:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>
<pre class="hidden">public void PreSendRequestHeaders(object sender, EventArgs e)
{
    HttpApplication httpApplication = (HttpApplication)sender;
    HttpApplication application = (HttpApplication)sender;
    HttpContext context = application.Context;
    context.Response.Headers.Remove(&quot;Server&quot;);           
}</pre>
<pre class="hidden">Public Sub PreSendRequestHeaders(ByVal s As Object, ByVal e As EventArgs)
     Dim app As HttpApplication = CType(s, HttpApplication)
     app.Context.Response.Headers.Remove(&quot;Server&quot;)
End Sub</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;PreSendRequestHeaders(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpApplication&nbsp;httpApplication&nbsp;=&nbsp;(HttpApplication)sender;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpApplication&nbsp;application&nbsp;=&nbsp;(HttpApplication)sender;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpContext&nbsp;context&nbsp;=&nbsp;application.Context;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;context.Response.Headers.Remove(<span class="cs__string">&quot;Server&quot;</span>);&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
}</pre>
</div>
</div>
</div>
<h2>More Information</h2>
<p class="MsoNormal">Below article talks about the HTTP Modules</p>
<p class="MsoNormal"><a href="https://msdn.microsoft.com/en-us/library/bb398986.aspx">https://msdn.microsoft.com/en-us/library/bb398986.aspx</a><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
