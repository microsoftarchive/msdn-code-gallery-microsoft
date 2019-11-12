# Input: Gestures and manipulations with GestureRecognizer
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
<p>This sample demonstrates how to handle pointer input and use the <a href="http://msdn.microsoft.com/library/windows/apps/br241937">
<b>GestureRecognizer</b></a>&nbsp;APIs to process gestures and manipulations in a Windows Store app.
</p>
<p>Specifically, this sample covers the following:</p>
<ul>
<li>Listen for and handle pointer input events. </li><li>Creating and attaching a gesture recognizer to each UI object that can be manipulated.
</li><li>Using <a href="http://msdn.microsoft.com/library/windows/apps/br241971"><b>GestureSettings</b></a> to configure the gesture recognizer to process manipulationRotate, manipulationTranslateX, manipulationTranslateY, manipulationScale, manipulationRotateInertia,
 manipulationScaleInertia, manipulationTranslateInertia, press and hold (right tap), and tap data.
</li><li>Handling manipulation events, such as <a href="http://msdn.microsoft.com/library/windows/apps/br241957">
<b>ManipulationStarted</b></a>, <a href="http://msdn.microsoft.com/library/windows/apps/br241958">
<b>ManipulationUpdated</b></a>, and <a href="http://msdn.microsoft.com/library/windows/apps/br241955">
<b>ManipulationCompleted</b></a>. </li><li>Using transformation matrices to calculate rotation, translation, and scale manipulations.
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;</p>
<p class="note">This sample does not cover creating a gesture recognizer pool and dynamically sharing recognizers between UI objects.</p>
<p></p>
<p></p>
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
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Conceptual</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">Getting started with apps</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465370">Guidelines for common user interactions</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700412">Responding to user interaction (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465397">Responding to user interaction (VB/C#/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh994926">Guidelines for user interaction (DirectX and C&#43;&#43;)</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242084"><b>Windows.UI.Input</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208383"><b>Windows.UI.Core</b></a>
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
