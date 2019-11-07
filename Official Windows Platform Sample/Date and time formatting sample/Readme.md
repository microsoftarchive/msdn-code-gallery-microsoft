# Date and time formatting sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- HTML5
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Globalization
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br206828">
<b>DateTimeFormatter</b></a> class in the <a href="http://msdn.microsoft.com/library/windows/apps/br206859">
<b>Windows.Globalization.DateTimeFormatting</b></a> namespace to display dates and times according to the user's preferences.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/br206828"><b>DateTimeFormatter</b></a> API is in the
<a href="http://msdn.microsoft.com/library/windows/apps/br206859"><b>Windows.Globalization.DateTimeFormatting</b></a> namespace. It provides a globally-aware method for formatting a date or time into a string for display to a user. The API can either use the
 default preferences of the current user, or the caller can override these to specify other languages, geographic region, and clock and calendar systems. The caller can request a format using the well-known constants (shorttime, longtime, shortdate or longdate)
 or define the specific elements required.</p>
<p>The sample also uses the <a href="http://msdn.microsoft.com/library/windows/apps/br229460">
<b>Windows.Globalization.CalendarIdentifiers</b></a> and <a href="http://msdn.microsoft.com/library/windows/apps/br229462">
<b>Windows.Globalization.ClockIdentifiers</b></a> classes.</p>
<p></p>
<p>This sample contains scenarios that demonstrate:</p>
<ul>
<li>How to format the current date and time using the Long and Short formats. </li><li>How to format the current date and time using custom formats that are specified using a template string or a parameterized template.
</li><li>How to format dates and times by overriding the user's default global context. This is used when an app provides an experience that presents dates or times that reflect different settings from the user's current defaults.
</li><li>How to format dates and times by using Unicode extensions in specified languages, overriding the user's default global context if applicable.
</li><li>How to convert and format the current date and time using the timezone support available in the
<a href="http://msdn.microsoft.com/library/windows/apps/dn264145"><b>Format</b></a> method.
</li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229460"><b>Windows.Globalization.CalendarIdentifiers</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229462"><b>Windows.Globalization.ClockIdentifiers</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br206828"><b>Windows.Globalization.DateTimeFormatting.DateTimeFormatter</b></a>
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
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>DateTimeFormattingSample.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build DateTimeFormattingSample.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>DateTimeFormattingSample.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build DateTimeFormattingSample.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>DateTimeFormattingSample.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy DateTimeFormattingSample.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>DateTimeFormattingSample.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy DateTimeFormattingSample.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>DateTimeFormattingSample.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>DateTimeFormattingSample.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
