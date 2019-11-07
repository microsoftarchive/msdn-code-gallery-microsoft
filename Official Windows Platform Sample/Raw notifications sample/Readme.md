# Raw notifications sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- User Interface
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p><img src="111741-image.png" alt="" align="middle">
</p>
<p>This sample shows how to use raw notification, which are push notifications with no associated UI that performs a background task for the app. For example, a magazine app can download the latest issue in the background so that it is ready when the user subsequently
 switches to the app. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The sample demonstrates the following actions: </p>
<ul>
<li>Requesting access from the user to be allowed to run in the background (Windows only)
</li><li>Opening a channel URI through which the raw notifications will be sent </li><li>Registering a background task for the raw notification </li><li>Receiving a raw notification when your app is running and visible (Windows only)
</li><li>Unregistering a background task </li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231476">Push and periodic notifications sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 Windows Store app samples</a>
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
<li>Select <b>RawNotifications.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build RawNotifications.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>RawNotifications.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build RawNotifications.WindowsPhone</b>. </li></ol>
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
<li>Select <b>RawNotifications.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy RawNotifications.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>RawNotifications.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy RawNotifications.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>RawNotifications.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>RawNotifications.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li><li>Give it a few seconds to launch in the emulator, after which you can find the sample in the Apps list.
</li></ol>
</li></ul>
<h2><a id="How_to_use_the_sample"></a><a id="how_to_use_the_sample"></a><a id="HOW_TO_USE_THE_SAMPLE"></a>How to use the sample</h2>
<p>For an app to be capable of performing background tasks, it must declare those background tasks in its app manifest file (package.appxmanifest). For a raw notification, you must declare the &quot;Push notification&quot; background task as this sample has done. In
 Visual Studio&nbsp;2013 Update&nbsp;2, this value is set in the <b>Declarations</b> page of the manifest editor, which sets the
<a href="http://msdn.microsoft.com/library/windows/apps/br211421"><b>BackgroundTasks</b></a> element in the package.appxmanifest file.</p>
<p>For raw notifications to work, your app tile must be able to receive notifications. Tile notifications can be disabled by a user for a single app or for all apps, or by a system administrator by using group policy.</p>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><b></b>An app is allowed to ask a user to grant background task access only one time. When you first run this sample and select
<b>Open a channel and register background task</b>, a dialog box appears. Regardless of the answer you choose, because of the &quot;ask only one time&quot; rule, you will not see this dialog again unless you uninstall and reinstall the sample. When you choose
<b>Allow</b> from the dialog box, the sample app is added to the lock screen because the two capabilities are set as a pair. However, because a raw notification does not include UI, you will not see a badge or tile text associated with a raw notification on
 the lock screen.</p>
</div>
</div>
<p>The channel URI that is returned is sent to your web server as it would be with any push notification, and any subsequent actions taken by the server to send a raw notification using that channel URI, are also no different than other push notifications.
 For more information, see the <a href="http://go.microsoft.com/fwlink/p/?linkid=231476">
Push and periodic notifications</a> sample.</p>
</div>
