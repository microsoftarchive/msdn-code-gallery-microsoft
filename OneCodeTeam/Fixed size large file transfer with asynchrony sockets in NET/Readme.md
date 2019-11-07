# Fixed size large file transfer with asynchrony sockets in NET
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Networking
- Windows Desktop App Development
## Topics
- Socket
## Updated
- 06/13/2013
## Description

<p class="MsoCommentText" style=""><b style=""><span style="font-size:14.0pt; font-family:&quot;Cambria&quot;,&quot;serif&quot;">How to transfer large size file by using asynchronous Sockets API in .NET</span></b><b><span style="font-size:14.0pt; font-family:&quot;Cambria&quot;,&quot;serif&quot;">
 (CSSocketTransferFile)</span></b></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">Introduction
</span></b></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><span style="font-size:10.0pt; line-height:115%">​</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">The sample demonstrates how to transfer large size</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
 file</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">with asynchronous sockets API in .NET</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">. In a typical .NET application, we
 can use POST methods to transfer files in </span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">a
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">HTTPRequ</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">e</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">st
 message, but with request body size limit.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Instead, .NET Socket API provides us an elegant and asynchronous way to transfer large files between the client and server.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">In this sample, you will see large data are pushed from server to the clients by using BeginSend, BeginReceive methods of the .NET Socket class.</span><span style="color:#1F497D">
</span></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">Running the Sample
</span></b></p>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.75in; text-indent:-.25in; line-height:12.75pt">
<span style=""><span style="">1．<span style="font:7.0pt &quot;Times New Roman&quot;"> </span>
</span></span>Open this sample in Microsoft Visual Studio and build the solution.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.25in; line-height:12.75pt">
<span style="color:black"><span style="">2．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="color:black">After you successfully build the sample project in Microsoft Visual Studio, you will get Server.exe under the .\CSSocketTransferFile\Server\bin\Debug folder and Client.exe under the .\CSSocketTransferFile\Client\bin\Debug
 folder. Server.exe </span>is responsible for <span style="color:black">sending files to the client, while Clien</span><span style="color:black">t</span><span style="color:black">.exe will receive the file transferred from the server.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.25in; line-height:12.75pt">
<span style=""><span style="">3．<span style="font:7.0pt &quot;Times New Roman&quot;"> </span>
</span></span>Launch Server.exe, <span style="">specify </span>a valid TCP port in the Port TextBox.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.25in; line-height:12.75pt">
<span style=""><span style="">4．<span style="font:7.0pt &quot;Times New Roman&quot;"> </span>
</span></span>Click &quot;Browse…&quot; button to select a file to be used to transfer.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.25in; line-height:12.75pt">
<span style=""><span style="">5．<span style="font:7.0pt &quot;Times New Roman&quot;"> </span>
</span></span>Click &quot;StartUp&quot; button to let the server listen to the <span style="">
specified Port.</span></p>
<p class="MsoListParagraphCxSpMiddle" style="line-height:12.75pt"><span style=""><img src="84301-image.png" alt="" width="327" height="290" align="middle">
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.75in; text-indent:-.25in; line-height:12.75pt">
<span style=""><span style="">6．<span style="font:7.0pt &quot;Times New Roman&quot;"> </span>
</span></span><span style="">When you click &quot;StartUp&quot; button to start the server, the system will show a Windows Security Alert dialog, please select a network and click the &quot;Allow access&quot; button.</span></p>
<p class="MsoListParagraphCxSpLast" style="line-height:12.75pt"><span style=""><img src="84302-image.png" alt="" width="539" height="425" align="middle">
</span></p>
<p class="MsoNormalCxSpFirst" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.75in; text-indent:-.25in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">7．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Launch multiple clients and specify the IP Address (</span><span lang="EN" style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">this
 example uses the Ipv4 pro</span><span lang="EN" style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">tocol</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">) and port of the server
 and then click the &quot;Save To…&quot; button to select a directory used to store the file received from the server.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84303-image.png" alt="" width="601" height="268" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.75in; text-indent:-.25in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">8．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Click the Connect button to connect to the specified server.
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84304-image.png" alt="" width="336" height="268" align="middle">
<img src="84305-image.png" alt="" width="336" height="269" align="middle">
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84306-image.png" alt="" width="325" height="289" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.75in; text-indent:-.25in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">9．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Select the clients you want to send the file on the lbxServer and then click the Send button on the Server form to send the file from the server to clients.
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">&nbsp;</span><span style="">
<img src="84307-image.png" alt="" width="601" height="269" align="middle">
</span></span></p>
<p class="MsoNormalCxSpLast" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84308-image.png" alt="" width="325" height="289" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoListParagraph" style="margin-left:.75in; text-indent:-.25in"><span style=""><span style="">10.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">&nbsp;&nbsp;</span>Send Completely.</p>
<p class="MsoNormalCxSpFirst" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84309-image.png" alt="" width="301" height="394" align="middle">
<img src="84310-image.png" alt="" width="299" height="397" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84311-image.png" alt="" width="326" height="289" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">Using the Code
</span></b></p>
<p class="MsoCommentText" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">1.
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Asynchronous</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">ly</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"> communicate with
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Server </span>
<span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">by </span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">using BeginConnect method of the Socket object.</span></p>
<p class="MsoNormal" style=""></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
 /// Start connect to the server.
 /// &lt;/summary&gt;
 public static void StartClient()
 {
&nbsp;&nbsp;&nbsp;&nbsp; connected = false;
&nbsp;&nbsp;&nbsp;&nbsp; if (IpAddress == null)
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MessageBox.Show(Properties.Resources.InvalidAddressMsg);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return;
&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp; IPEndPoint remoteEP = new IPEndPoint(IpAddress, Port);


&nbsp;&nbsp;&nbsp;&nbsp; // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
&nbsp;&nbsp;&nbsp;&nbsp; Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


&nbsp;&nbsp;&nbsp;&nbsp; // Begin to connect the server.
&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
&nbsp;&nbsp;&nbsp;&nbsp; connectDone.WaitOne();


&nbsp;&nbsp;&nbsp;&nbsp; if (connected)
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Begin to receive the file after connecting to server successfully.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Receive(clientSocket);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; receiveDone.WaitOne();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Notify the user whether receive the file completely.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Client.BeginInvoke(new FileReceiveDoneHandler(Client.FileReceiveDone));


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Close the socket.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.Shutdown(SocketShutdown.Both);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.Close();
&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Thread.CurrentThread.Abort();
&nbsp;&nbsp;&nbsp;&nbsp; }
 }

