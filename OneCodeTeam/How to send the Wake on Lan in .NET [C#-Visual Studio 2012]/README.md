# How to send the Wake on Lan in .NET [C#-Visual Studio 2012]
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
- 08/29/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://bit.ly/onecodesampletopbanner" alt="">
</a></div>
<p class="MsoNormal"><strong><span style="font-size:14.0pt; line-height:107%">How to send&nbsp;the Wake on Lan in .NET [C#-Visual Studio 2012]
</span></strong></p>
<p class="MsoNormal"><strong><span style="font-size:14.0pt; line-height:107%">Introduction:
</span></strong></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
This sample demonstrates how to send the Wake up on LAN. For this program to function, your machine must be set up to accept wake-up on LAN requests.<span style="font-size:9.5pt; font-family:Consolas; color:green; background:white">
</span>The motherboard must support Wake On LAN.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal"><strong><span style="font-size:14.0pt; line-height:107%">Using the Code:
</span></strong></p>
<p class="MsoNormal">Code Details:</p>
<p class="MsoNormal"><span class="SpellE"><strong>WakeUpOnLan</strong></span> class<span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:#2b91af">
</span>asks the user to enter the MAC address of the machine which you want to wake up. This class calls the
<span class="SpellE"><strong>Wakup</strong></span> method <span>that </span>takes MAC address as input parameter. See the signature of the Wakeup method below</p>
<p class="MsoNormal"><span class="GramE"><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:black; background:white">Wakeup(</span></span><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:blue; background:white">string</span><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:black; background:white">
<span class="SpellE">macAddress</span>)</span></p>
<p class="MsoNormal"><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:black; background:white"><br>
</span></p>
<p class="MsoNormal"><span class="SpellE"><strong>WOLUdpClient</strong></span>
<span class="SpellE">calss</span> which is derived from <span class="SpellE">
<strong>UdpClient</strong></span>. This class implemented the functionality of broadcasting the packet. About how this class got implemented, please see the code for more clarity.</p>
<p class="MsoNormal"><strong><span style="font-size:14.0pt; line-height:107%">How to get Mac address:
</span></strong></p>
<p class="MsoNormal">You can get MAC address of the machine by using :&gt;ipconfig /all. And you can see the physical address as below:</p>
<p class="MsoNormal"><img id="158924" src="https://i1.code.msdn.s-msft.com/sending-the-wake-on-lan-in-696884ac/image/file/158924/1/t1.png" alt="" width="432" height="20"></p>
<p class="MsoNormal">While entering the MAC address of the machine, remove all &quot;-&quot; as below:</p>
<p class="MsoNormal">4437E694B11E</p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">Please see the below screen shot:</p>
<p class="MsoNormal"><img id="158925" src="https://i1.code.msdn.s-msft.com/sending-the-wake-on-lan-in-696884ac/image/file/158925/1/t2.png" alt="" width="624" height="421"></p>
<p class="MsoNormal"><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:black">&nbsp;</span></p>
<p class="MsoNormal">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
