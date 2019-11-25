# How to download files from FTP server
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Networking
- Windows Desktop App Development
## Topics
- Download
- FTP
## Updated
- 10/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://aka.ms/onecodesampletopbanner1" alt="">
</a></div>
<h1>Ftp File Downloader to download a particular file in the ftp server in c&#43;&#43;</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The C&#43;&#43; sample code of console application developed in visual studio 2012 helps us to download a particular file from the ftp server.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1: Open the &quot;Ftp.sln&quot; file using VS 2012 or above. <br>
<br>
<img src="162316-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step2: Build the code eith by cliking &quot;ctrl&#43; shift&#43; B&quot; or by pressing F6 button. <br>
<br>
<img src="162317-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step3: Execute the code by pressing F5. <br>
<br>
<img src="162318-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step4: Once the console opens, enter the required details <br>
<br>
<img src="162319-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
Step5: Once you have entered&nbsp;all the details, the file will be downloaded&nbsp;in the same location where the project is present
<br>
<br>
<img src="162320-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
<img src="162321-image.png" alt="" width="500" height="300" align="middle">
<br>
<br>
</p>
<h2>Using the Code</h2>
<p class="MsoNormal">The following code snippet runs ftp commands for downloading the file.</p>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>

<div class="preview">
<pre class="cplusplus"><span class="cpp__datatype">int</span>&nbsp;main()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Password&nbsp;=&nbsp;getpass(<span class="cpp__string">&quot;Enter&nbsp;password&nbsp;:&quot;</span>,&nbsp;<span class="cpp__keyword">true</span>);&nbsp;<span class="cpp__com">//calling&nbsp;function&nbsp;which&nbsp;prints&nbsp;'*'&nbsp;in&nbsp;place&nbsp;of&nbsp;actual&nbsp;password</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;system(<span class="cpp__string">&quot;ftp&nbsp;-s:commands.bin&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
}</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p class="MsoNormal">Here is the code snippet that writes '*' in the place of password</p>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span>

<div class="preview">
<pre class="cplusplus"><span class="cpp__datatype">string</span>&nbsp;getpass(<span class="cpp__keyword">const</span>&nbsp;<span class="cpp__datatype">char</span>&nbsp;*prompt,&nbsp;<span class="cpp__datatype">bool</span>&nbsp;show_asterisk&nbsp;=&nbsp;<span class="cpp__keyword">true</span>)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">while</span>&nbsp;((ch&nbsp;=&nbsp;getch())&nbsp;!=&nbsp;RETURN)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">if</span>&nbsp;(ch&nbsp;==&nbsp;BACKSPACE)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
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
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;....&nbsp;
}</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
