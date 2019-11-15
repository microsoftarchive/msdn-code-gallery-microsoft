# C# Windows app uses WM_COPYDATA for IPC (CSReceiveWM_COPYDATA)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- Windows General
## Topics
- WM_COPYDATA
- Inter-process Communication
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WINDOWS APPLICATION : CSReceiveWM_COPYDATA Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
Inter-process Communication (IPC) based on the Windows message WM_COPYDATA is <br>
a mechanism for exchanging data among Windows applications in the local <br>
machine. The receiving application must be a Windows application. The data <br>
being passed must not contain pointers or other references to objects not <br>
accessible to the application receiving the data. While WM_COPYDATA is being <br>
sent, the referenced data must not be changed by another thread of the <br>
sending process. The receiving application should consider the data read-only. <br>
If the receiving application must access the data after SendMessage returns, <br>
it needs to copy the data into a local buffer.<br>
<br>
This code sample demonstrates receiving a custom data structure (MyStruct) <br>
from the sending application (CSSendWM_COPYDATA) by handling WM_COPYDATA <br>
messages.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the WM_COPYDATA samples.<br>
<br>
Step1. After you successfully build the CSSendWM_COPYDATA and <br>
CSReceiveWM_COPYDATA sample projects in Visual Studio 2008, you will get the <br>
applications: CSSendWM_COPYDATA.exe and CSReceiveWM_COPYDATA.exe. <br>
<br>
Step2. Run CSSendWM_COPYDATA.exe and CSReceiveWM_COPYDATA.exe. In <br>
CSSendWM_COPYDATA, input the Number and Message fields.<br>
<br>
&nbsp;Number: 123456<br>
&nbsp;Message: Hello World<br>
<br>
Then click the SendMessage button. The number and the message will be sent <br>
to CSReceiveWM_COPYDATA through a WM_COPYDATA message. When <br>
CSReceiveWM_COPYDATA receives the message, the application extracts the <br>
number and the message, and display them in the window.<br>
<br>
</p>
<h3>Sample Relation:</h3>
<p style="font-family:Courier New">(The relationship between the current sample and the rest samples in
<br>
Microsoft All-In-One Code Framework <a target="_blank" href="http://1code.codeplex.com)">
http://1code.codeplex.com)</a><br>
<br>
CSSendWM_COPYDATA -&gt; CSReceiveWM_COPYDATA<br>
CSSendWM_COPYDATA sends data to CSReceiveWM_COPYDATA through the WM_COPYDATA <br>
message.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Override the WndProc method in the Windows Form.<br>
<br>
&nbsp; &nbsp;protected override void WndProc(ref Message m)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp;}<br>
<br>
2. Handle the WM_COPYDATA message in WndProc <br>
<br>
&nbsp; &nbsp;if (m.Msg == WM_COPYDATA)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;<br>
3. Get the COPYDATASTRUCT struct from lParam of the WM_COPYDATA message, and <br>
fetch the data (MyStruct object) from COPYDATASTRUCT.lpData.<br>
<br>
&nbsp; &nbsp;// Get the COPYDATASTRUCT struct from lParam.<br>
&nbsp; &nbsp;COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));<br>
<br>
&nbsp; &nbsp;// If the size matches<br>
&nbsp; &nbsp;if (cds.cbData == Marshal.SizeOf(typeof(MyStruct)))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Marshal the data from the unmanaged memory block to a
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// MyStruct managed struct.<br>
&nbsp; &nbsp; &nbsp; &nbsp;MyStruct myStruct = (MyStruct)Marshal.PtrToStructure(cds.lpData,
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;typeof(MyStruct));<br>
&nbsp; &nbsp;}<br>
<br>
4. Display the data in the form.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
WM_COPYDATA Message<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms649011.aspx">http://msdn.microsoft.com/en-us/library/ms649011.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
