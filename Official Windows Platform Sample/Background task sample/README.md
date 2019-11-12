# Background task sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- App model
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows you how to create and register background tasks using the Windows Runtime background task API.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>A background task is triggered by a system or time event and can be constrained by one or more conditions. When a background task is triggered, its associated handler runs and performs the work of the background task. A background task can run even when
 the app that registered the background task is suspended.</p>
<p>This sample demonstrates the following:</p>
<ul>
<li>Creating and registering background tasks written in C&#43;&#43;, C#, or JavaScript. </li><li>Creating a background task that is triggered by a system event. </li><li>Requesting the user's permission to add the app to the lock screen. </li><li>Creating a background task that is triggered by a time trigger. </li><li>Adding a condition that constrains the background task to run only when the condition is in effect.
</li><li>Reporting background task progress and completion to the app. </li><li>Using a deferral object to include asynchronous code in your background task.
</li><li>Handling the cancellation of a background task, and ensuring the task is cancelled when required conditions are no longer met.
</li><li>Initializing background task progress and completion handlers when the app is launched.
</li></ul>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b></b></dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868260">Displaying tiles on the lock screen</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770837">Launching, resuming, and multitasking</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh977053">Managing background tasks</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh977056">Supporting your app with background tasks</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>API reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224847"><b>Windows.ApplicationModel.Background (Windows Store apps using C#/VB/C&#43;&#43; and XAML)</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701740"><b>Windows.UI.WebUI.WebUIBackgroundTaskInstance (Windows Store apps using JavaScript and HTML)</b></a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br224847"><b>Windows.ApplicationModel.Background</b></a>
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
<li>Select <b>BackgroundTask.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build BackgroundTask.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>BackgroundTask.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build BackgroundTask.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>BackgroundTask.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>BackgroundTask.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
<p><b>Triggering background tasks</b></p>
<p>To trigger the background tasks associated with the <code>TimeZoneChange</code> event:</p>
<ol>
<li>Change date and time settings. </li><li>Click <b>Change time zone...</b> </li><li>Select a time zone that has a UTC offset different from the currently selected time zone.
</li><li>Click <b>OK</b>. </li></ol>
<p>Background tasks associated with the <code>TimeTrigger</code> event will only fire if the app is currently on the lock screen. There are two ways to accomplish this.</p>
<p>Accept the initial request to add the BackgroundTaskSample app to the lock screen:</p>
<ol>
<li>Launch the BackgroundTaskSample app for the first time. </li><li>Register the TimeTrigger event. </li><li>Accept the request to add the BackgroundTaskSample app to the lock screen. </li></ol>
<p>Add the BackgroundTaskSample app to the lock screen manually:</p>
<ol>
<li>From the Start screen, go to <b>Settings</b> &gt; <b>Customize your lock screen</b>.
</li><li>Choose the BackgroundTaskSample app for the lock screen. </li><li>Launch the BackgroundTaskSample app and register the TimeTrigger event. </li></ol>
<p class="note"><b>Note</b>&nbsp;&nbsp;The minimum delay for creating TimeTrigger events is 15 minutes. The first timer event, however, might not occur until 15 minutes after it is expected to expire (30 minutes after the app registers the event).</p>
<h2><a id="Read_more"></a><a id="read_more"></a><a id="READ_MORE"></a>Read more</h2>
<p>See the following topics for step-by-step information about using background tasks:</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/hh977055">Quickstart: Create and register a background task</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh977058">How to respond to system events with background tasks</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh977057">How to set conditions for running a background task</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh977052">How to handle a cancelled background task</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh977054">How to monitor background task progress and completion</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh977059">How to run a background task on a timer</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/jj883699">How to use maintenance triggers</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh977049">How to declare background tasks in the application manifest</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh977051">Guidelines and checklists for background tasks</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/jj542416"><b>How to debug a background task</b></a>
</li></ul>
</div>
