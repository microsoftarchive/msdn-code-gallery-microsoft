# DatagramSocket sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Networking
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows you how to a use datagram (UDP) socket to send and receive data using the
<a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a> and related classes in the
<a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a> namespace in your Windows Runtime app.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The client component of the sample creates a UDP socket, uses the socket to send and receive data, and closes the socket. The server component of the sample creates a UDP socket to listen for incoming network packets, receives incoming UDP packets from the
 client, sends data to the client, and closes the socket. This sample is provided in the JavaScript, C#, and C&#43;&#43; programming languages.
</p>
<p>The client component of the sample demonstrates the following features:</p>
<p></p>
<ul>
<li>Use the <a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a> class to create a UDP socket for the client to send and receive data.
</li><li>Add a handler for a <a href="http://msdn.microsoft.com/library/windows/apps/br241358">
<b>DatagramSocket.MessageReceived</b></a> event that indicates that a UDP datagram was received on the
<a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a> object.
</li><li>Set the remote endpoint for a UDP network server where packets should be sent using one of the
<a href="http://msdn.microsoft.com/library/windows/apps/hh701219"><b>DatagramSocket.ConnectAsync</b></a> methods.
</li><li>Send data to the server using the <a href="http://msdn.microsoft.com/library/windows/apps/br208154">
<b>Streams.DataWriter</b></a> object which allows a programmer to write common types (integers and strings, for example) on any stream.
</li><li>Close the socket. </li></ul>
<p></p>
<p>The server component of the sample demonstrates the following features:</p>
<p></p>
<ul>
<li>Use the <a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a> class to create a UDP socket to listen for and receive incoming datagram packets and for sending packets.
</li><li>Add a handler for a <a href="http://msdn.microsoft.com/library/windows/apps/br241358">
<b>DatagramSocket.MessageReceived</b></a> event that indicates that a UDP datagram was received on the
<a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a> object.
</li><li>Bind the socket to a local service name to listen for incoming UDP packets using the
<a href="http://msdn.microsoft.com/library/windows/apps/dn279143"><b>DatagramSocket.BindServiceNameAsync</b></a> method.
</li><li>Receive a <a href="http://msdn.microsoft.com/library/windows/apps/br241358"><b>DatagramSocket.MessageReceived</b></a> event that indicates that a UDP datagram was received on the
<a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a> object.
</li><li>Receive data from the client using the <a href="http://msdn.microsoft.com/library/windows/apps/br241358">
<b>DatagramSocket.MessageReceived</b></a> handler. The <a href="http://msdn.microsoft.com/library/windows/apps/br241344">
<b>DatagramSocketMessageReceivedEventArgs</b></a> object passed to the <b>DatagramSocket.MessageReceived</b> handler allows an app to receive data from the client and also determine the remote address and port that sent the data.
</li><li>Close the socket. </li></ul>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Use of this sample requires network access using the loopback interface.</p>
<p>For a sample that shows how to use a stream (TCP) socket to send and receive data in a Windows Runtime app, download the
<a href="http://go.microsoft.com/fwlink/p/?linkid=243037">StreamSocket sample</a>.</p>
<p><b>Network capabilities</b></p>
<p>This sample requires that network capabilities be set in the <i>Package.appxmanifest</i> file to allow the app to access the network at runtime. These capabilities can be set in the app manifest using Microsoft Visual Studio.
</p>
<p>To build the Windows version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Private Networks (Client &amp; Server)</b>: The sample has inbound and outbound network access on a home or work network (a local intranet). Even though this sample by default runs on loopback, it needs either the
<b>Private Networks (Client &amp; Server)</b> or <b>Internet (Client &amp; Server)</b> capability in order to send and receive UDP packets using a
<a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a>. The
<b>Private Networks (Client &amp; Server)</b> capability is represented by the <b>
Capability name = &quot;privateNetworkClientServer&quot;</b> tag in the app manifest. The <b>
Internet (Client &amp; Server)</b> capability is represented by the <b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest.
</p>
</li><li>
<p>If the sample is modified to connect to the server component running on a different device on the Internet (a more typical app), the
<b>Internet (Client)</b> capability must be set for the client component. This is represented by the
<b>Capability name = &quot;internetClient&quot;</b>. </p>
</li><li>
<p>If the sample is modified so the server component is running on a different device on the Internet (a more typical app), the
<b>Internet (Client &amp; Server)</b> capability must be set for the server component. This is represented by the
<b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest. </p>
</li></ul>
<p>To build the Windows Phone version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Internet (Client &amp; Server):</b> This sample has complete access to the network for both client operations (outbound-initiated access) and server operations (inbound-initiated access). This allows the app to send and receive UDP packets using a
<a href="http://msdn.microsoft.com/library/windows/apps/br241319"><b>DatagramSocket</b></a> with another device on the Internet or on a local intranet. This is represented by the
<b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;On Windows Phone, there is only one network capability which enables all network access for the app.</p>
<p></p>
</li></ul>
<p></p>
<p>For more information on network capabilities, see <a href="http://msdn.microsoft.com/library/windows/apps/hh770532">
How to set network capabilities</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Network communications using an IP loopback address cannot normally be used for interprocess communication between a Windows Runtime app and a different process (a different Windows Runtime app or a desktop app) because this is
 restricted by network isolation. Network communication using an IP loopback address is allowed within the same process for communication purposes in a Windows Runtime app. For more information, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh770532">How to set network capabilities</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Other - C#/VB/C&#43;&#43; and XAML</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452751">Adding support for networking (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452976">Connecting to network services (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452986">How to send and receive network data with a datagram socket (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj835817">How to set network capabilities (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj150598">How to use advanced socket controls (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770534">Troubleshooting and debugging network connections</a>
</dt><dt><b>Other - JavaScript and HTML</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452752">Adding support for networking (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452977">Connecting to network services (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452986">How to send and receive network data with a datagram socket (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770532">How to set network capabilities (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh780596">How to use advanced socket controls (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770534">Troubleshooting and debugging network connections</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226882"><b>DatagramSocket</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241344"><b>DatagramSocketMessageReceivedEventArgs</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207124"><b>Windows.Networking</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208119"><b>Windows.Storage.Streams.DataReader</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208154"><b>Windows.Storage.Streams.DataWriter</b></a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=243037">StreamSocket sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt></dl>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>DatagramSocket.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build DatagramSocket.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>DatagramSocket.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build DatagramSocket.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>DatagramSocket.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy DatagramSocket.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>DatagramSocket.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy DatagramSocket.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>DatagramSocket.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>DatagramSocket.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
