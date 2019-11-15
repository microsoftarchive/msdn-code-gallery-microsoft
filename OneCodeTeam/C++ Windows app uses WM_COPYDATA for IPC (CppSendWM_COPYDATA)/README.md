# C++ Windows app uses WM_COPYDATA for IPC (CppSendWM_COPYDATA)
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
<h2>WIN32 APPLICATION : CppSendWM_COPYDATA Project Overview</h2>
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
This code sample demonstrates sending a custom data structure (MY_STRUCT) to <br>
the receiving Windows application (CppReceiveWM_COPYDATA) by using <br>
SendMessage(WM_COPYDATA). If the data structure fails to be passed, the <br>
application displays the error code for diagnostics. A typical error code is <br>
0x5 (Access is denied) caused by User Interface Privilege Isolation (UIPI). <br>
UIPI prevents processes from sending selected window messages and other USER <br>
APIs to processes running with higher integrity. When the receiving <br>
application (CppReceiveWM_COPYDATA) runs at an integrity level higher than <br>
this sending application, you will see the &quot;SendMessage(WM_COPYDATA) failed <br>
w/err 0x00000005&quot; error message.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the WM_COPYDATA samples.<br>
<br>
Step1. After you successfully build the CppSendWM_COPYDATA and <br>
CppReceiveWM_COPYDATA sample projects in Visual Studio 2008, you will get the <br>
applications: CppSendWM_COPYDATA.exe and CppReceiveWM_COPYDATA.exe. <br>
<br>
Step2. Run CppSendWM_COPYDATA.exe and CppReceiveWM_COPYDATA.exe. In <br>
CppSendWM_COPYDATA, input the Number and Message fields.<br>
<br>
&nbsp;Number: 123456<br>
&nbsp;Message: Hello World<br>
<br>
Then click the SendMessage button. The number and the message will be sent <br>
to CppReceiveWM_COPYDATA through a WM_COPYDATA message. When <br>
CppReceiveWM_COPYDATA receives the message, the application extracts the <br>
number and the message, and display them in the window.<br>
<br>
</p>
<h3>Sample Relation:</h3>
<p style="font-family:Courier New">(The relationship between the current sample and the rest samples in
<br>
Microsoft All-In-One Code Framework <a target="_blank" href="http://1code.codeplex.com)">
http://1code.codeplex.com)</a><br>
<br>
CppSendWM_COPYDATA -&gt; CppReceiveWM_COPYDATA<br>
CppSendWM_COPYDATA sends data to CppReceiveWM_COPYDATA through the <br>
WM_COPYDATA message.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Find the handle of the receiving window (FindWindow)<br>
<br>
2. Prepare the COPYDATASTRUCT struct with the data to be sent. <br>
(COPYDATASTRUCT)<br>
<br>
3. Send the COPYDATASTRUCT struct through the WM_COPYDATA message to the <br>
receiving window.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: WM_COPYDATA Message<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms649011.aspx">http://msdn.microsoft.com/en-us/library/ms649011.aspx</a><br>
<br>
MSDN: Using Data Copy<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms649009.aspx">http://msdn.microsoft.com/en-us/library/ms649009.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
