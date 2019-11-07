# How to post json data Using HttpClient in Win10 UWP
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- .NET
- Networking
- Universal Windows App Development
## Topics
- WCF
- WebClient
- UWP
- Post json
## Updated
- 08/31/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">&lt;How to
</span><span style="font-weight:bold; font-size:14pt">post</span><span style="font-weight:bold; font-size:14pt">
</span><span style="font-weight:bold; font-size:14pt">json</span><span style="font-weight:bold; font-size:14pt"> data using
</span><span style="font-weight:bold; font-size:14pt">HttpClient</span><span style="font-weight:bold; font-size:14pt"> in Win10 UWP apps</span><span style="font-weight:bold; font-size:14pt">&gt;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>This</span><span> sample demonstrates how to post </span><span>json</span><span> data to web server using
</span><span>HttpClient</span><span> in Win10 UWP apps.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>This sample is upgraded from </span><a href="https://code.msdn.microsoft.com/How-to-use-HttpClient-to-b9289836" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">How to use HTTPClient to post Json data to
</span><span style="color:#0563c1; text-decoration:underline">W</span><span style="color:#0563c1; text-decoration:underline">ebService in Windows Store apps</span></a><span>.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>To open and run this sample, ensure the following requisites ha</span><span>ve</span><span> been reached:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Microsoft Windows 10(10.0.14393) or above.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Microsoft Visual Studio 2015 Update3 or later version(s).</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Microsoft Visual Studio
</span><span>ha</span><span>s i</span><span>nstalled UWP developer component.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Microsoft Visual Studio
</span><span>ha</span><span>s</span><span> i</span><span>nstalled Web developer component.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">B</span><span style="font-weight:bold; font-size:12pt">uilding
</span><span style="font-weight:bold; font-size:12pt">the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Open the sample solution &ldquo;</span><span style="font-weight:bold">CSPostJsonViaHttpClient.sln</span><span>&rdquo; using Visual Studio.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>In Solution Explorer, right click project &ldquo;</span><span style="font-weight:bold">CSPostJsonViaHttpClient</span><span>&rdquo; and select
</span><span style="font-weight:bold">Set as </span><span style="font-weight:bold">StartUp</span><span style="font-weight:bold"> Project</span><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="159053-image.png" alt="" width="402" height="383" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Right click project &ldquo;</span><span style="font-weight:bold">JSONWCFService</span><span>&rdquo; and choose
</span><span style="font-weight:bold">Rebuild</span><span>, to restore </span><span>references</span><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="159054-image.png" alt="" width="479" height="207" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">R</span><span style="font-weight:bold; font-size:12pt">unning the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Use Visual studio to open the sample solution, then press
</span><span style="font-weight:bold">F5 Key</span><span> or select </span><span style="font-weight:bold">Debug -&gt; Start Debugging</span><span> from the menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>When the app is running, you can see</span><span> WCF service
</span><span>running.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="159055-image.png" alt="" width="506" height="189" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>A</span><span>nd the UWP sample app is running</span><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="159056-image.png" alt="" width="554" height="649" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click the button &ldquo;</span><span style="font-weight:bold">Start</span><span>&rdquo;.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="159057-image.png" alt="" width="554" height="649" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>The app will send a
</span><span>json</span><span> data to WCF service, and set result of WCF service to UI.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="159058-image.png" alt="" width="506" height="658" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">U</span><span style="font-weight:bold; font-size:12pt">sing the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-autospace:none">
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">httpClient = new HttpClient();
string resourceAddress = &quot;http://localhost:46789/WCFService.svc/GetData&quot;;
 
string jsonStr = JsonConvert.SerializeObject(new { Name = ViewModel.Name, Age = ViewModel.Age });
 
httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(&quot;application/json&quot;));
HttpResponseMessage wcfResponse = await httpClient.PostAsync(resourceAddress, new StringContent(jsonStr, Encoding.UTF8, &quot;application/json&quot;));
 
string responseText = await wcfResponse.Content.ReadAsStringAsync();
ViewModel.ServerResult = responseText;</pre>
<div class="preview">
<pre class="csharp">httpClient&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpClient();&nbsp;
<span class="cs__keyword">string</span>&nbsp;resourceAddress&nbsp;=&nbsp;<span class="cs__string">&quot;http://localhost:46789/WCFService.svc/GetData&quot;</span>;&nbsp;
&nbsp;&nbsp;
<span class="cs__keyword">string</span>&nbsp;jsonStr&nbsp;=&nbsp;JsonConvert.SerializeObject(<span class="cs__keyword">new</span>&nbsp;{&nbsp;Name&nbsp;=&nbsp;ViewModel.Name,&nbsp;Age&nbsp;=&nbsp;ViewModel.Age&nbsp;});&nbsp;
&nbsp;&nbsp;
httpClient.DefaultRequestHeaders.Accept.Add(<span class="cs__keyword">new</span>&nbsp;System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(<span class="cs__string">&quot;application/json&quot;</span>));&nbsp;
HttpResponseMessage&nbsp;wcfResponse&nbsp;=&nbsp;await&nbsp;httpClient.PostAsync(resourceAddress,&nbsp;<span class="cs__keyword">new</span>&nbsp;StringContent(jsonStr,&nbsp;Encoding.UTF8,&nbsp;<span class="cs__string">&quot;application/json&quot;</span>));&nbsp;
&nbsp;&nbsp;
<span class="cs__keyword">string</span>&nbsp;responseText&nbsp;=&nbsp;await&nbsp;wcfResponse.Content.ReadAsStringAsync();&nbsp;
ViewModel.ServerResult&nbsp;=&nbsp;responseText;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">More information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>HttpClient</span><span> Class</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="http://msdn.microsoft.com/en-us/library/system.net.http.httpclient.aspx" style="text-decoration:none"><span style="color:#960bb4">http://msdn.microsoft.com/en-us/library/system.net.http.httpclient.aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Json.NET</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="http://www.newtonsoft.com/json" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://www.newtonsoft.com/json</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a name="_GoBack"></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
