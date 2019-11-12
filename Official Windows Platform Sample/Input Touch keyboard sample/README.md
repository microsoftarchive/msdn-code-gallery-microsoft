# Input: Touch keyboard sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- Windows Runtime
## Topics
- Devices and sensors
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how the touch keyboard can be launched automatically in custom controls that are not derived from platform controls.
</p>
<p>The sample implements custom controls that require keyboard input and are not derived from standard XAML controls that accept input (such as TextBox, RichTextBox, or PasswordBox). The sample implements the following UI Automation Control Pattern interfaces:
 the TextPattern provider interface (<a href="http://msdn.microsoft.com/library/windows/apps/br242627"><b>ITextProvider</b></a>) and the ValuePattern provider interface (<a href="http://msdn.microsoft.com/library/windows/apps/br242663"><b>IValueProvider</b></a>).
 This ensures that the touch keyboard is invoked when a user taps on the custom control.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Conceptual</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700412">Responding to user interaction</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh972345">Guidelines and checklist for touch keyboard</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">Getting started with apps</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208383"><b>Windows.UI.Core</b></a>
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
<li>
<p>Start Visual Studio and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</p>
</li><li>
<p>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.
</p>
</li><li>
<p>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </p>
</li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </p>
</div>
