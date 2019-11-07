# How to implement serial port communication in .NET
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- .NET
## Topics
- Serial Port
## Updated
- 11/27/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<h1>How to implement serial port communication in .NET</h1>
<p><strong>Overview:</strong> This code sample is written&nbsp;to demonstrate serial port communication. This program displays the data from serial port.</p>
<p><strong>Code details:</strong></p>
<p>C.Net framework has come up with implementation of serial port communication with System.IO.Ports name space. Under this name space we have SerialPort class supports functionalities to get available COM ports, opens&nbsp;and closes the ports. And lao this
 exposes to read bytes, character and data from the port.</p>
<p>Here&nbsp;is the code for your refrence:</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden"> string[] ports = SerialPort.GetPortNames();
 SerialPort myserialPort;
 myserialPort = new SerialPort(str, 9600);
 myserialPort.Open(); //open th eserial port
 byte b = (byte)myserialPort.ReadByte(); ///read a byte
 char c = (char)myserialPort.ReadChar(); // read a char
 string line = (string)myserialPort.ReadLine(); //read a whole line
 string all = (string)myserialPort.ReadExisting(); //read everythin in the buffer
 myserialPort.Close();
</pre>
<div class="preview">
<pre class="csharp">&nbsp;<span class="cs__keyword">string</span>[]&nbsp;ports&nbsp;=&nbsp;SerialPort.GetPortNames();&nbsp;
&nbsp;SerialPort&nbsp;myserialPort;&nbsp;
&nbsp;myserialPort&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SerialPort(str,&nbsp;<span class="cs__number">9600</span>);&nbsp;
&nbsp;myserialPort.Open();&nbsp;<span class="cs__com">//open&nbsp;th&nbsp;eserial&nbsp;port</span>&nbsp;
&nbsp;<span class="cs__keyword">byte</span>&nbsp;b&nbsp;=&nbsp;(<span class="cs__keyword">byte</span>)myserialPort.ReadByte();&nbsp;<span class="cs__com">///read&nbsp;a&nbsp;byte</span>&nbsp;
&nbsp;<span class="cs__keyword">char</span>&nbsp;c&nbsp;=&nbsp;(<span class="cs__keyword">char</span>)myserialPort.ReadChar();&nbsp;<span class="cs__com">//&nbsp;read&nbsp;a&nbsp;char</span>&nbsp;
&nbsp;<span class="cs__keyword">string</span>&nbsp;line&nbsp;=&nbsp;(<span class="cs__keyword">string</span>)myserialPort.ReadLine();&nbsp;<span class="cs__com">//read&nbsp;a&nbsp;whole&nbsp;line</span>&nbsp;
&nbsp;<span class="cs__keyword">string</span>&nbsp;all&nbsp;=&nbsp;(<span class="cs__keyword">string</span>)myserialPort.ReadExisting();&nbsp;<span class="cs__com">//read&nbsp;everythin&nbsp;in&nbsp;the&nbsp;buffer</span>&nbsp;
&nbsp;myserialPort.Close();&nbsp;</pre>
</div>
</div>
</div>
<p>Test:<br>
To test this program, build the program successfully without error.<br>
It will display the data of&nbsp;wether the port opens or not. <br>
<br>
</p>
<p>&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
