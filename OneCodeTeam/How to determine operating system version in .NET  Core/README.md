# How to determine operating system version in .NET  Core
## Requires
- Visual Studio 2017
## License
- Apache License, Version 2.0
## Technologies
- .NET
- .NET Core
## Topics
- OS
- Version
- .NET Core
## Updated
- 03/30/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><a name="OLE_LINK1"></a><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">How
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">to</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">determine operating system version in .NET Core</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>This sample demonstrates how </span><span>to</span><span> </span><span>determine operating system version in .NET Core</span><span>.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Visual Studio 2017 or above [</span><a href="https://www.visualstudio.com/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Visual
 Studio Home Page</span></a><span>]</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Building the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Open the sample solution &ldquo;</span><span style="font-size:10pt">CSNETCoreOSVersion</span><span style="font-size:10pt">.sln&rdquo;
 using Visual Studio</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Right click the project &ldquo;</span><span style="font-size:10pt">CSNETCoreOSVersion</span><span style="font-size:10pt">&rdquo;
 and select Restore Packages</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Press
</span><span style="font-weight:bold; font-size:10pt">F6</span><span style="font-size:10pt"> Key or select
</span><span style="font-weight:bold; font-size:10pt">Build -&gt; Build Solution</span><span style="font-size:10pt"> from the menu to build the sample.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Open the sample solution using Visual Studio, then press F5 Key or select Debug -&gt; Start
 Debugging from the menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="171484-image.png" alt="" width="575" height="331" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Program.cs :</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;Get&nbsp;OS&nbsp;platform</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;returns&gt;&lt;/returns&gt;</span>&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;OSPlatform&nbsp;GetOSPlatform()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;OSPlatform&nbsp;osPlatform&nbsp;=&nbsp;OSPlatform.Create(<span class="cs__string">&quot;Other&nbsp;Platform&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Check&nbsp;if&nbsp;it's&nbsp;windows</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">bool</span>&nbsp;isWindows&nbsp;=&nbsp;RuntimeInformation.IsOSPlatform(OSPlatform.Windows);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;osPlatform&nbsp;=&nbsp;isWindows&nbsp;?&nbsp;OSPlatform.Windows&nbsp;:&nbsp;osPlatform;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Check&nbsp;if&nbsp;it's&nbsp;osx</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">bool</span>&nbsp;isOSX&nbsp;=&nbsp;RuntimeInformation.IsOSPlatform(OSPlatform.OSX);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;osPlatform&nbsp;=&nbsp;isOSX&nbsp;?&nbsp;OSPlatform.OSX&nbsp;:&nbsp;osPlatform;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Check&nbsp;if&nbsp;it's&nbsp;Linux</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">bool</span>&nbsp;isLinux&nbsp;=&nbsp;RuntimeInformation.IsOSPlatform(OSPlatform.Linux);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;osPlatform&nbsp;=&nbsp;isLinux&nbsp;?&nbsp;OSPlatform.Linux&nbsp;:&nbsp;osPlatform;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;osPlatform;&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">More information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://docs.microsoft.com/de-de/dotnet/core/api/system.runtime.interopservices.runtimeinformation"><span style="color:#0563c1; text-decoration:underline">RuntimeInformation class</span></a><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