</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
 /// Start connect to the server.
 /// &lt;/summary&gt;
 public static void StartClient()
 {
&nbsp;&nbsp;&nbsp;&nbsp; connected = false;
&nbsp;&nbsp;&nbsp;&nbsp; if (IpAddress == null)
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MessageBox.Show(Properties.Resources.InvalidAddressMsg);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return;
&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp; IPEndPoint remoteEP = new IPEndPoint(IpAddress, Port);


&nbsp;&nbsp;&nbsp;&nbsp; // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
&nbsp;&nbsp;&nbsp;&nbsp; Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


&nbsp;&nbsp;&nbsp;&nbsp; // Begin to connect the server.
&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
&nbsp;&nbsp;&nbsp;&nbsp; connectDone.WaitOne();


&nbsp;&nbsp;&nbsp;&nbsp; if (connected)
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Begin to receive the file after connecting to server successfully.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Receive(clientSocket);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; receiveDone.WaitOne();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Notify the user whether receive the file completely.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Client.BeginInvoke(new FileReceiveDoneHandler(Client.FileReceiveDone));


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Close the socket.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.Shutdown(SocketShutdown.Both);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.Close();
&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Thread.CurrentThread.Abort();
&nbsp;&nbsp;&nbsp;&nbsp; }
 }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoCommentText" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">2.
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Asynchronous</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">ly</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"> communicate with
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Server </span>
<span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">by </span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">using BeginReceive method of the Socket object.
</span><span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
 /// Receive the file send by the server.
 /// &lt;/summary&gt;
 /// &lt;param name=&quot;clientSocket&quot;&gt;&lt;/param&gt;
 private static void Receive(Socket clientSocket)
 {
&nbsp;&nbsp;&nbsp;&nbsp; StateObject state = new StateObject();
&nbsp;&nbsp;&nbsp;&nbsp; state.WorkSocket = clientSocket;


&nbsp;&nbsp;&nbsp;&nbsp; ReceiveFileInfo(clientSocket);


&nbsp;&nbsp;&nbsp;&nbsp; int progressLen = checked((int)(fileLen / StateObject.BufferSize &#43; 1));
&nbsp;&nbsp;&nbsp;&nbsp; object[] length = new object[1];
&nbsp;&nbsp;&nbsp;&nbsp; length[0] = progressLen;
&nbsp;&nbsp;&nbsp;&nbsp; Client.BeginInvoke(new SetProgressLengthHandler(Client.SetProgressLength), length);


&nbsp;&nbsp;&nbsp;&nbsp; // Begin to receive the file from the server.
&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; catch
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!clientSocket.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; HandleDisconnectException();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; }
 }

</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
 /// Receive the file send by the server.
 /// &lt;/summary&gt;
 /// &lt;param name=&quot;clientSocket&quot;&gt;&lt;/param&gt;
 private static void Receive(Socket clientSocket)
 {
&nbsp;&nbsp;&nbsp;&nbsp; StateObject state = new StateObject();
&nbsp;&nbsp;&nbsp;&nbsp; state.WorkSocket = clientSocket;


&nbsp;&nbsp;&nbsp;&nbsp; ReceiveFileInfo(clientSocket);


&nbsp;&nbsp;&nbsp;&nbsp; int progressLen = checked((int)(fileLen / StateObject.BufferSize &#43; 1));
&nbsp;&nbsp;&nbsp;&nbsp; object[] length = new object[1];
&nbsp;&nbsp;&nbsp;&nbsp; length[0] = progressLen;
&nbsp;&nbsp;&nbsp;&nbsp; Client.BeginInvoke(new SetProgressLengthHandler(Client.SetProgressLength), length);


&nbsp;&nbsp;&nbsp;&nbsp; // Begin to receive the file from the server.
&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; catch
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!clientSocket.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; HandleDisconnectException();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; }
 }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">3. For the large size file, we can't get the entire file at once. For this case we can
</span><span lang="EN" style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">iterativ</span><span lang="EN" style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">ely
</span><span lang="EN" style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">call</span><span lang="EN" style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">the BeginReceive method in the Callback function of the BeginReceive.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
private static void ReceiveCallback(IAsyncResult ar)
 {
&nbsp;&nbsp;&nbsp;&nbsp; StateObject state = (StateObject)ar.AsyncState;
&nbsp;&nbsp;&nbsp;&nbsp; Socket clientSocket = state.WorkSocket;
&nbsp;&nbsp;&nbsp;&nbsp; BinaryWriter writer;


&nbsp;&nbsp;&nbsp;&nbsp; int bytesRead = clientSocket.EndReceive(ar);
&nbsp;&nbsp;&nbsp; &nbsp;if (bytesRead &gt; 0)
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; //If the file doesn't exist, create a file with the filename got from server. If the file exists, append to the file.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!File.Exists(fileSavePath))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Create));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Append));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer.Write(state.Buffer, 0, bytesRead);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer.Flush();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer.Close();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Notify the progressBar to change the position.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Client.BeginInvoke(new ProgressChangeHandler(Client.ProgressChanged));


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Recursively receive the rest file.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!clientSocket.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MessageBox.Show(Properties.Resources.DisconnectMsg);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Signal if all the file received.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; receiveDone.Set();
&nbsp;&nbsp;&nbsp;&nbsp; }
 }

</pre>
<pre id="codePreview" class="csharp">
private static void ReceiveCallback(IAsyncResult ar)
 {
&nbsp;&nbsp;&nbsp;&nbsp; StateObject state = (StateObject)ar.AsyncState;
&nbsp;&nbsp;&nbsp;&nbsp; Socket clientSocket = state.WorkSocket;
&nbsp;&nbsp;&nbsp;&nbsp; BinaryWriter writer;


&nbsp;&nbsp;&nbsp;&nbsp; int bytesRead = clientSocket.EndReceive(ar);
&nbsp;&nbsp;&nbsp; &nbsp;if (bytesRead &gt; 0)
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; //If the file doesn't exist, create a file with the filename got from server. If the file exists, append to the file.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!File.Exists(fileSavePath))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Create));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Append));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer.Write(state.Buffer, 0, bytesRead);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer.Flush();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; writer.Close();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Notify the progressBar to change the position.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Client.BeginInvoke(new ProgressChangeHandler(Client.ProgressChanged));


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Recursively receive the rest file.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!clientSocket.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MessageBox.Show(Properties.Resources.DisconnectMsg);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Signal if all the file received.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; receiveDone.Set();
&nbsp;&nbsp;&nbsp;&nbsp; }
 }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="text-autospace:none"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">4.
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Asynchronous</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">ly</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"> communicate with
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Client </span>
<span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">by </span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">using BeginAccept method of the Socket object.</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
</span></p>
<p class="MsoNormal" style="text-autospace:none"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
/// Server start to listen the client connection.
/// &lt;/summary&gt;
public static void StartListening()
{
&nbsp;&nbsp;&nbsp; IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);


