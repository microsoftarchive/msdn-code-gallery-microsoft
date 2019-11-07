# Lock screen apps sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- User Interface
## Updated
- 04/16/2014
## Description

<div id="mainSection">
<p>This sample shows how an app can have a presence on the lock screen—the screen that is shown when the computer is locked—with a badge to provide basic status information or a tile to provide more detailed status. An app can also send toast notifications
 to the lock screen. All apps that are granted a lock screen presence also have the ability to perform background tasks.
</p>
<p>The sample demonstrates the following scenarios: </p>
<ul>
<li>Requesting lock screen access for the app </li><li>Querying for the current lock screen access status of the app </li><li>Removing the app's lock screen access at its request </li><li>Sending a badge update notification to an app's badge on the lock screen </li><li>Sending a text-only tile update notification to an app's tile on the lock screen
</li><li>Using secondary tiles with the lock screen </li></ul>
<p></p>
<p>An app that has a lock screen presence must declare one or more of the following types of background tasks:
</p>
<ul>
<li>Control channel </li><li>Timer </li><li>Push notification </li></ul>
<p></p>
<p>In Microsoft Visual Studio&nbsp;2013, this value is set in the <b>Declarations</b> page of the manifest editor, which sets the
<a href="http://msdn.microsoft.com/library/windows/apps/br211421"><b>BackgroundTasks</b></a> element in the package.appxmanifest file. This value has been set for this sample.</p>
<p>For an app to send a toast notification, the developer must have declared that the app is toast-capable in its app manifest file (package.appxmanifest) as has been done in this sample app. Normally, you do this by using the Visual Studio&nbsp;2013 manifest editor,
 where you'll find the setting in the <b>Application UI</b> tab, under the <b>Notifications</b> section. For more info, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh781238">How to opt in for toast notifications</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465403">Guidelines and checklist for tiles and badges</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700416">Quickstart: Showing tile and badge updates on the lock screen</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779720">Lock screen overview</a>
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
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Download the sample's .zip file using one of the buttons near the top of the page.
</li><li>Unzip the downloaded file into a folder on your computer. </li><li>Start Visual Studio&nbsp;2013 and select <b>File &gt; Open &gt; Project/Solution</b>.
</li><li>Go to the folder where you unzipped the sample. </li><li>Find and open the folder named for the sample and one of its programming language subfolders (C#, JS.
</li><li>Double-click the Microsoft Visual Studio Solution (.sln) file to open it. </li><li>Select <b>Build &gt; Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug &gt; Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug &gt; Start Without Debugging</b>.</p>
<h2><a id="How_to_use_the_sample"></a><a id="how_to_use_the_sample"></a><a id="HOW_TO_USE_THE_SAMPLE"></a>How to use the sample</h2>
<p>An app is allowed to ask a user to add the app to the lock screen only one time. When you first run this sample, you must select
<b>Request lock screen access</b> in the first scenario before attempting to run any other scenario. At that point, you are presented with a dialog box. Regardless of the answer you choose, because of the &quot;ask only one time&quot; rule, you will not see this dialog
 box again unless you uninstall and reinstall the sample.</p>
<p>When you choose <b>Allow</b> from the dialog box, the sample app is added to the lock screen. You can verify this by going to
<b>PC settings</b> and examining the <b>Lock screen apps</b> list to see the sample's logo listed there. To remove the app from the lock screen, click the logo and click
<b>Don't display a badge here</b>. Because the sample app itself can't ask you to add it to the lock screen a second time, if you do remove the app here, you'll have to re-add it here as well, by clicking on one of the plus signs and selecting it from the Flyout.</p>
<p>Return to the sample and select scenario 2. Click the <b>Send badge notification</b> button.</p>
<p>Lock your computer either by clicking your user tile in the upper right corner of the Start screen and selecting
<b>Lock</b> or by pressing the Windows logo key &#43; L. Notice the app's logo and the badge displaying the number 6.</p>
<p>Log back in and go to <b>PC settings</b>. Click the <b>Send tile notification</b> button. Under
<b>Lock screen apps</b>, click the item under <b>Choose an app to display detailed status</b> and select the sample app from the flyout menu. This step must be performed to send a tile notification to the lock screen. This action can only be performed by the
 user—it cannot be done programmatically.</p>
<p>Return to the sample and again select scenario 2. Click the <b>Send tile notification</b> button.</p>
<p>One more time, lock your computer to see the lock screen. Notice the tile text that appears above the date.</p>
<p>Return to the sample. Scenario 3 deals with secondary tiles, which are tiles that reference a specific location or experience inside of the parent app. Only the user can create and remove secondary tiles. When you select &quot;Create a badge-only secondary tile&quot;
 or &quot;Create a badge a tile text secondary tile&quot; you are taking the role of the user and granting permission for those secondary tiles to be created on the Start screen.</p>
<p>After you've created one or both of those tiles, you must manually add them to the lock screen. Go to
<b>PC settings</b>, click one of the plus signs, and choose the secondary tile (for instance, &quot;LockScreen JS - Badge only&quot;). If you created the badge and text secondary tile, assign it to the &quot;Choose an app to display detailed status&quot; slot. (Note: Despite its
 name, the badge and text secondary tile does not display a badge.) Lock your computer to see those items on the lock screen.</p>
</div>
