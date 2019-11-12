# Background Transfer sample
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
- 05/13/2014
## Description

<div id="mainSection">
<p>This sample shows how to use the Background Transfer API to download and upload files in the background in Windows Runtime apps.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Classes in the <a href="http://msdn.microsoft.com/library/windows/apps/br207242">
<b>Windows.Networking.BackgroundTransfer</b></a> namespace provide a power-friendly, cost-aware, and flexible API for transferring files in the background. This sample shows how to download and upload files using the Background Transfer API.</p>
<p>For the download scenario, the sample first uses methods on <a href="http://msdn.microsoft.com/library/windows/apps/br207126">
<b>BackgroundDownloader</b></a> class to enumerate any downloads that were going on in the background while the app was closed. An app should enumerate these downloads when it gets started so it can attach a progress handler to these downloads to track progress
 and prevent stale downloads. Then other methods on the <b>BackgroundDownloader</b> and related classes are used to start new downloads to the local Pictures Library. The sample also shows how to pause downloads and change the priority of a download.
</p>
<p>For the upload scenario, the sample first uses methods on <a href="http://msdn.microsoft.com/library/windows/apps/br207140">
<b>BackgroundUploader</b></a> class to enumerate any uploads that were going on in the background while the app was closed. An app should enumerate these uploads when it gets started so it can attach a progress handler to these uploads to track progress and
 prevent stale uploads. Then other methods on the <b>BackgroundUploader</b> and related classes are used to start new uploads. The sample also shows how to set a content header and use a multipart upload.
</p>
<p>The sample also shows how to configure and use toast and tile notifications to inform the user when all transfers succeed or when at least one transfer fails.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Background transfer is primarily designed for long-term transfer operations for resources like video, music, and large images. For short-term operations involving transfers of smaller resources (i.e. a few KB), the HTTP APIs are
 recommended. <a href="http://msdn.microsoft.com/library/windows/apps/dn298639"><b>HttpClient</b></a> is preferred and can be used in all languages supported by Windows Runtime apps.
<a href="http://msdn.microsoft.com/library/windows/apps/br229787"><b>XHR</b></a> can be used in JavaScript.
<a href="http://msdn.microsoft.com/library/windows/apps/hh770550">IXHR2</a> can be used in C&#43;&#43;.</p>
<p></p>
<p>This sample requires the following capabilities:</p>
<ul>
<li><b>Internet (Client &amp; Server)</b> - Needed to send requests to download or upload files to HTTP or FTP servers on the Internet.
</li><li><b>Private Networks (Client &amp; Server)</b> - Needed to send requests to download or upload files to HTTP or FTP servers on a home or work intranet.
</li><li><b>Pictures Library</b> - Needed to downloads files to the Pictures library. </li></ul>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows version of this sample by default requires network access using the loopback interface.</p>
<p></p>
<p><b>Network capabilities</b></p>
<p>This sample requires that network capabilities be set in the <i>Package.appxmanifest</i> file to allow the app to access the network at runtime. These capabilities can be set in the app manifest using Microsoft Visual Studio.
</p>
<p>To build the Windows version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Internet (Client)</b>: The sample has outbound-initiated network access to the Internet. This allows the app to send requests to download or upload files to HTTP or FTP servers on the Internet. This is represented by the
<b>Capability name = &quot;internetClient&quot;</b> tag in the app manifest. </p>
<p><b>Private Networks (Client &amp; Server)</b>: The sample has inbound and outbound network access on a home or work network (a local intranet). This allows the app to send requests to download or upload files to HTTP or FTP servers on a local home or work
 Intranet. The <b>Private Networks (Client &amp; Server)</b> capability is represented by the
