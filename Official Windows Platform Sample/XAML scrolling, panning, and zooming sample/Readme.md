# XAML scrolling, panning, and zooming sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Controls
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br209527">
<b>ScrollViewer</b></a> control to pan and zoom. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Specifically, this sample covers:</p>
<ul>
<li>Using panning and scrolling to enable users to reach content that won't otherwise fit within a view. You can use the
<a href="http://msdn.microsoft.com/library/windows/apps/br209549"><b>HorizontalScrollMode</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br209589"><b>VerticalScrollMode</b></a> properties to restrict panning to the horizontal or vertical axis or enable panning in any direction. You can use the
<a href="http://msdn.microsoft.com/library/windows/apps/hh968047"><b>IsHorizontalRailEnabled</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/hh968049"><b>IsVerticalRailEnabled</b></a> properties to lock the motion to the horizontal or vertical axis after panning has started. You can use the
<a href="http://msdn.microsoft.com/library/windows/apps/br209547"><b>HorizontalScrollBarVisibility</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br209587"><b>VerticalScrollBarVisibility</b></a> to show, hide, or disable scroll bars.
</li><li>Using snap points to help users reach key locations in your content. You can use mandatory snap points to automatically continue panning until a snap point is reached, or you can use proximity snap points to continue panning only when the current location
 is close to a snap point. You can specify the snap point type by setting the <a href="http://msdn.microsoft.com/library/windows/apps/br209553">
<b>HorizontalSnapPointsType</b></a> and <a href="http://msdn.microsoft.com/library/windows/apps/br209593">
<b>VerticalSnapPointsType</b></a> properties. </li><li>Enabling users to resize or zoom your content by setting the <a href="http://msdn.microsoft.com/library/windows/apps/br209601">
<b>ZoomMode</b></a> property. You can also set the minimum and maximum zoom levels through the
<a href="http://msdn.microsoft.com/library/windows/apps/br209567"><b>MinZoomFactor</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br209565"><b>MaxZoomFactor</b></a> properties.
</li></ul>
<p>This sample is written in XAML. For the HTML version, see the <a href="http://go.microsoft.com/fwlink/p/?linkid=242394">
Scrolling, panning, and zooming sample (HTML)</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Roadmaps</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for C# and Visual Basic</a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209527"><b>ScrollViewer</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209549"><b>HorizontalScrollMode</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209589"><b>VerticalScrollMode</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh968047"><b>IsHorizontalRailEnabled</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh968049"><b>IsVerticalRailEnabled</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209547"><b>HorizontalScrollBarVisibility</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209587"><b>VerticalScrollBarVisibility</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209553"><b>HorizontalSnapPointsType</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209593"><b>VerticalSnapPointsType</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209601"><b>ZoomMode</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209567"><b>MinZoomFactor</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209565"><b>MaxZoomFactor</b></a>
</dt><dt><b>Concepts</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br211380">Create an app using C# or Visual Basic</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465307">Guidelines for optical zoom and resizing</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465310">Guidelines for panning</a>
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
<p></p>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory containing the sample, and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Select either the Windows or Windows Phone project version of the sample. Press Ctrl&#43;Shift&#43;B, or select
<b>Build</b> &gt; <b>Build Solution</b>. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ol>
<li>Select either the Windows or Windows Phone project version of the sample. </li><li>Select <b>Build</b> &gt; <b>Deploy Solution</b>. </li></ol>
<p><b>Deploying and running the sample</b></p>
<ol>
<li>Right-click either the Windows or Windows Phone project version of the sample in
<b>Solution Explorer</b> and select <b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or select <b>Debug</b> &gt; <b>
Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or select<b>Debug</b> &gt;
<b>Start Without Debugging</b>. </li></ol>
</div>
