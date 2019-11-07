# Application data sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Store
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows you how to store and retrieve data that is specific to each user and Windows Runtime app by using the Windows Runtime application data APIs (<a href="http://msdn.microsoft.com/library/windows/apps/br241587"><b>Windows.Storage.ApplicationData</b></a>
 and so on). </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Application data includes session state, user preferences, and other settings. It is created, read, updated, and deleted when the app is running. The operating system manages these data stores for your app:
</p>
<ul>
<li><b>local</b>: Data that exists on the current device and is backed up in the cloud
</li><li><b>roaming</b>: Data that exists on all devices on which the user has installed the app
</li><li><b>temporary</b>: Data that could be removed by the system any time the app isn't running
</li><li><b>localcache</b>: Persistent data that exists only on the current device </li></ul>
<p>If you use roaming data in your app, your users can easily keep your app's application data in sync across multiple devices. The operating system replicates roaming data to the cloud when it is updated, and synchronizes the data to any other devices on which
 the app is installed, reducing the amount of setup work that the user needs to do to install your app on multiple devices.
</p>
<p>If you use local data in your app, your users can back up application data in the cloud. This application data can then be restored back on any other device while setting up the device with the same account.</p>
<p>The sample covers these key tasks:</p>
<ul>
<li>Reading and writing settings to an app data store </li><li>Reading and writing files to an app data store </li><li>Responding to roaming events </li></ul>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Tasks</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465118">Quickstart: Local application data (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700361">Quickstart: Local application data (C#/VB/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465123">Quickstart: Roaming application data (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700362">Quickstart: Roaming application data (C#/VB/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465130">Quickstart: Temporary application data (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700363">Quickstart: Temporary application data (C#/VB/C&#43;&#43;)</a>
</dt><dt><b>Guidelines</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465094">Guidelines for roaming application data</a>
</dt><dt><b>Concepts</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh464917">Application data</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241587"><b>Windows.Storage.ApplicationData</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241588"><b>Windows.Storage.ApplicationDataCompositeValue</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241599"><b>Windows.Storage.ApplicationDataContainer</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241600"><b>Windows.Storage.ApplicationDataContainerSettings</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229774"><b>WinJS.Application</b></a>
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
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>ApplicationDataSample.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build ApplicationDataSample.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>ApplicationDataSample.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build ApplicationDataSample.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>ApplicationDataSample.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy ApplicationDataSample.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>ApplicationDataSample.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy ApplicationDataSample.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>ApplicationDataSample.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>ApplicationDataSample.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
