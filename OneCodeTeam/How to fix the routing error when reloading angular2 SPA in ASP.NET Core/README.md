# How to fix the routing error when reloading angular2 SPA in ASP.NET Core
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- .NET
- Web App Development
## Topics
- ASP.NET Core
- Angular2
- Routing
## Updated
- 03/28/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to
</span><span>f</span><span style="font-weight:bold; font-size:14pt">ix the routing error when reloading angular2 APS in ASP.NET Core</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>When you set up angular2 in your ASP.NET Core web application, you might notice that, when you&nbsp;route to an address like
</span><a href="http://localhost:5000/component" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://localhost:5000/component</span></a><span>, or when you reload this page, the result maybe a 404 error.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Since angular2 has a new routing role locally, like localhost:5000/home/index, same as ASP.NET's routing role, all requests will go through server's routing role first, and request address like localhost:5000/home/index does not exist in server,
 so the server will return a 404 error to you.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>This sample will demonstrate how to work around this problem.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>.Net Core 1.0 or later version(s). [</span><a href="http://go.microsoft.com/fwlink/?LinkID=798306" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">.Net
 Core &#43; Visual Studio tooling</span></a><span>]</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Microsoft Visual Studio 2015 update3 or above. [</span><a href="https://www.visualstudio.com/en-us/visual-studio-homepage-vs.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Visual
 Studio 2015 Home Page installer</span></a><span>]</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Node.js v5.0.0 or later version(s). [</span><a href="https://nodejs.org/en/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Node.js
 and NPM</span></a><span>]</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Npm 3.0.0 or above. [</span><a href="https://nodejs.org/en/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Node.js
 and NPM</span></a><span>]</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Building the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Press
</span><span style="font-weight:bold">win &#43; R</span><span> and type </span><span style="font-weight:bold">cmd</span><span> to open Command Line window.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="163591-image.png" alt="" width="527" height="305" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Local to the
</span><span style="font-weight:bold">wwwroot</span><span> folder of this solution.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="163592-image.png" alt="" width="797" height="203" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Type command
</span><span style="font-weight:bold">npm install</span><span>, then the npm will install the packages.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="163593-image.png" alt="" width="795" height="271" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Open the sample solution &ldquo;</span><span style="font-weight:bold">CSAngular2RoutingInAspNetCore.sln</span><span>&rdquo; using Visual Studio.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Right click the project &ldquo;</span><span style="font-weight:bold">CSAngular2RoutingInAspNetCore</span><span>&rdquo; and select Restore Packages.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="163594-image.png" alt="" width="586" height="422" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Press
</span><span style="font-weight:bold">F6 Key</span><span> or select </span><span style="font-weight:bold">Build -&gt; Build Solution</span><span> from the menu to build the sample.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Open the sample solution using Visual Studio, then press F5 Key or select
<strong>Debug -&gt; Start Debugging </strong>from the menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>When the web application is running, you can see this page&nbsp;on your browser.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="163595-image.png" alt="" width="617" height="319" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click the
</span><span style="font-weight:bold">Component2</span><span> link, and the view will route to Component2.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="163596-image.png" alt="" width="615" height="317" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Press
</span><span style="font-weight:bold">F5 Key</span><span> or click <strong>Reload</strong> button on your browser,
</span><span>the page will reload success.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="163597-image.png" alt="" width="615" height="317" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Startup.cs / Configure method.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>When you use static files case:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">app.Use(async&nbsp;(context,&nbsp;next)&nbsp;=&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;next();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(context.Response.StatusCode&nbsp;==&nbsp;<span class="cs__number">404</span>&nbsp;&amp;&amp;&nbsp;!Path.HasExtension(context.Request.Path.Value))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;context.Request.Path&nbsp;=&nbsp;<span class="cs__string">&quot;/index.html&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;next();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;})&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;.UseDefaultFiles(<span class="cs__keyword">new</span>&nbsp;DefaultFilesOptions&nbsp;{&nbsp;DefaultFileNames&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;List&lt;<span class="cs__keyword">string</span>&gt;&nbsp;{&nbsp;<span class="cs__string">&quot;index.html&quot;</span>&nbsp;}&nbsp;})&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;.UseStaticFiles()&nbsp;
.UseMvc();&nbsp;</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#000000; font-size:9.5pt">&nbsp;</span><span style="color:#000000; font-size:9.5pt">When you&nbsp;use mvc view case:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">app.UseStaticFiles()&nbsp;
&nbsp;&nbsp;&nbsp;.UseMvc(routes&nbsp;=&gt;&nbsp;
&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;routes.MapRoute(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;name:&nbsp;<span class="cs__string">&quot;default&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;template:&nbsp;<span class="cs__string">&quot;{controller=Home}/{action=Index}&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;routes.MapSpaFallbackRoute(<span class="cs__string">&quot;spa-fallback&quot;</span>,&nbsp;<span class="cs__keyword">new</span>&nbsp;{&nbsp;controller&nbsp;=&nbsp;<span class="cs__string">&quot;Home&quot;</span>,&nbsp;action&nbsp;=&nbsp;<span class="cs__string">&quot;Index&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;});&nbsp;</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">More information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Angular 2 Routing with ASP.NET Core 1 (aka ASP.NET 5)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>&nbsp;</span><a href="http://benjii.me/2016/01/angular2-routing-with-asp-net-core-1/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://benjii.me/2016/01/angular2-routing-with-asp-net-core-1/</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Routing in Angular 2 with ASP.NET Core</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>&nbsp;</span><a href="http://blog.nbellocam.me/2016/03/21/routing-angular-2-asp-net-core/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://blog.nbellocam.me/2016/03/21/routing-angular-2-asp-net-core/</span></a><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
