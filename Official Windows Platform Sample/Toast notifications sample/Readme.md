# Toast notifications sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- User Interface
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows you how to use toast notifications, which are notifications sent from an app to the user. Toast appears as a pop-up notification in the upper right corner of the screen. A user can select the toast (touch or click) to launch the associated
 app. Toast notifications can be sent locally or through a web service. This sample demonstrates the functionality and features of local toast notifications and lets you preview all the toast template types.</p>
<p>The sample demonstrates the following scenarios: </p>
<ul>
<li>Sending a text-only toast </li><li>Sending a toast that uses an image included in the app's package </li><li>Sending a toast that uses an image from the web </li><li>Specifying the sound that plays when a toast is displayed </li><li>Responding to events that arise from the user's response to the toast: a selection, a dismissal, or a time-out
</li><li>Using long-duration toast </li></ul>
<p></p>
<p>To see a sample of toast used as an alarm, download the <a href="http://go.microsoft.com/fwlink/?linkid=310148">
Alarm toast notification sample</a>.</p>
<p>For an app to send a toast notification, the developer must have declared that the app is toast-capable in its app manifest file (package.appxmanifest), as they have for this sample app. Normally, you will do this through the Microsoft Visual Studio&nbsp;2013
 manifest editor, where you find the setting in the <b>Application</b> tab, under the
<b>Notifications</b> section. For more information, see <a href="http://msdn.microsoft.com/library/windows/apps/hh781238">
How to opt in for toast notifications</a>.</p>
<p>For any functionality that involves non-local content, the developer must have declared the &quot;Internet (Client)&quot; capability in the app's manifest. In the Visual Studio&nbsp;2013 manifest editor, this option is under the
<b>Capabilities</b> tab.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Overviews</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779727">Toast notifications (concepts)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761494">The toast notification template catalog</a>
</dt><dt><b>JavaScript/HTML tutorials</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465448">Quickstart: Sending a toast notification</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh781238">How to opt in for toast notifications</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761468">How to handle activation from a toast notification</a>
</dt><dt><b>C#/C&#43;&#43;/XAML tutorials</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868254">Quickstart: Sending a toast notification</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868218">How to opt in for toast notifications</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh868212">How to handle activation from a toast notification</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208661"><b>Windows.UI.Notifications namespace</b></a>
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
<p>As you send the toast notifications from the different scenarios, watch for them to appear in the upper-right corner of the screen. Each toast will be dismissed by the system after a few seconds, but you can also manually dismiss them with a swipe or a click.</p>
</div>
