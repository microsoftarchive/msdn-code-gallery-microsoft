# C# mailslot server for IPC (CSMailslotServer)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- IPC and RPC
- Windows General
## Topics
- mailslot
- Inter-process Communication
## Updated
- 02/19/2013
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>CONSOLE APPLICATION : CSMailslotServer Project Overview</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
Mailslot is a mechanism for one-way inter-process communication in the local<br>
machine or across the computers in the intranet. Any clients can store <br>
messages in a mailslot. The creator of the slot, i.e. the server, retrieves <br>
the messages that are stored there:<br>
<br>
Client (GENERIC_WRITE) ---&gt; Server (GENERIC_READ)<br>
<br>
This code sample demonstrates calling CreateMailslot to create a mailslot <br>
named &quot;\\.\mailslot\SampleMailslot&quot;. The security attributes of the slot are &nbsp;<br>
customized to allow Authenticated Users read and write access to the slot, <br>
and to allow the Administrators group full access to it. The sample first <br>
creates such a mailslot, then it reads and displays new messages in the slot <br>
when user presses ENTER in the console.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the mailslot sample.<br>
<br>
Step1. After you successfully build the CSMailslotClient and CSMailslotServer <br>
sample projects in Visual Studio 2008, you will get the applications: <br>
CSMailslotClient.exe and CSMailslotServer.exe. <br>
<br>
Step2. Run CSMailslotServer.exe in a command prompt to start up the server <br>
end of the mailslot. The application will output the following information <br>
in the command prompt if the mailslot is created successfully.<br>
<br>
&nbsp;Server:<br>
&nbsp; &nbsp;The mailslot (\\.\mailslot\SampleMailslot) is created.<br>
<br>
Step3. Run CSMailslotClient.exe in another command prompt to start up the <br>
client end of the mailslot. The application should output the message below <br>
in the command prompt when the client successfully opens the mailslot.<br>
<br>
&nbsp;Client:<br>
&nbsp; &nbsp;The mailslot (\\.\mailslot\SampleMailslot) is opened.<br>
<br>
Step4. The client attempts to write three messages to the mailslot. <br>
<br>
&nbsp;Client:<br>
&nbsp; &nbsp;The message &quot;Message 1 for mailslot&quot; is written to the slot<br>
&nbsp; &nbsp;The message &quot;Message 2 for mailslot&quot; is written to the slot<br>
&nbsp; &nbsp;The message &quot;Message 3 for mailslot&quot; is written to the slot<br>
<br>
There is a three seconds' interval between the second message and the third <br>
one. During the interval, if you press ENTER in the server application, the <br>
mailslot server will retrieve the first two messages and display them. <br>
<br>
&nbsp;Server:<br>
&nbsp; &nbsp;Checking new messages...<br>
&nbsp; &nbsp;Message #1: Message 1 for mailslot<br>
&nbsp; &nbsp;Message #2: Message 2 for mailslot<br>
<br>
After the interval, the client writes the thrid message. If you press ENTER <br>
again in the server application, the mailslot server prints the message:<br>
<br>
&nbsp;Server:<br>
&nbsp; &nbsp;Checking new messages...<br>
&nbsp; &nbsp;Message #1: Message 3 for mailslot<br>
<br>
Step5. Enter 'Q' in the server application. This will close the mailslot and <br>
quit the server.<br>
<br>
</p>
<h3>Sample Relation:</h3>
<p style="font-family:Courier New">(The relationship between the current sample and the rest samples in
<br>
Microsoft All-In-One Code Framework <a href="http://1code.codeplex.com)" target="_blank">
http://1code.codeplex.com)</a><br>
<br>
CSMailslotClient -&gt; CSMailslotServer<br>
CSMailslotServer creates the mailslot. CSMailslotClient opens the mailslot <br>
and writes messages to it.<br>
<br>
<br>
Code Logic (P/Invoke the native APIs):<br>
<br>
1. Create a mailslot. <br>
<br>
&nbsp;1.1 Prepare the security attributes (the lpSecurityAttributes parameter in <br>
&nbsp;CreateMailslot) for the mailslot. This is optional. If the <br>
&nbsp;lpSecurityAttributes parameter of CreateMailslot is NULL, the mailslot gets
<br>
&nbsp;a default security descriptor and the handle cannot be inherited. The ACLs <br>
&nbsp;in the default security descriptor of a mailslot grant full control to the <br>
&nbsp;LocalSystem account, (elevated) administrators, and the creator owner. They
<br>
&nbsp;also give only read access to members of the Everyone group and the <br>
&nbsp;anonymous account. However, if you want to customize the security <br>
&nbsp;permission of the mailslot, (e.g. to allow Authenticated Users to read from
<br>
&nbsp;and write to the mailslot), you need to create a SECURITY_ATTRIBUTES structure.<br>
&nbsp;<br>
&nbsp;The CreateMailslotSecurity helper function creates and initializes a new <br>
&nbsp;SECURITY_ATTRIBUTES structure to allow Authenticated Users read and write <br>
&nbsp;access to a mailslot, and to allow the Administrators group full access to <br>
&nbsp;the mailslot.<br>
&nbsp;<br>
&nbsp;1.2 Create the mailslot. (CreateMailslot)<br>
<br>
2. Check messages in the mailslot. (ReadMailslot)<br>
<br>
&nbsp;2.1. Check for the number of messages in the mailslot. (GetMailslotInfo)<br>
<br>
&nbsp;2.2. Retrieve the messages one by one from the mailslot. While reading, &nbsp;<br>
&nbsp;update the number of messages that are left in the mailslot. <br>
&nbsp;(ReadFile, GetMailslotInfo)<br>
<br>
3. Close the handle of the mailslot instance. (CloseHandle)<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
Using Mailslots / Reading from a Mailslot<br>
<a href="http://msdn.microsoft.com/en-us/library/aa365785.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/aa365785.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
