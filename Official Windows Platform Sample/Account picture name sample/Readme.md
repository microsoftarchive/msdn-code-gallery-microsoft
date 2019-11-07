# Account picture name sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- HTML5
- Windows Runtime
## Topics
- Controls
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates different ways of getting the name of the user that is currently logged in. It also demonstrates how to get and set the image used for the user's tile.
</p>
<p>Specifically, this sample demonstrates the following scenarios:</p>
<ul>
<li>How to get the <a href="http://msdn.microsoft.com/library/windows/apps/hh921595">
<b>DisplayName</b></a> for the current logged on user. </li><li>How to get the first and last name for the current logged on user. (This is only available for Microsoft accounts. An empty string is returned if a Microsoft account is not available.)
</li><li>How to obtain the logged on user's account picture as a bitmap. You can get request three different types: small, large, and video. If the size that is requested is not available an empty file is returned.
</li><li>How to set the account picture for the currently logged on user. You can set three different types: small, large, and video. (More than one type can be set in the same call, but a small image must be accompanied by a large image and/or video.)
</li><li>How to register for a change event for account picture updates. </li></ul>
<p>Check out <a href="http://msdn.microsoft.com/library/windows/apps/br241881"><b>Windows.System.UserProfile</b></a> for info about the classes and methods used in this sample.
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
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241881"><b>Windows.System.UserProfile</b></a>
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
<p>To debug the app and then run it, press F5 or use <b>Debug &gt; Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug &gt; Start Without Debugging</b>.</p>
</div>