&nbsp;&nbsp;&nbsp; // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
&nbsp;&nbsp;&nbsp; Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; listener.Bind(localEndPoint);
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; catch (SocketException ex)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MessageBox.Show(ex.Message);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; listener.Listen(c_clientSockets);


&nbsp;&nbsp;&nbsp; //loop listening the client.
&nbsp;&nbsp;&nbsp; while (true)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; allDone.Reset();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; allDone.WaitOne();
&nbsp;&nbsp;&nbsp; }
}

</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
/// Server start to listen the client connection.
/// &lt;/summary&gt;
public static void StartListening()
{
&nbsp;&nbsp;&nbsp; IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);


&nbsp;&nbsp;&nbsp; // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
&nbsp;&nbsp;&nbsp; Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; listener.Bind(localEndPoint);
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; catch (SocketException ex)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MessageBox.Show(ex.Message);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; listener.Listen(c_clientSockets);


&nbsp;&nbsp;&nbsp; //loop listening the client.
&nbsp;&nbsp;&nbsp; while (true)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; allDone.Reset();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; allDone.WaitOne();
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="text-autospace:none"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="text-autospace:none"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="text-autospace:none"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">&nbsp;</span>5.
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Asynchronous</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">ly</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"> communicate with
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Client </span>
<span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">by </span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">using
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">BeginSend</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"> method of the Socket object. For the large sized file, we can't read it to the memory at once, so we
 can </span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Blocking read file and send to the clients.
