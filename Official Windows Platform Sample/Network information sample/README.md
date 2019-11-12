# Network information sample
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
<p>This sample shows you how to access network connectivity, cost, and usage information using classes in the
<a href="http://msdn.microsoft.com/library/windows/apps/br207308"><b>Windows.Networking.Connectivity</b></a> namespace in your Windows Runtime app. This sample is provided in the JavaScript, C#, VB.NET, and C&#43;&#43; programming languages.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>This sample demonstrates the following features:</p>
<p></p>
<ul>
<li>Retrieve connectivity details and usage information for network connections on a device using the
<a href="http://msdn.microsoft.com/library/windows/apps/br207293"><b>NetworkInformation</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br207249"><b>ConnectionProfile</b></a> classes. This includes connectivity and usage for the current Internet connection profile and retrieving a list of all connection profiles.
</li><li>Register for notifications of connection state change events and retrieve the connection state change information.
</li><li>Manage metered network cost constraints using best practices by maintaining awareness of network connection cost or data plan status changes.
</li><li>Retrieve connection usage data for a specific period of time. </li><li>Retrieve <a href="http://msdn.microsoft.com/library/windows/apps/br207278"><b>LanIdentifier</b></a> objects associated with adapters on a network and access information that can be used to determine its relative location within the network infrastructure.
</li></ul>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Use of this sample does not require Internet or intranet access so no network capabilities need to be set in the
<i>Package.appmanifest</i> file.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Other - C#/VB/C&#43;&#43; and XAML</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452985">Accessing network connection state and managing network costs (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj835820">How to manage network connection events and changes in availability (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj835821">How to manage metered network cost constraints (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452991">How to retrieve network connection information (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465163">How to retrieve network connection usage data for a specific period of time (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465168">How to retreive network adapter and locality information(Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><b>Other - JavaScript and HTML</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452983">Accessing network connection state and managing network costs (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">How to manage network connection state changes (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">How to manage connections on metered networks (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465167">How to retrieve network adapter and locality information (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700381">How to retrieve network connection information (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465162">How to retrieve network connection usage information for a specific time period (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207249"><b>ConnectionProfile</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207256"><b>DataPlanStatus</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207293"><b>NetworkInformation</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207308"><b>Windows.Networking.Connectivity</b></a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/?LinkID=393191">Network status background sample</a>
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
<li>Select <b>NetworkInformationApi.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build NetworkInformationApi.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>NetworkInformationApi.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build NetworkInformationApi.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>NetworkInformationApi.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy NetworkInformationApi.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>NetworkInformationApi.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy NetworkInformationApi.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>NetworkInformationApi.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>NetworkInformationApi.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
