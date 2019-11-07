# Creating a Windows Runtime DLL component with C++ sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- App model
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample shows how to create an in-process DLL component in Microsoft Visual C&#43;&#43; that's used in C&#43;&#43;/CX, JavaScript, and C# client code.
</p>
<p>The OvenServer project contains a runtime class named Oven, which implements an IOven interface and an IAppliance interface and shows how to declare properties, methods, and events by using the Microsoft::WRL namespace. For more info, see
<a href="http://msdn.microsoft.com/library/windows/apps/br224617">Windows Runtime C&#43;&#43; reference</a>.</p>
<p>The WRLInProcessWinRTComponent_server project produces a DLL named Microsoft.SDKSamples.Kitchen.dll. The WRLInProcessWinRTComponent_server project is built into the Visual&nbsp;C&#43;&#43; component extensions (C&#43;&#43;/CX) project by including the generated header file,
 Microsoft.SDKSamples.Kitchen.h. The WRLInProcessWinRTComponent_server project and its corresponding proxy/stub project are referenced directly in the provided JavaScript and C# projects.</p>
<p>Also, the sample shows how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br224651">
<b>RoOriginateError</b></a> function to report an error and an informative string to an attached debugger.</p>
<p>You can implement an out-of-process server by using the Microsoft::WRL namespace. For more info, see
<a href="http://go.microsoft.com/fwlink/p/?linkid=258333">Creating a Windows Runtime EXE component sample (C&#43;&#43;)</a>. Also, you can implement a component by using C&#43;&#43;/CX. For more info, see
<a href="http://go.microsoft.com/fwlink/p/?linkid=258330">Creating a Windows Runtime in-process component sample (C&#43;&#43;/CX)</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=258333">Creating a Windows Runtime EXE component sample (C&#43;&#43;)</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=258330">Creating a Windows Runtime in-process component sample (C&#43;&#43;/CX)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224617">Windows Runtime C&#43;&#43; reference</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224651"><b>RoOriginateError</b></a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br224617">Windows Runtime C&#43;&#43; reference</a>
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
