# DirectX marble maze game sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- DirectX
- Windows 8.1
- Windows Phone 8.1
## Topics
- Audio and video
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how to build a basic 3D game using DirectX. This game is a simple labyrinth game where the player is challenged to roll a marble through a maze of pitfalls using tilt controls.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Areas covered include:</p>
<ul>
<li>Incorporating the Windows Runtime into your DirectX game for full Windows Developer Preview support
</li><li>Using DirectX to render 3D graphics for display in a game </li><li>Implementing simple vertex and pixel shaders with HLSL </li><li>Developing simple physics and collision behaviors in a DirectX game </li><li>Handling input from accelerometer, touch, and mouse, and game controller with the Windows Runtime and XInput
</li><li>Playing and mixing sound effects and background music with XAudio2 </li></ul>
<p></p>
<p>This sample is written in C&#43;&#43; and requires some experience with graphics programming and DirectX. Complete content that examines this code can be found at
<a href="http://msdn.microsoft.com/library/windows/apps/br230257">Developing Marble Maze, a game in C&#43;&#43; and DirectX</a>.</p>
<p>For more info about the concepts and APIs demonstrated in this sample, see these topics:
</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/br229580">Create an app using DirectX</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780567">Walkthrough: create a simple game with DirectX</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/ff476080">Direct3D 11 graphics</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dd370987">Direct2D graphics</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/bb509561">DirectX HLSL</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh405049">XAudio2</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/ee417001">XInput</a> </li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh452744">Developing games</a>
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
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p>This sample is a Universal sample and contains both Windows&nbsp;8.1 and Windows Phone 8.1 projects, as well as the code shared between them.</p>
<p></p>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>MarbleMaze.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>BuildMarbleMaze.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>MarbleMaze.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build MarbleMaze.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>To run this sample after building it, first right-click the target project (<b>MarbleMaze.Windows</b> or
<b>MarbleMaze.WindowsPhone</b>) in <b>Solution Explorer</b>, and select <b>Set as Startup Project</b>. Then, press F5 (run with debugging enabled) or Ctrl&#43;F5 (run without debugging enabled) from Visual Studio&nbsp;2013 Update&nbsp;2 for Windows&nbsp;8.1 (any SKU). (Or select
 the corresponding options from the <b>Debug</b> menu.)</p>
<h3><a id="Playing_the__game"></a><a id="playing_the__game"></a><a id="PLAYING_THE__GAME"></a>Playing the game</h3>
<p>The Marble Maze game can be played with touch controls, a tilt accelerometer, an Xbox 360 controller, or the mouse.</p>
<p>To start the game, click or tap <b>Start Game</b>. To view high scores for the local session, tap
<b>High Scores</b>.</p>
<p>The controls are as follows:</p>
<ul>
<li>Touch controls: press on the screen in the relative (to the marble) direction that you want the marble to roll.
</li><li>Xbox 360 controller: tilt the left analog stick in the direction that you want the marble to roll.
</li><li>Tile accelerometer: tilt the accelerometer-enabled device in the direction that you want the marble to roll.
</li><li>Mouse: Click and hold the left mouse button while the pointer is on the screen in the relative (to the marble) direction that you want the marble to roll.
</li></ul>
<p></p>
<p>(Some of these behaviors may not be available or identical in the Windows Phone 8.1 SDK emulator.)</p>
</div>
