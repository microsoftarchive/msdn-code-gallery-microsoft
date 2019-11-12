# Json Sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- JSON
- universal app
## Updated
- 05/13/2014
## Description

<div id="mainSection">
<p>This sample shows you how to encode and decode JavaScript Object Notation (JSON) objects, arrays, strings, numbers and booleans using classes in the
<a href="http://msdn.microsoft.com/library/windows/apps/br240639"><b>Windows.Data.Json</b></a> namespace in your Windows Runtime app. This sample is provided in the C# and C&#43;&#43; programming languages. JavaScript developers should use the
<a href="http://go.microsoft.com/fwlink/?LinkId=398621">JSON.Parse</a> method to parse JSON. For more information, see the
<a href="http://go.microsoft.com/fwlink/?LinkID=398620">JSON Object (JavaScript)</a>, an intrinsic object that provides functions to convert JavaScript values to and from the JSON format.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>This sample demonstrates the following features:</p>
<p></p>
<ul>
<li>Serialize and deserialize JSON objects using the <a href="http://msdn.microsoft.com/library/windows/apps/br225267">
<b>JsonObject</b></a> class. </li><li>Serialize and deserialize JSON arrays using the <a href="http://msdn.microsoft.com/library/windows/apps/br225234">
<b>JsonArray</b></a> class. </li><li>Serialize and deserialize strings using the <a href="http://msdn.microsoft.com/library/windows/apps/br240622">
<b>JsonValue</b></a> class. </li><li>Serialize and deserialize numbers using the <a href="http://msdn.microsoft.com/library/windows/apps/br240622">
<b>JsonValue</b></a> class. </li><li>Serialize and deserialize booleans using the <a href="http://msdn.microsoft.com/library/windows/apps/br240622">
<b>JsonValue</b></a> class. </li></ul>
<p></p>
<p>The following features can be used in a Windows Runtime app to retrieve text that contains JSON from an HTTP server or send text that contains JSON to an HTTP server.</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/dn298639"><b>Windows.Web.Http.HttpClient</b></a> - Supports apps written in JavaScript, C#, Visual Basic, or C&#43;&#43;.
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh831163">XML HTTP Extended Request (IXMLHttpRequest2)</a> - Supports apps written in C&#43;&#43;.
</li><li><a href="http://msdn.microsoft.com/en-us/library/jj988008(v=vs.120).aspx">C&#43;&#43; REST SDK</a> - Supports apps written in C&#43;&#43;.
</li></ul>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Use of this sample does not require Internet or intranet access so no network capabilities need to be set in the
<i>Package.appmanifest</i> file.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Other</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761504">Connecting to web services (Windows Runtime app using C&#43;&#43;, C#, or Visual Basic)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770289">Using JavaScript Object Notation (JSON) (Windows Runtime app using C&#43;&#43;, C#, or Visual Basic)</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/en-us/library/jj988008(v=vs.120).aspx">C&#43;&#43; REST SDK</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br225234"><b>JsonArray</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br225267"><b>JsonObject</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br240622"><b>JsonValue</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br240639"><b>Windows.Data.Json</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298639"><b>Windows.Web.Http.HttpClient</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh831163">XML HTTP Extended Request (IXMLHttpRequest2)</a>
</dt><dt><b>Samples</b> </dt><dt><a href=" http://go.microsoft.com/fwlink/p/?linkid=242550">HttpClient Sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
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
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>Json.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build Json.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>Json.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build Json.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>Json.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy Json.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>Json.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy Json.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>Json.Windows</b> in <b>Solution Explorer</b> and select <b>Set as StartUp Project</b>.
</li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>Json.WindowsPhone</b> in <b>Solution Explorer</b> and select <b>
Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
