# Push and periodic notifications client-side sample
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
- 11/24/2018
## Description

<div id="mainSection">
<p><img src="111740-image.png" alt="" align="middle">
</p>
<p>This sample shows how a client app can register and listen for push notifications that are sent from a web server. Push notifications can be used to update a badge or a tile, raise a toast notification, or launch a background task. Periodic notifications
 act in the other direction, polling a web server for tile or badge content at a fixed time interval.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The sample demonstrates the following actions: </p>
<ul>
<li>Requesting a channel URI through which the push notifications will be sent </li><li>Renewing a channel </li><li>Listening for push notifications from the web server </li><li>Polling a web server for updated tile content </li><li>Polling a web server for updated badge content </li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>For JavaScript/HTML developers</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465460">Sending push notifications with WNS</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465407">How to authenticate with the Windows Push Notification Service (WNS)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465412">How to request, create, and save a notification channel</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761476">How to set up periodic notifications for tiles</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761476">How to set up periodic notifications for badges</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465450">How to update a badge through push notifications</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465450">Quickstart: Sending a tile push notification</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761487">Quickstart: Sending a toast push notification</a>
</dt><dt><b>For C#/C&#43;&#43;/XAML developers</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868244">Sending push notifications with WNS</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868206">How to authenticate with the Windows Push Notification Service (WNS)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868221">How to request, create, and save a notification channel</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868228">How to set up periodic notifications for tiles</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868228">How to set up periodic notifications for badges</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868252">How to update a badge through push notifications</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868252">Quickstart: Sending a tile push notification</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868255">Quickstart: Sending a toast push notification</a>
</dt><dt><b>Related samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=241553">Raw notifications sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 Windows Store app samples download pack</a>
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
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>PushAndPeriodicNotifications.Windows</b> in <b>Solution Explorer</b>.
</li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy PushAndPeriodicNotifications.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>PushAndPeriodicNotifications.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy PushAndPeriodicNotifications.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>PushAndPeriodicNotifications.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>PushAndPeriodicNotifications.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li><li>Give it several seconds to launch in the emulator (it takes over the full screen, so if you're still seeing your Start screen tiles, the sample hasn't launched yet), after which you can find the sample in the Apps list. Add the tile's sample to the Start
 screen so that you can see the result of the action that you've taken in the sample. A tile must be pinned to the Start screen to receive notifications.
</li></ol>
</li></ul>
<h2><a id="How_to_use_the_sample"></a><a id="how_to_use_the_sample"></a><a id="HOW_TO_USE_THE_SAMPLE"></a>How to use the sample</h2>
<p>For any functionality that involves non-local content, the developer must have declared the &quot;Internet (Client)&quot; capability in the app's manifest. In the Visual Studio&nbsp;2013 manifest editor, this option is under the
<b>Capabilities</b> tab.</p>
<p>To run this sample, you need access to a web server that can store your notification channel, send push notifications to your client, and provide tile and badge update content when polled.</p>
<p>For tile and badge push notifications to work, your app tile must be able to receive notifications. Tile notifications can be disabled by a user for a single app or for all apps, or by a system administrator through group policy.</p>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><b></b>Windows Phone: External to this sample, you can experiment with push notifications in the Windows Phone emulator, through its
<b>Additional Tools</b> flyout. This uses only local resources and does not require a web server, but it appears to the app that it is receiving a notification through Windows Push Notification Services (WNS).</p>
</div>
</div>
</div>