<b>Capability name = &quot;privateNetworkClientServer&quot;</b> tag in the app manifest. </p>
</li></ul>
<p>To build the Windows Phone version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Internet (Client &amp; Server):</b> This sample has complete access to the network for both client operations (outbound-initiated access) and server operations (inbound-initiated access). This allows the app to send requests to download or upload files
 to HTTP or FTP servers on the Internet or on a local intranet. This is represented by the
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
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Background Transfer is only enabled on Windows if at least one of the networking capabilities is set:
<b>Internet (Client)</b>, <b>Internet (Client &amp; Server)</b>, or <b>Private Networks (Client &amp; Server)</b>.
</p>
<p></p>
<p>This sample is currently provided in the JavaScript, C#, VB, and C&#43;&#43; programming languages.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Other resources</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452752">Adding support for networking</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770532">How to configure network isolation capabilities</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700370">Quickstart: Downloading a file</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700372">Quickstart: Uploading a file</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761434">Transferring a file from a network resource</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207126"><b>BackgroundDownloader</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207140"><b>BackgroundUploader</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298639"><b>HttpClient</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770550">IXHR2</a> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207242"><b>Windows.Networking.BackgroundTransfer</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br227346"><b>Windows.Storage</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229787"><b>XHR</b></a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
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
<li>Select <b>BackgroundTransfer.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build BackgroundTransfer.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>BackgroundTransfer.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build BackgroundTransfer.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>BackgroundTransfer.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy BackgroundTransfer.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>BackgroundTransfer.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy BackgroundTransfer.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the Windows version of the sample</b></p>
<p>This sample requires that a web server is available for the app to access for uploading and downloading files. The web server must be started before the app is run. The web server must also have a
<i>BackgroundTransferSample</i> path available for uploads and downloads. The sample includes a PowerShell script that will install IIS on the local computer, create the
<i>BackgroundTransferSample</i> folder on the server, copy files to this folder, create firewall rules to allow access, and enable IIS.
</p>
<p>The easiest way to run the sample is to use the provided web server scripts. Browse to the
<i>Server</i> folder in your sample folder to setup and start the web server. There are two options possible.</p>
<p></p>
<ul>
<li>Start PowerShell elevated (Run as administrator) and run the following command:
<p><b>.\SetupServer.ps1</b></p>
<p>Note that you may also need to change script execution policy. </p>
</li><li>Start an elevated Command Prompt (Run as administrator) and run following command:
<p><b>PowerShell.exe -ExecutionPolicy Unrestricted -File SetupServer.ps1</b></p>
</li></ul>
<p></p>
<p>When the web server is not needed anymore, please browse to the <i>Server</i> folder in you sample folder and run one of the following:</p>
<p></p>
<ul>
<li>Start PowerShell elevated (Run as administrator) and run the following command:
<p><b>.\RemoveServer.ps1</b></p>
<p>Note that you may also need to change script execution policy. </p>
</li><li>Start an elevated Command Prompt (Run as administrator) and run following command:
<p><b>PowerShell.exe -ExecutionPolicy Unrestricted -File RemoveServer.ps1</b></p>
</li></ul>
<p></p>
<p>The sample can run using any web server, not only the one provided with the sample. If IIS is used on a different computer, then the previous scripts can be used with minor changes.
</p>
<ul>
<li>Copy the <i>Server</i> folder to the device where IIS will be run. </li><li>Run the above scripts to install IIS, create the <i>BackgroundTransferSample</i> folder on the server, copy files to this folder, and enable IIS.
</li></ul>
<p></p>
<p>The sample must also be updated when run against a non-localhost web server. To configure the sample for use with IIS on a different device:
</p>
<ul>
<li>The hostname of the server to connect to needs to be updated. This can be handled in two ways. The
<b>AddressField</b> element in the HTML or XAML files can be edited so that &quot;localhost&quot; is replaced by the hostname or IP address of the web server. Alternately when the app is run, enter the hostname or IP address of the web server instead of the &quot;localhost&quot;
 value in the <b>Remote address</b> textbox. </li></ul>
<p></p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;IIS is not available on ARM builds nor on Windows Phone. Instead, set up the web server on a separate 64-bit or 32-bit computer and follow the steps for using the sample against a non-localhost web server.
</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;When used with the supplied scripts, this Windows Runtime app sample communicates with another process (IIS server which is a desktop app) on the same machine over loopback for demonstration purposes only. A Windows Runtime app
 that communicates over loopback to another process that represents a Windows Runtime app or a desktop app is not allowed and such apps will not pass Store validation. For more information, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh780593">How to enable loopback and troubleshoot network isolation</a>.
</p>
<p>However if a server different than IIS is used, then this requires some special configuration of the server to create the
<i>BackgroundTransferSample</i> folder. </p>
<dl><dd>Copy the <i>Server\webSite</i> directory to the <i>BackgroundTransferSample</i> folder on the web server and configure the server to allow download and upload requests.
</dd></dl>
<p></p>
<p>To configure the sample for use with a web server different than IIS not using localhost:</p>
<dl><dd>The remote server address and local filename fields should be updated. This can be handled in two ways. The
<b>serverAddressField</b> and <b>fileNameField</b> elements in the HTML or XAML files can be edited so that the remote server address and filename are replaced by a server address and filename for the non-IIS server. Alternately when the app is run, enter the
 remote address and local filename to access on the web server instead of the default values in the
<b>Remote address</b> and <b>Local file name</b> fields displayed. </dd></dl>
<p></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>BackgroundTransfer.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
<p><b>Deploying and running the Windows Phone version of the sample</b></p>
<p>IIS is not available on Windows Phone. For the app to access a web server, there are two options:</p>
<ul>
<li>The easiest way to run the sample is to use the provided web server scripts on a separate 64-bit or 32-bit device that can run IIS. Follow the instructions for deploying and running the Windows version of the sample using IIS on a different device.
</li><li>Use a web server different than IIS on a separate device. Follow the instructions for deploying and running the Windows version of the sample using a non-IIS web server.
</li></ul>
<p></p>
<ul>
<li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>BackgroundTransfer.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