</span></p>
<p class="MsoNormal" style=""><span style="font-size:9.5pt; font-family:Consolas"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
// Blocking read file and send to the clients asynchronously.
using (FileStream stream = new FileStream(FileToSend, FileMode.Open))
{
&nbsp;&nbsp;&nbsp; do
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.Reset();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; signal = 0;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; stream.Flush();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; readBytes = stream.Read(buffer,0,c_bufferSize);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (ClientsToSend.Count == 0)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.Set();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (KeyValuePair&lt;Socket,IPEndPoint&gt; kvp in ClientsToSend)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Socket handler = kvp.Key;
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IPEndPoint ipEndPoint = kvp.Value;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; handler.BeginSend(buffer, 0, readBytes, SocketFlags.None, new AsyncCallback(SendCallback), handler);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!handler.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; closedSockets.Add(handler);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; signal&#43;&#43;;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; removedItems.Add(ipEndPoint.ToString());


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Signal if all the clients are disconnected.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (signal &gt;= ClientsToSend.Count)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.Set();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.WaitOne();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Remove the clients which are disconnected.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RemoveClient(closedSockets);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RemoveClientItem(removedItems);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; closedSockets.Clear();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; removedItems.Clear();
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; while (readBytes &gt; 0);
}

</pre>
<pre id="codePreview" class="csharp">
// Blocking read file and send to the clients asynchronously.
using (FileStream stream = new FileStream(FileToSend, FileMode.Open))
{
&nbsp;&nbsp;&nbsp; do
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.Reset();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; signal = 0;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; stream.Flush();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; readBytes = stream.Read(buffer,0,c_bufferSize);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (ClientsToSend.Count == 0)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.Set();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (KeyValuePair&lt;Socket,IPEndPoint&gt; kvp in ClientsToSend)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Socket handler = kvp.Key;
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IPEndPoint ipEndPoint = kvp.Value;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; try
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; handler.BeginSend(buffer, 0, readBytes, SocketFlags.None, new AsyncCallback(SendCallback), handler);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; catch
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (!handler.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; closedSockets.Add(handler);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; signal&#43;&#43;;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; removedItems.Add(ipEndPoint.ToString());


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Signal if all the clients are disconnected.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (signal &gt;= ClientsToSend.Count)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.Set();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sendDone.WaitOne();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Remove the clients which are disconnected.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RemoveClient(closedSockets);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RemoveClientItem(removedItems);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; closedSockets.Clear();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; removedItems.Clear();
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; while (readBytes &gt; 0);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">More Information
</span></b></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.aspx">Socket</a></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://www.albahari.com/threading">Threading in C#</a></p>
<p class="MsoNormal" style=""><span style=""></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
