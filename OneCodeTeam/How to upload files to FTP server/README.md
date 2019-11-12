# How to upload files to FTP server
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Networking
- Windows Desktop App Development
## Topics
- Upload
- FTP
## Updated
- 11/10/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<h1>Ftp File uploader to upload a particular file in the ftp server in c&#43;&#43;</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The C&#43;&#43; sample code of console application developed in visual studio 2012 helps us to upload a particular file from the ftp server that supports only plain FTP not FTPS.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1: Open the &quot;Ftp.sln&quot; file using VS 2012 or above. <br>
<br>
<img src="163201-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step2: Build the code either by cliking &quot;ctrl&#43; shift&#43; B&quot; or by pressing F6 button.
<br>
<br>
<img src="163202-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step3: Press F5 to execute the code.<br>
<br>
<img src="163203-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step4: Once the console is opened, enter the required details.<br>
<br>
<img src="163204-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step5: Once you have entered all the details, the file will be uploaded&nbsp;in the ftp server.<br>
<br>
<img src="163205-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
<img src="163206-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
<img src="163207-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
<img src="163208-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Below is the code snippet which runs ftp commands for downloading the file and the code snippet that writes '*' in the place&nbsp;for password</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>

<div class="preview">
<pre class="cplusplus"><span class="cpp__datatype">int</span>&nbsp;main()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Password&nbsp;=&nbsp;getpass(<span class="cpp__string">&quot;Enter&nbsp;password&nbsp;:&quot;</span>,&nbsp;<span class="cpp__keyword">true</span>);&nbsp;<span class="cpp__com">//calling&nbsp;function&nbsp;which&nbsp;prints&nbsp;'*'&nbsp;in&nbsp;place&nbsp;of&nbsp;actual&nbsp;password</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;system(<span class="cpp__string">&quot;ftp&nbsp;-s:commands.bin&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
}&nbsp;
&nbsp;
Below&nbsp;is&nbsp;the&nbsp;code&nbsp;snippet&nbsp;that&nbsp;writes&nbsp;<span class="cpp__string">'*'</span>&nbsp;in&nbsp;place&nbsp;of&nbsp;password&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<span class="cpp__datatype">string</span>&nbsp;getpass(<span class="cpp__keyword">const</span>&nbsp;<span class="cpp__datatype">char</span>&nbsp;*prompt,&nbsp;<span class="cpp__datatype">bool</span>&nbsp;show_asterisk&nbsp;=&nbsp;<span class="cpp__keyword">true</span>)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">while</span>&nbsp;((ch&nbsp;=&nbsp;getch())&nbsp;!=&nbsp;RETURN)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">if</span>&nbsp;(ch&nbsp;==&nbsp;BACKSPACE)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">else</span>&nbsp;<span class="cpp__keyword">if</span>&nbsp;(ch&nbsp;==&nbsp;<span class="cpp__number">0</span>&nbsp;||&nbsp;ch&nbsp;==&nbsp;<span class="cpp__number">224</span>)&nbsp;<span class="cpp__com">//&nbsp;handle&nbsp;escape&nbsp;sequences</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;getch();&nbsp;<span class="cpp__com">//&nbsp;ignore&nbsp;non&nbsp;printable&nbsp;chars</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">continue</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;password&nbsp;&#43;=&nbsp;ch;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">if</span>&nbsp;(show_asterisk)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cout&nbsp;&lt;&lt;&nbsp;<span class="cpp__string">'*'</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__com">//....</span>&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
