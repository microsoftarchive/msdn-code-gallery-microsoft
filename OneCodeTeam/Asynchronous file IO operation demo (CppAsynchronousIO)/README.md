# Asynchronous file I/O operation demo (CppAsynchronousIO)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- File System
## Topics
- IO
- CreateFile
- WriteFile
- ReadFile
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>CONSOLE APPLICATION : CppAsynchronousIO Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
CppAsynchronousIO demonstrates the asynchronous file I/O operations. A <br>
thread performing asynchronous file I/O sends an I/O request to the kernel by <br>
calling an appropriate function. If the request is accepted by the kernel, <br>
the calling thread continues processing another job until the kernel signals <br>
to the thread that the I/O operation is complete. It then interrupts its <br>
current job and processes the data from the I/O operation as necessary.<br>
<br>
</p>
<h3>Project Relation:</h3>
<p style="font-family:Courier New"><br>
CppAsynchronousIO - CppSynchronousIO<br>
CppAsynchronousIO shows asynchronous I/O operations and CppSynchronousIO <br>
demonstrates synchronous I/O.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Create/open a file for reading and writing asynchronously by calling <br>
CreateFile, specifying the FILE_FLAG_OVERLAPPED flag in the <br>
dwFlagsAndAttributes parameter. <br>
<br>
2. To queue an I/O request for a device driver, call ReadFile and WriteFile <br>
with an OVERLAPPED structure.<br>
<br>
3. To receive completed I/O request notifications, Windows offers four <br>
different methods:<br>
<br>
a) Signaling a device kernel object: Not useful for performing multiple <br>
simultaneous I/O requests against a single device. Allows one thread to <br>
issue an I/O request and another thread to process it. (See <br>
WaitForDeviceObject)<br>
<br>
b) Signaling an event kernel object: Allows multiple simultaneous I/O <br>
requests against a single device. Allows one thread to issue an I/O request <br>
and another thread to process it. (See: WaitForEventObject and <br>
GetOverlappedResult)<br>
<br>
c) Using alertable I/O: Allows multiple simultaneous I/O requests against a <br>
single device. The thread that issued an I/O request must also process it. <br>
(See: AlertableIO)<br>
<br>
d) Using I/O completion ports: Allows multiple simultaneous I/O requests <br>
against a single device. Allows one thread to issue an I/O request and <br>
another thread to process it. This technique is highly scalable and has the <br>
most flexibility. (See IoCompletionPort)<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: Synchronous and Asynchronous I/O<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa365683.aspx">http://msdn.microsoft.com/en-us/library/aa365683.aspx</a><br>
<br>
Windows via C/C&#43;&#43;, Fifth Edition. Basics of Asynchronous Device I/O<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/cc500408.aspx">http://msdn.microsoft.com/en-us/library/cc500408.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
