# Input: Simplified ink sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Devices and sensors
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p></p>
<p>This sample demonstrates how to use ink functionality (such as, capturing, manipulating, and interpreting ink strokes) in Windows Store apps using JavaScript, C#, and C&#43;&#43;. This is a lightweight version of
<a href="http://go.microsoft.com/fwlink/p/?linkid=231622">Input: Ink sample</a> that exposes app commands through keyboard shortcuts instead of the app UI.</p>
<p>Specifically, this sample covers using the <a href="http://msdn.microsoft.com/library/windows/apps/br208524">
<b>Windows.UI.Input.Inking</b></a> APIs to do the following:</p>
<ul>
<li>Render ink strokes </li><li>Save and load ink strokes </li><li>Copy and paste ink strokes </li><li>Select ink strokes </li><li>Delete ink strokes </li><li>Recognize handwriting from ink strokes </li></ul>
<p></p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Read the following instructions before using the app.</p>
<ul>
<li>Draw ink strokes with a pen or with a mouse while pressing the left mouse button.
</li><li>Erase ink strokes with a pen eraser tip, a pen while pressing the barrel button, or a mouse while pressing the right mouse button.
</li><li>Select ink strokes by drawing a lasso around the strokes with a pen while pressing the barrel button, or a mouse while pressing both the left mouse button and the CTRL key on the keyboard.
</li><li>You can also use the following keyboard shortcuts to manage, manipulate, and process ink strokes:
<ul>
<li>CTRL&#43;A to select all strokes. </li><li>CTRL&#43;C to copy selected strokes onto the clipboard. </li><li>CTRL&#43;D to change drawing attributes. </li><li>CTRL&#43;O to read Graphics Interchange Format (GIF) with embedded Ink Serialized Format (ISF) files from the Libraries or Pictures folder (or both).
</li><li>CTRL&#43;R to change the ink recognizer. </li><li>CTRL&#43;S to write GIF with embedded ISF files to the Libraries or Pictures folder (or both).
</li><li>CTRL&#43;T to copy text from ink recognition onto the clipboard. </li><li>CTRL&#43;V to paste strokes from the clipboard. </li><li>Space to perform ink recognition. </li><li>Backspace to delete strokes. </li></ul>
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
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231622">Input: Ink sample</a>
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
<p>To build this sample:</p>
<ol>
<li>Start Visual Studio and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</div>
