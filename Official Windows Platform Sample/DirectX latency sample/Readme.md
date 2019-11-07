# DirectX latency sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- DirectX
- Windows Runtime
## Topics
- frame presentation
- DXGI swap chain
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates using the lower-latency frame presentation APIs added in Windows&nbsp;8.1.
</p>
<p>Windows&nbsp;8.1 includes a new set of APIs for DirectX apps to present frames with lower latency, allowing for faster UI response. By using the new APIs, apps can wait for the DXGI swap chain to present - as opposed to having the swap chain wait on the app.
 Like existing block-on-present behavior, the desired frame latency is adjustable using API calls. For more information see the following APIs used in this sample:</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/dn268309"><b>GetFrameLatencyWaitableObject</b></a> method
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/bb173076"><b>DXGI_SWAP_CHAIN_FLAG</b></a> enumeration
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/ms687036"><b>WaitForSingleObjectEx</b></a> method
</li></ul>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8.1 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=243667">
Windows&nbsp;8.1 app samples pack</a>. The samples in the Windows&nbsp;8.1 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2013.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/bb173076"><b>DXGI_SWAP_CHAIN_FLAG</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/ms687036"><b>WaitForSingleObjectEx</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=243667">Windows 8.1 app samples</a>
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
