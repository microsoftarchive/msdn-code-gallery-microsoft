# XAML Navigation sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- Windows Runtime
## Topics
- Controls
- User Interface
## Updated
- 12/12/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to implement basic page navigation in a Windows Store app using C&#43;&#43;, C#, or Visual Basic.
</p>
<p>The sample demonstrates these tasks:</p>
<ol>
<li>
<p><b>Basic navigation</b></p>
<p>This scenario shows how to create multiple pages for your Windows Store app to support user navigation between the pages within the app, similar to how you navigate through pages on a website.</p>
</li><li>
<p><b>Pass info between pages</b></p>
<p>This scenario shows how to share information between pages. The <a href="http://msdn.microsoft.com/library/windows/apps/br242694">
<b>Navigate</b></a> method has an argument to pass a parameter to the target page. The
<a href="http://msdn.microsoft.com/library/windows/apps/br227508"><b>OnNavigatedTo</b></a> method of the target page uses the
<a href="http://msdn.microsoft.com/library/windows/apps/br243289"><b>NavigationEventArgs.Parameter</b></a> property to capture this parameter from the source.</p>
</li><li>
<p><b>Cancel navigation</b></p>
<p>This scenario shows how to cancel navigation by using the Page.<a href="http://msdn.microsoft.com/library/windows/apps/br227509"><b>OnNavigatingFrom</b></a> method and the
<a href="http://msdn.microsoft.com/library/windows/apps/br243278"><b>NavigatingCancelEventArgs</b></a> parameter to cancel a navigation request.</p>
</li></ol>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
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
