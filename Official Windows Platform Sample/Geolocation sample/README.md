# Geolocation sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Services
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>The Geolocation sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br225603">
<b>Windows.Devices.Geolocation</b></a> namespace to get the geographic location of the user's device. An app can use the Geolocation namespace to get the location one time, or it can continuously track the location by getting location update events.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The sample also demonstrates how to:</p>
<ul>
<li>use the <a href="http://msdn.microsoft.com/library/windows/apps/dn263744"><b>Windows.Devices.Geolocation.Geofencing</b></a> namespace to get notifications when the user's device has entered or left an area of interest.
</li><li>read the <a href="http://msdn.microsoft.com/library/windows/apps/br225600"><b>StatusChangedEventArgs</b></a> to handle location status changed events.
</li><li>get the location using a background task. </li><li>handle doing geofencing as a <a href="http://msdn.microsoft.com/library/windows/apps/br224847">
<b>Background</b></a> task. </li><li>use the <a href="http://msdn.microsoft.com/library/windows/apps/jj635260"><b>GeocoordinateSatelliteData</b></a> to obtain additional information on the quality of the satellite based positioning obtained.
</li><li>display a toast when a background geofencing event has occurred. </li><li>refresh geofence binding on resume and after removal of a geofences. Note that removal requires subscribing to removal events if a geofence is set as single use or a duration is set.
</li><li>use of the <a href="http://msdn.microsoft.com/library/windows/apps/dn263600">
<b>DeviceAccessInformation.AccessChanged</b></a> event. Change the location permissions from the Settings charm to see the access change events.
</li></ul>
<p></p>
<p>Geofences need to be created in the <b>Foreground Geofencing</b> scenario (Scenario 4) and then you can go to
<b>Background Geofencing</b> scenario (Scenario 5) to register for background geofencing events.</p>
<p>The Geolocation sample formats and parses time and dates in the en-US locale using the Gregorian calendar and 24-hour clock. To help other locales in entering data the edit fields have a format example shown below the control. For example,
<b>Start Time</b> would be entered <b>mm/dd/yyyy hh:mm:ss</b> format. February 2, 2014 at 10:34 pm would be written 2/2/2014 22:34:00. For the
<b>Dwell Time</b> and <b>Duration</b> the format is dd:hh:mm:ss so a time span of 7 days, 23 hours, 45 minutes and 55 seconds would be written as 7:23:45:55.
</p>
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
</dt><dt><b>Tutorials</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465121">How to adjust the distance between location updates</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452755">How to respond to location updates</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465129">Quickstart: detecting the user's location</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/?LinkID=325245">Run Windows Store apps in the simulator</a>
</dt><dt><b>API Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br225603"><b>Windows.Devices.Geolocation</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn263744"><b>Windows.Devices.Geolocation.Geofencing</b></a>
</dt><dt><b>Guidelines</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465148">Guidelines for location-aware apps</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/hh465139">Detecting the user's location</a>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>None supported </dt></td>
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
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory containing the sample in the language you desire - either C&#43;&#43;, C#, JavaScript, or Visual Basic. Double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
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
