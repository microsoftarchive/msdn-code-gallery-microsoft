# WiFiDirectDevice sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- connect to Wi-Fi Direct devices
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>Demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/dn297617">
<b>WiFiDirectDevice</b></a> class to find and connect to nearby Wi-Fi Direct devices.
</p>
<p>You can use Wi-Fi Direct to both enumerate a list of Wi-Fi Direct devices within wireless range, and then set up a socket connection between apps using Wi-Fi Direct devices. The
<a href="http://msdn.microsoft.com/library/windows/apps/dn297617"><b>WiFiDirectDevice</b></a> sample demonstrates how to use the Wi-Fi Direct API to get a list of Wi-Fi Direct devices within range, and then create a socket connection between your device and
 a device from the list.</p>
<p>You need to enable the <b>Proximity</b> capability in your app manifest file in order to use the Wi-Fi Direct API.</p>
<p>You can also create a connection between peer apps (your app installed on more than one peer device) using Wi-Fi Direct and the
<a href="http://msdn.microsoft.com/library/windows/apps/hh701080"><b>FindAllPeersAsync</b></a> method in the Proximity API. For more information and example code, see the
<a href="http://go.microsoft.com/fwlink/p/?linkid=245082">Proximity sample</a> and
<a href="http://msdn.microsoft.com/library/windows/apps/hh465221">Proximity and tapping</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn297617"><b>WiFiDirectDevice</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn297687"><b>Windows.Devices.WiFiDirect</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows app samples</a>
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
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </p>
</div>
