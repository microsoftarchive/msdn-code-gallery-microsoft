# USSD message management sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Networking
- Mobile Broadband
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates network account management using the USSD protocol with GSM-capable mobile broadband devices. USSD is typically used for account management of a mobile broadband profile by the Mobile Network Operator (MNO). USSD messages are specific
 to the MNO and must be chosen accordingly when used in a live network.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=301754">Mobile Broadband on the Windows Hardware Dev Center</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207353"><b>MobileBroadbandAccount</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207402"><b>UssdMessage</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241143"><b>UssdSession</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241133"><b>UssdResultCode</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br241148"><b>Windows.Networking.NetworkOperators</b></a>
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
<p>To debug the app and then run it, press F5 or use <b>Debug &gt; Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug &gt; Start Without Debugging</b>.</p>
<p>The USSD SDK sample accesses privileged APIs and requires a custom signed Mobile Broadband account metadata package that references this application or the application accessing the device in order to run. The application will display an
<b>Access is Denied</b> error message if the metadata package does not explicitly grant permission to this application. For info about Mobile Broadband, see the
<a href="http://go.microsoft.com/fwlink/p/?LinkID=301754">Windows Hardware Dev Center</a>.</p>
</div>
