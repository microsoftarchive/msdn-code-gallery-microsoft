# Real-time communication sample
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
<p>This sample demonstrates how to use the low latency feature to enable real-time communication applications.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Specifically, this sample contains: </p>
<ul>
<li>
<p>A simple end-to-end video call client that demonstrates the low latency mode of the Windows Runtime capture engine. This is enabled using the
<a href="http://msdn.microsoft.com/library/windows/apps/hh767377"><b>msRealTime</b></a> the
<a href="http://msdn.microsoft.com/library/windows/apps/hh767390"><b>video</b></a> tag or
<a href="http://msdn.microsoft.com/library/windows/apps/br227414"><b>RealTimePlayback</b></a> on the
<a href="http://msdn.microsoft.com/library/windows/apps/br242926"><b>MediaElement</b></a>. The sample uses a custom network source and a custom sink extension to send and receive captured audio and video data between two computers.
</p>
</li><li>
<p>A demonstration of the end-to-end latency of video captured using the <a href="http://msdn.microsoft.com/library/windows/apps/br226738">
<b>Media Capture</b></a> API and displayed using a <a href="http://msdn.microsoft.com/library/windows/apps/hh767390">
<b>video</b></a> and <a href="http://msdn.microsoft.com/library/windows/apps/br242926">
<b>MediaElement</b></a> with low latency mode enabled. Two output windows are displayed. The first shows a camera preview window of the raw output from your camera. The second is a local host client window that shows the video from the camera when compressed,
 streamed, and received over machine's loopback network interface. This window demonstrates the end-to-end latency of video captured, streamed to, and displayed by a remote client minus network latency.
</p>
</li></ul>
<p></p>
<p></p>
<p class="note"><b>Important</b>&nbsp;&nbsp; </p>
<p class="note">This sample uses the Media Extension feature of Windows&nbsp;8.1 to add functionality to the Microsoft Media Foundation pipeline. A Media Extension consists of a hybrid object that implements both Component Object Model (COM) and Windows Runtime
 interfaces. The COM interfaces interact with the Media Foundation pipeline. The Windows Runtime interfaces activate the component and interact with the Windows Store app.
</p>
<p class="note">In most situations, it is recommended that you use Visual C&#43;&#43; with Component Extensions (C&#43;&#43;/CX ) to interact with the Windows Runtime. But in the case of hybrid components that implement both COM and Windows Runtime interfaces, such as Media
 Extensions, this is not possible. C&#43;&#43;/CX can only create Windows Runtime objects. So, for hybrid objects it is recommended that you use
<a href="http://go.microsoft.com/fwlink/p/?linkid=243149">Windows Runtime C&#43;&#43; Template Library</a> to interact with the Windows Runtime. Be aware that Windows Runtime C&#43;&#43; Template Library has limited support for implementing COM interfaces.</p>
<p class="note">For more info on creating a Media Foundation media extension in Windows Store app, see Walkthrough:
<a href="http://go.microsoft.com/fwlink/p/?LinkID=309355">Creating a Windows Store app using WRL and Media Foundation</a> and the
<a href="http://go.microsoft.com/fwlink/p/?linkid=241427">Media extension sample</a>.</p>
<p class="note"></p>
<p></p>
<p class="note"><b>Important</b>&nbsp;&nbsp;The binaries used by this sample have been included for proof of concept purposes only. They might have significant performance, reliability, and security issues and should not be used outside of a test environment. They
 are not licensed for use in a production environment or for use with sensitive data.</p>
<p></p>
<p></p>
<p class="note"><b>Important</b>&nbsp;&nbsp;The URL passed to the code is not validated or authenticated. The application must perform these actions.</p>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Roadmaps</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for apps using C# and Visual Basic</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700360">Roadmap for apps using C&#43;&#43;</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465037">Roadmap for apps using JavaScript</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh767284">Designing UX for apps</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465134">Adding multimedia</a>
</dt><dt><b>Tasks</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452742">How to enable low-latency playback</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=309355">Creating a Windows Store app using WRL and Media Foundation</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226738"><b>Windows.Media.Capture.MediaCapture</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241124"><b>MediaCapture</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh767377"><b>msRealTime</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br240987"><b>MediaExtensionManager</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701296"><b>Windows.Media.MediaProperties</b></a>
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
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>SimpleCommunication.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build SimpleCommunication.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>SimpleCommunication.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build SimpleCommunication.WindowsPhone</b>. </li></ol>
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
<li>Select <b>SimpleCommunication.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy SimpleCommunication.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>SimpleCommunication.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy SimpleCommunication.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>SimpleCommunication.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>SimpleCommunication.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
