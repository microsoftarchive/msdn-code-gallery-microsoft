# Handling Activated, Suspending and Resuming events using the Windows Runtime
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- HTML5
- Windows Runtime
## Topics
- App model
## Updated
- 08/14/2014
## Description

<div id="mainSection">
<p>This sample shows you how to use the <strong>Windows.UI.WebUI.WebUIApplication</strong>
<strong>Activated</strong>, <strong>Suspending</strong> and <strong>Resuming</strong> events in your Windows Store app.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/br242314"><strong>Windows.UI.WebUI.WebUIApplication.Activated</strong></a> event will be fired when your app is being launched.
<a href="http://msdn.microsoft.com/library/windows/apps/br242314"><strong>Windows.UI.WebUI.WebUIApplication.Activated</strong></a> gives your app the opportunity to restore the last state the user saw it in. It also allows you to acquire any activation parameters
 passed to you by the system. <a href="http://msdn.microsoft.com/library/windows/apps/br242314">
<strong>Windows.UI.WebUI.WebUIApplication.Activated</strong></a> may also fire while your app is running if there are new activation parameters for your app to respond to.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/br242316"><strong>Windows.UI.WebUI.WebUIApplication.Suspending</strong></a> event fires whenever your app is being suspended by the system.
<a href="http://msdn.microsoft.com/library/windows/apps/br242316"><strong>Windows.UI.WebUI.WebUIApplication.Suspending</strong></a> gives your app the opportunity to save the user&rsquo;s current session so that it can be restored in the case that your app
 is terminated. You can also use this event to free exclusive system resources like files or references to external devices.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/br242315"><strong>Windows.UI.WebUI.WebUIApplication.Resuming</strong></a> event fires whenever your app is woken up from the suspended state. Because your app can stay suspended for hours or even
 days, it&rsquo;s possible that data within your app has gone stale. <a href="http://msdn.microsoft.com/library/windows/apps/br242315">
<strong>Windows.UI.WebUI.WebUIApplication.Resuming</strong></a> gives your app the opportunity to refresh this stale data. Note that you do not have to restore saved state during resume.
<a href="http://msdn.microsoft.com/library/windows/apps/br242315"><strong>Windows.UI.WebUI.WebUIApplication.Resuming</strong></a> keeps your app in memory so your app&rsquo;s previous state is still loaded.</p>
<p>Windows attempts to keep as many suspended apps in memory as possible. By keeping these apps in memory, Windows ensures that users can quickly and reliably switch between suspended apps. However, if there aren't enough resources to keep your app in memory,
 Windows can terminate your app. Note that apps don't receive notification that they are being terminated, so the only opportunity you have to save your app's data is during suspension. When an app determines that it is activated after being terminated, it
 should load the application data that it saved during suspend so that the app appears as it did when it was suspended.</p>
<p>Generally, users don't need to close apps, they can let Windows manage them. However, a user can choose to close an app. There's no special event to indicate that the user has closed an app. After an app has been closed by the user, it's suspended and terminated.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p>&nbsp;</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the
<a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p>&nbsp;</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><strong>Tasks</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465102">How to activate an app (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465093">How to activate an app (C#/VB/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465138">How to suspend an app (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465115">How to suspend an app (C#/VB/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465114">How to resume an app (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465110">How to resume an app (C#/VB/C&#43;&#43;)</a>
</dt><dt><strong>Guidelines</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465088">Guidelines for app suspend and resume</a>
</dt><dt><strong>Concepts</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh464925">Application lifecycle</a>
</dt><dt><strong>Reference</strong> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224691"><strong>Windows.ApplicationModel</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224766"><strong>Windows.ApplicationModel.Activation</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br205865"><strong>Windows.ApplicationModel.Core</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242317"><strong>Windows.UI.WebUI</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242324"><strong>Windows.UI.Xaml.Application</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229774"><strong>WinJS.Application</strong></a>
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
<li>Start Visual Studio&nbsp;2013 and select <strong>File &gt; Open &gt; Project/Solution</strong>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.
</li><li>Press F7 or use <strong>Build &gt; Build Solution</strong> to build the sample.
</li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <strong>Debug &gt; Start Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use
<strong>Debug &gt; Start Without Debugging</strong>.</p>
</div>
