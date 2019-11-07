# Secondary tiles sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- User Interface
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p><img src="111760-image.png" alt="" align="middle">
</p>
<p>This sample shows how to pin and use a secondary tile, which is a tile that directly accesses a specific, non-default section or experience within an app, such as a saved game or a specific friend in a social networking app. Sections or experiences within
 an app that can be pinned to the Start screen as a secondary tile are chosen and exposed by the app, but the creation of the secondary tile is strictly up to the user.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The sample demonstrates the following scenarios: </p>
<ul>
<li>Pinning a secondary tile to the Start screen </li><li>Removing a secondary tile from the Start screen </li><li>Enumerating all secondary tiles owned by the calling app </li><li>Determining whether a particular tile is currently pinned to the Start screen
</li><li>Processing arguments when the app is activated through a secondary tile </li><li>Sending a local tile notification and badge notification to the secondary tile
</li><li>Using the app bar to pin and unpin tiles. (JavaScript and C# only) </li><li>Updating the secondary tile's default logo </li><li>Selecting from among alternative secondary tile visuals (Windows only) </li><li>Selecting from among alternative secondary tile visuals as an asynchronous operation (Windows only)
</li></ul>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Some functionality in the sample requires that the tile can receive notifications. Tile notifications can be disabled by a user for a single app or for all apps, or by a system administrator by using group policy.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465398">Guidelines and checklist for secondary tiles</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465443">Quickstart: Pinning a secondary tile</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761474">Quickstart: Sending notifications to a secondary tile</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242183"><b>SecondaryTile class</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465372">Secondary tiles overviews</a>
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
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>SecondaryTiles.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy SecondaryTiles.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>SecondaryTiles.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy SecondaryTiles.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>SecondaryTiles.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>SecondaryTiles.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li><li>Give it several seconds to launch in the emulator (it takes over the full screen, so if you're still seeing your Start screen tiles, the sample hasn't launched yet), after which you can find the sample in the Apps list. Add the tile's sample to the Start
 screen so that you can see the result of the action that you've taken in the sample. A tile must be pinned to the Start screen to receive notifications.
</li></ol>
</li></ul>
<h2><a id="How_to_use_the_sample"></a><a id="how_to_use_the_sample"></a><a id="HOW_TO_USE_THE_SAMPLE"></a>How to use the sample</h2>
<p>In some of the scenarios, you need to switch to the Start screen to see the effect of the scenario on the secondary tile. Click the sample tile to return to the main sample page or the secondary tile (in this sample, the secondary tile simply says &quot;Windows
 SDK&quot;) to be taken into an area of the sample that confirms that you've launched it from a secondary tile.</p>
<p>Note that the response to pinning a tile differs between Windows and Windows Phone. On Windows Phone, when you pin a secondary tile, you exit the app and are taken to the Start screen. In Windows, you must manually switch to the Start screen to see the secondary
 tile.</p>
</div>
