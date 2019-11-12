# Input: Ink sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- HTML5
- Windows Runtime
## Topics
- Devices and sensors
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p></p>
<p>This sample demonstrates how to use ink functionality (such as, capturing, manipulating, and interpreting ink strokes) in Windows Store apps using JavaScript.
</p>
<p>Specifically, this sample covers using the <a href="http://msdn.microsoft.com/library/windows/apps/br208524">
<b>Windows.UI.Input.Inking</b></a> APIs to:</p>
<ul>
<li>Render ink strokes </li><li>Save and load ink strokes </li><li>Copy and paste ink strokes </li><li>Select ink strokes </li><li>Delete ink strokes </li><li>Recognize handwriting from ink strokes </li><li>Search for a string within recognition results </li></ul>
<p></p>
For a lightweight version of this sample that exposes app commands through keyboard shortcuts, see
<a href="http://go.microsoft.com/fwlink/p/?linkid=246570">Input: Simplified ink sample</a>.
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Read the following instructions before using the app.</p>
<ul>
<li>Draw ink strokes with a tablet pen. </li><li>Activate the app bar for color pickers, line selectors, and other functions. </li><li>In <b>Erase</b> mode, a stroke deletes all strokes that it touches. If the pen has an eraser, it erases strokes (switching back to the pen tip exits erase mode).
</li><li>In <b>Select</b> mode, a stroke selects all strokes that it encompasses. Exit
<b>Select</b> mode by choosing another function on the app bar.
<p class="note"><b>Note</b>&nbsp;&nbsp; Ink color or width is applied to all selected strokes and highlight color or width is applied to all selected highlights.
</p>
</li><li>In <b>Recognition</b> mode, all (or only selected) strokes are processed through handwriting recognition. After recognition has completed:
<ul>
<li>View the primary text candidate(s) near the top of the screen. </li><li>Tap on a word to show all text candidates for that word. </li><li>Use <b>Find</b> to search for a string within the recognition results. </li></ul>
</li><li>Delete all strokes (or only selected strokes) by pressing the <b>Erase</b> button.
</li><li>Press the <b>More</b> button to:
<ul>
<li>Use <b>Copy</b> to copy selected strokes onto the clipboard. </li><li>Use <b>Paste</b> to paste strokes from the clipboard. </li><li>Use <b>Save</b> and <b>Load</b> to read and write Ink Serialized Format (ISF) files to the Libraries or Pictures folder or both.
</li></ul>
</li><li>
<p>All buttons and menu choices can be activated with mouse, pen, or touch. </p>
</li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/">Getting started with apps</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700425">Responding to pen and stylus interactions</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700412">Responding to user interaction</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=246570">Input: Simplified ink sample</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208524"><b>Windows.UI.Input.Inking</b></a>
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
<li>Start Visual Studio&nbsp;2013 and select <b>File &gt; Open &gt; Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.
</li><li>Press F7 or use <b>Build &gt; Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</div>
