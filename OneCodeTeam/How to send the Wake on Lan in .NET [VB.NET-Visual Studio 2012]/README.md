# How to send the Wake on Lan in .NET [VB.NET-Visual Studio 2012]
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Networking
- Windows Desktop App Development
## Topics
- Wake on LAN
## Updated
- 09/05/2016
## Description

<hr>
<h1><span style="font-size:13.5pt; font-family:&quot;Segoe UI Light&quot;,sans-serif; color:black; background:#FCFCFC">How to send the Wake on Lan in .NET [VB.NET &ndash; Visual Studio 2012]</span></h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample demonstrates how to wake up on LAN. For this program to function, your machine must be set up to accept wake-up on LAN requests. The motherboard must support Wake On LAN.</p>
<h2>Using the code</h2>
<p class="MsoNormal">Code details:</p>
<p class="MsoNormal"><strong>Main</strong> method calls ExecuteWOL with MAC address of the machine which you want to wake up. This class calls the
<strong>ExecuteWOL</strong> method it takes MAC address as input parameter. Please the signature of the Wakeup method below</p>
<p class="MsoNormal"><strong>ExecuteWOL</strong> <span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
(</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">ByVal</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"> MAC As
</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">String</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">)</span></p>
<p class="MsoNormal"><strong>ExecuteWOL</strong>&nbsp; method&nbsp; calls <strong>
generatePacket()</strong>&nbsp; method .Create an object of <strong>UdpClient</strong> calss and call the send method. This class implemented the functionality of broadcasting the packet. For more clarity, see the source code of
<strong>ExecuteWOL</strong> function implementation.</p>
<p class="MsoNormal"><strong><span style="font-size:12.0pt; font-family:&quot;Calibri Light&quot;,sans-serif">How to get Mac address:</span></strong></p>
<p class="MsoNormal">You can get MAC address of the machine by using :&gt;ipconfig /all. And see the physical address as below:</p>
<p class="MsoNormal"><img src="159292-image.png" alt="" width="432" height="20" align="middle"></p>
<p class="MsoNormal">While entering the MAC address of the machine, remove all &quot;-&quot; as below:</p>
<p class="MsoNormal">4437E694B11E</p>
<p class="MsoNormal">Please see the below screen shot:</p>
<p class="MsoNormal"><img src="159293-image.png" alt="" width="624" height="421" align="middle"> &nbsp;</p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
