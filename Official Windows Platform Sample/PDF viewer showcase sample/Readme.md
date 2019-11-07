# PDF viewer showcase sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Portable Document Format (pdf)
## Updated
- 05/13/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how to work with Portable Document Format (PDF) files programmatically.
</p>
<p>Specifically, this sample shows how to:</p>
<ul>
<li>Scroll through various pages in a PDF file. </li><li>Use semantic zoom to switch between viewing single and multiple pages in a PDF file.
</li><li>Share a PDF file in an email message. </li></ul>
<p>For detailed info about how this sample works, see the companion <a href="http://go.microsoft.com/fwlink/p/?LinkId=306022">
PDF viewer end-to-end sample</a> documentation.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=306021">Windows.Data.Pdf</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=299582">PDF viewer sample</a>
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
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
<p>To test semantic zoom with the keyboard, press CTRL&#43;PLUS SIGN or CTRL&#43;MINUS SIGN. If your testing machine doesn't have touch enabled or does not support touch, run the app in
<b>Simulator</b> mode in Microsoft Visual Studio. After the app starts in the simulator, click the
<b>Pinch/zoom touch mode</b> icon on the simulator's edge, and then use the mouse to simulate pinch and zoom touch gestures. To perform these gestures, press and hold the left mouse button while rotating the mouse wheel backward or forward. To turn off pinch/zoom
 touch mode, click either the <b>Mouse mode</b> or <b>Basic touch</b> icon on the simulator's edge.</p>
<p>To test device rotation, if you have a desktop PC or your testing machine doesn't have rotation enabled, run the app in
<b>Simulator</b> mode in Visual Studio. After the app starts in the simulator, click the
<b>Rotate clockwise (90 degrees)</b> or <b>Rotate counterclockwise (90 degrees)</b> icon on the simulator's edge to simulate device rotation.</p>
<p>After the app starts, you can load a different PDF file. To do this, display the app bar: swipe from the top or bottom edge; or right-click the mouse; or with
<b>Basic touch</b> mode enabled in the simulator, with the mouse pointer anywhere on the top or bottom device frame, press and hold the left mouse button while dragging onto the app's surface. Click the open button on the app bar, and browse to and select the
 desired PDF file.</p>
<p>In the JavaScript sample: to share the loaded PDF file in an email message, display the charms bar: swipe from the right edge; or move the mouse pointer to the upper-right or lower-right corner; or with
<b>Basic touch</b> mode enabled in the simulator, with the mouse pointer anywhere on the right device frame, press and hold the left mouse button while dragging onto the app's surface. Click the
<b>Share</b> charm, and follow the on-screen instructions.</p>
</div>
