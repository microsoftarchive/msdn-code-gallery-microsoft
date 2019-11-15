# Socket communication in VB (VBSocketCommunication)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- IPC and RPC
- Network
## Topics
- Socket
## Updated
- 02/19/2013
## Description

<h1>CONSOLE APPLICATION (VBSocketCommunication)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">Sockets are an application programming interface (API) in an operating system, used for in inter-process communication. Sockets constitute a mechanism for delivering incoming data packets to the appropriate application process or thread,
 based on a combination of local and remote IP addresses and port numbers. Each socket is mapped by the operational system to a communicating application process or thread.</p>
<p class="MsoNormal">.NET supplies a Socket class which implements the Berkeley sockets interface. It provides a rich set of methods and properties for network communications. The Socket class allows you to perform both synchronous and asynchronous data transfer
 using any of the communication protocols listed in the ProtocolType enumeration. It supplies the following types of socket:</p>
<p class="MsoListParagraphCxSpFirst"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Stream: Supports reliable, two-way, connection-based byte streams without the duplication of data and without preservation of boundaries.</p>
<p class="MsoListParagraphCxSpMiddle"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>&nbsp;</span>Dgram<span class="GramE">:Supports</span> datagrams, which are connectionless, unreliable messages of a fixed (typically small) maximum length.</p>
<p class="MsoListParagraphCxSpMiddle"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Raw: Supports access to the underlying transport protocol.Using the SocketTypeRaw, you can communicate using protocols like Internet Control Message Protocol (Icmp) and Internet Group Management Protocol (<span class="SpellE">Igmp</span>).</p>
<p class="MsoListParagraphCxSpMiddle"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>&nbsp;</span>Rdm: Supports connectionless, message-oriented, reliably delivered messages, and preserves message boundaries in data.</p>
<p class="MsoListParagraphCxSpMiddle"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span class="SpellE">Seqpacket</span>: Provides connection-oriented and reliable two-way transfer of ordered byte streams across a network.</p>
<p class="MsoListParagraphCxSpMiddle"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Unknown:Specifies an unknown Socket type.</p>
<p class="MsoListParagraphCxSpLast"><span style="font-family:������">&nbsp;</span></p>
<h2>There are some limitations on this sample:</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Due to the socket buffer size, the string message including EOM marker shouldn't bigger than 1024 bytes when encoded to bytes by Unicode.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. The sample is designed for receiving and sending only one string message.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
To overcome the limitations above, the developer need handle message separating and merging operations on socket buffer.</p>
<p class="MsoListParagraph">&nbsp;</p>
<h2>Code Logic:</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Create a socket to listen the incoming TCP connection.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. After get the client connection<span class="GramE">,asynchronously</span> receive the data and listen the TCP connection again.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
3. Finishing receiving data, send the response to client process.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
4. If user inputs the word quit to exit the program</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<h2>Running the Sample</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
VB<span class="SpellE">SocketServer</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span><img src="53009-image.png" alt="" width="576" height="304" align="middle">
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<h1><span class="SpellE"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">VBSocketClient</span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span><span style="font-weight:normal">&nbsp;</span></h1>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<strong><span style="font-size:14.0pt; font-family:&quot;Cambria&quot;,&quot;serif&quot;">&nbsp;</span></strong></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span><img src="53010-image.png" alt="" width="565" height="203" align="middle">
</span><strong><span style="font-size:14.0pt; font-family:&quot;Cambria&quot;,&quot;serif&quot;">&nbsp;</span></strong></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<strong></strong></p>
<h2>More Information</h2>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoListParagraphCxSpFirst" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.aspx">Socket Class</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://msdn.microsoft.com/en-us/library/fx6588te.aspx">Asynchronous Server Socket Example</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://www.amazon.com/Professional-Network-Programming-Srinivasa-Sivakumar/dp/1861007353">Chapter4: Using Sockets of Professional .NET Network Prgromming</a>
</span></p>
<p class="MsoListParagraphCxSpLast" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
