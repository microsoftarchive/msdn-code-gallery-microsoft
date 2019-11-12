# Alarm toast notifications sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- toast notifications
- alarm apps
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This code sample demonstrates how to raise an alarm toast notification that includes
<b>Snooze</b> and <b>Dismiss</b> buttons. Alarm toasts are available as of Windows&nbsp;8.1.</p>
<p>One <a href="http://msdn.microsoft.com/library/windows/apps/hh700416">lock screen app</a> can be designated by the user as the system alarm app, either through PC Settings or by accepting a permissions dialog. Once a lock screen app is designated as the
 system alarm app, it can schedule alarm notifications that are shown to the user within an accuracy of one second.</p>
<p>This sample demonstrates the following functionality for alarm apps: </p>
<ul>
<li>Raising an alarm toast notification </li><li>Setting a custom snooze duration for an alarm toast </li></ul>
<p></p>
<p>For general toast notification sample code, see the <a href="http://msdn.microsoft.com/library/windows/apps/">
Toast notifications sample</a>.</p>
<p>An app that has a lock screen presence must declare one or more of the following types of background tasks:</p>
<ul>
<li>Control channel </li><li>Timer </li><li>Push notification </li></ul>
<p>In the Microsoft Visual Studio manifest editor, this background task value is set in the
<b>Declarations</b> tab. This sets the <b>BackgroundTasks</b> element in the <code>
package.appxmanifest</code> file. The background task typically runs periodically to schedule new alarms, recurring alarms, or both. This sample uses a placeholder background task that is never scheduled.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="m_dev_guide.ux_ui#alarms">New API for alarm apps</a> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn268318"><b>Toast schema commands element</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn297281"><b>AlarmApplicationManager</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">Toast notifications sample</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">Lock screen apps sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows app samples</a>
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
</li><li>Unzip the downloaded file into a folder on your computer. </li><li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the folder where you unzipped the sample. </li><li>Find and open the folder named for the sample and one of its programming language subfolders (C#, JS.
</li><li>Double-click the Visual Studio Solution (.sln) file to open it. </li><li>Use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
<p>The first time (and only the first time) that you run the app, you will be shown a dialog that asks &quot;Replace your current alarm app with this one?&quot; To see the alarm on the lock screen, or to see it elsewhere with its snooze and dismiss buttons, you should
 select &quot;Yes&quot;. If you do not, you can replace the current alarm app with the sample app manually later through PC Settings. The only way to see the original dialog again is to uninstall and reinstall the app.</p>
<p>To use the sample:</p>
<ol>
<li>Select a delay time to specify the amount of time before the alarm appears. This must be between 1 and 60 minutes (inclusive).
</li><li>Either click the <b>Default Snooze</b> button to use the default snooze time of approximately 9 minutes or set a snooze time and set the
<b>Custom Snooze</b> button. </li><li>Wait for the alarm to appear or lock your computer (for instance by pressing Windows logo key &#43; L) to take you to the lock screen to see the alarm there.
</li><li>When the alarm appears, you can take these actions:
<ul>
<li>Select snooze. The alarm will reappear after the specified or default time. </li><li>Select dismiss. </li><li>Do nothing and the toast will dismiss itself after a time </li></ul>
</li></ol>
<p>If the alarm does not appear on the lock screen, go to PC Settings to ensure that the alarm notification sample is set as the alarm app.</p>
<p>Note that if you do not set the alarm notification app as the system alarm app, you can still see the alarm everywhere except the lock screen. However, it will not have the snooze or dismiss buttons. In that case, you have these options:</p>
<ul>
<li>Click or touch the toast body to launch the app. </li><li>Click the X (visible on mouse-over) or swipe the toast to dismiss it </li><li>Do nothing and the toast will dismiss itself after a time </li></ul>
</div>
