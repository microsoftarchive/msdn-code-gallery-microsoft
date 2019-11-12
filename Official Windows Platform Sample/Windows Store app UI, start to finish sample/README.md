# Windows Store app UI, start to finish sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Store app
## Topics
- User Interface
- User Experience
## Updated
- 03/31/2014
## Description

<p><span style="font-family:Times New Roman; font-size:small">&nbsp;</span></p>
<p><span style="line-height:115%; font-size:small"><span style="font-family:Calibri"><span lang="EN"><br>
</span><span style="font-family:Times New Roman">&nbsp;</span><span style="line-height:115%">An instructional sample app for Windows 8.1 that implements a basic Windows UI and demonstrates most controls.
</span></span></span></p>
<p><span style="line-height:115%; font-size:small"><span style="font-family:Calibri"><span style="line-height:115%">This is the companion sample for the&nbsp;<a href="http://go.microsoft.com/fwlink/?LinkId=328038"><span style="color:#0000ff; font-family:Times New Roman">Windows
 Store app UI, start to finish (C#/C&#43;&#43;/VB)</span></a> and&nbsp;<a href="http://go.microsoft.com/fwlink/?LinkId=328039"><span style="color:#0000ff; font-family:Times New Roman">Windows Store app UI, start to finish (JavaScript)</span></a> articles.<br>
</span></span></span></p>
<p><span style="font-family:Calibri"><span style="font-size:small"><strong><span lang="EN" style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><br>
</span></strong><span lang="EN" style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Note: this sample is a gallery of Windows Store app controls. With the sample, you can browse the controls and the code for implementing each control.&nbsp;</span><span lang="EN" style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span>&nbsp;</span></span></span></span></p>
<p><span style="font-family:Calibri">--<br>
</span></p>
<p><span style="font-family:Calibri"><strong><span style="color:black; font-size:12pt">Build the sample</span></strong><span style="font-family:Times New Roman">
</span></span></p>
<ol style="list-style-type:decimal; direction:ltr">
<li style="color:black; font-size:12pt; font-style:normal; font-weight:normal"><span style="font-family:Calibri"><span style="color:black; line-height:140%; font-size:12pt">Start Visual Studio&nbsp;2013 and select
<strong>File</strong> &gt; <strong>Open</strong> &gt; <strong>Project/Solution</strong>.
</span></span></li><li style="color:black; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; font-size:12pt; font-style:normal; font-weight:normal">
<span style="font-family:Calibri"><span style="color:black; line-height:140%; font-size:12pt">Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</span></span></li><li style="font-size:12pt; font-style:normal; font-weight:normal"><span style="font-family:Calibri"><span style="color:black; line-height:140%; font-size:12pt">Press F7 or use
<strong>Build</strong> &gt; <strong>Build Solution</strong> to build the sample. </span>
<strong><span style="color:black; font-size:12pt">Run the sample</span></strong><span style="color:black; font-size:12pt">To debug the app and then run it, press F5 or use
<strong>Debug</strong> &gt; <strong>Start Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </span><span style="color:black; font-size:12pt">&nbsp;</span><strong><span lang="EN" style="font-size:12pt"><br>
</span></strong></span></li></ol>
<p style="font-size:12pt; font-style:normal; font-weight:normal"><span style="font-family:Calibri"><strong><span lang="EN" style="font-size:12pt">SEE ALSO</span></strong></span></p>
<p style="font-size:12pt; font-style:normal; font-weight:normal"><span style="font-family:Calibri"><strong><span lang="EN" style="font-size:12pt">&nbsp;</span></strong><span style="font-size:12pt"><a href="http://go.microsoft.com/fwlink/?LinkId=328038"><span style="color:#0000ff; font-family:Times New Roman">Windows
 Store app UI, start to finish (C#/C&#43;&#43;/VB)</span></a></span><span style="font-size:12pt">&nbsp;</span></span></p>
<p style="font-size:12pt; font-style:normal; font-weight:normal"><span style="font-family:Calibri"><span style="font-size:12pt"><a href="http://go.microsoft.com/fwlink/?LinkId=328039"><span style="color:#0000ff; font-family:Times New Roman">Windows Store app
 UI, start to finish (JavaScript)</span></a></span></span></p>
<p style="margin:0in 0in 10pt; line-height:115%"><span style="font-family:Calibri"><br>
</span></p>
<p style="margin:0in 0in 10pt; line-height:115%">&nbsp;</p>
<p>&nbsp;</p>
<p style="margin:0in 0in 10pt; line-height:115%">&nbsp;</p>
<p><span style="font-family:Times New Roman; font-size:small">&nbsp;</span></p>
<p><span style="font-size:small">&nbsp;</span></p>
<pre><br></pre>
<p><span style="font-family:Calibri">&nbsp;</span></p>
<pre><span style="font-family:Calibri"><br></span></pre>
<p><span style="font-family:Calibri"><span style="line-height:115%; font-size:12pt">&nbsp;</span></span></p>
<pre><span style="font-family:Times New Roman">











</span><span style="font-family:Times New Roman">
 </span><span style="font-family:Times New Roman">
  </span><span style="font-family:Times New Roman">
  </span><span style="font-family:Times New Roman">
 </span><span style="font-family:Times New Roman">
 </span><span style="font-family:Times New Roman">
  </span><span style="font-family:Times New Roman">
  </span><span style="font-family:Times New Roman">
 </span><span style="font-family:Times New Roman">
</span></pre>
<p>&nbsp;</p>
<table border="0" cellspacing="0" cellpadding="0" style="border-collapse:collapse">
<tbody>
<tr>
<td width="77" style="padding:0in 0.5pt; border:#000000; width:0.8in; background-color:transparent">
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span>
<p style="margin:5pt 6pt; text-align:center; line-height:normal"><strong><span style="color:black; font-size:12pt"><span style="font-family:Calibri">Client</span></span></strong></p>
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span></td>
<td width="256" style="padding:0in 0.5pt; border:#000000; width:192pt; background-color:transparent">
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span>
<p style="margin:5pt 6pt; line-height:normal"><span style="color:black; font-size:12pt"><span style="font-family:Calibri">Windows 8.1
</span></span></p>
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span></td>
</tr>
<tr>
<td width="77" style="padding:0in 0.5pt; border:#000000; width:0.8in; background-color:transparent">
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span>
<p style="margin:5pt 6pt; text-align:center; line-height:normal"><strong><span style="color:black; font-size:12pt"><span style="font-family:Calibri">Server</span></span></strong></p>
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span></td>
<td width="256" style="padding:0in 0.5pt; border:#000000; width:192pt; background-color:transparent">
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span>
<p style="margin:5pt 6pt; line-height:normal"><span style="color:black; font-size:12pt"><span style="font-family:Calibri">Windows Server 2012 R2
</span></span></p>
<span style="font-family:Times New Roman; font-size:small">&nbsp;</span></td>
</tr>
</tbody>
</table>
<p><span style="font-family:Calibri"><span style="line-height:115%; font-size:12pt">&nbsp;</span></span></p>
<pre><span style="font-family:Times New Roman">

</span></pre>
<pre style="margin:0in 0in 10pt; line-height:115%"><p style="margin:5pt 0in; line-height:normal; page-break-after:avoid"><strong><span style="color:black; font-size:12pt">Build the sample</span></strong></p><span style="font-family:Times New Roman">

</span><p style="margin:0in 0in 8pt 42pt; line-height:normal; text-indent:-0.25in"><span style="color:black; font-size:16pt"><span>1.<span style="font:7pt/normal &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;</span></span></span><strong><span style="color:black; font-size:12pt">Visual Studio 2013 Preview and
select File &gt; Open &gt; Project/Solution. </span></strong></p><span style="font-family:Times New Roman">

</span><p style="margin:0in 0in 8pt 42pt; line-height:normal; text-indent:-0.25in"><span style="color:black; font-size:16pt"><span>2.<span style="font:7pt/normal &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp; </span></span></span><span style="color:black; font-size:12pt"><span>&nbsp;</span>Go to the directory in which you
unzipped the sample. Go to the directory named for the sample, and double-click
the Visual Studio 2013 Solution (.sln) file. </span></p><span style="font-family:Times New Roman">

</span><p style="margin:0in 0in 8pt 42pt; line-height:normal; text-indent:-0.25in"><span style="color:black; font-size:16pt"><span>3.<span style="font:7pt/normal &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp; </span></span></span><span style="color:black; font-size:12pt">Press F7 or use <strong>Build</strong> &gt; <strong>Build Solution</strong> to build the
sample. </span></p><span style="font-family:Times New Roman">

</span><p style="margin:0in 0in 8pt; line-height:normal"><strong><span style="color:black; font-size:12pt">Run the sample</span></strong></p><span style="font-family:Times New Roman">

</span><p style="margin:5pt 0in 6pt 42pt; line-height:normal"><span style="color:black; font-size:12pt">To debug the app and then run it, press F5 or use <strong>Debug</strong> &gt; <strong>Start
Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use <strong>Debug</strong>
&gt; <strong>Start Without Debugging</strong>. </span></p><span style="font-family:Times New Roman">

</span><p style="margin:5pt 0in 5pt 0.5in; line-height:normal"><span style="font-size:12pt">&nbsp;</span></p><span style="font-family:Times New Roman">

</span><p style="margin:5pt 0in; line-height:normal"><span style="font-size:12pt">&nbsp;</span></p><span style="font-family:Times New Roman">

</span><p style="margin:5pt 0in; line-height:normal"><strong><span lang="EN" style="font-size:14pt">SEE ALSO</span></strong></p><span style="font-family:Times New Roman">

</span><p style="margin:0in 0in 8pt"><span lang="EN"><a href="http://msdn.microsoft.com/en-us/library/windows/apps/hh974580.aspx"><span style="color:#0000ff; font-family:Times New Roman">Create
your first Windows Store app using C&#43;&#43;</span></a></span></p><span style="font-family:Times New Roman">

</span><p style="margin:0in 0in 8pt"><span lang="EN"><a href="http://msdn.microsoft.com/en-us/library/windows/apps/dn263182.aspx"><span style="color:#0000ff; font-family:Times New Roman">Quickstart:
Adding push notifications for a mobile service (Windows Store apps using
C#/VB/C&#43;&#43; and XAML)</span></a></span></p><span style="font-family:Times New Roman">

</span></pre>
<p>&nbsp;</p>
<pre style="margin:0in 0in 10pt; line-height:115%"><br></pre>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<pre style="margin:0in 0in 10pt; line-height:115%"><br></pre>
<p>&nbsp;</p>
<p><span style="font-family:Times New Roman; font-size:small"><br>
</span></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<div class="mcePaste" id="_mcePaste" style="left:-10000px; top:0px; width:1px; height:1px; overflow:hidden">
<p>An instructional sample app for Windows 8.1 that implements a basic Windows UI and demonstrates most controls. This is the companion sample for the Windows Store app UI, start to finish (C#/C&#43;&#43;/VB) and Windows Store app UI, start to finish (JavaScript) articles.</p>
<p>This sample is a gallery of Windows Store app controls. With the sample, you can browse the controls and the code for implementing each control.&nbsp;</p>
<p>Build the sample<br>
1.&nbsp;Start Visual Studio 2013 and select File &gt; Open &gt; Project/Solution.
<br>
2.&nbsp;Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio 2013 Solution (.sln) file.
<br>
3.&nbsp;Press F7 or use Build &gt; Build Solution to build the sample. <br>
Run the sample<br>
To debug the app and then run it, press F5 or use Debug &gt; Start Debugging. To run the app without debugging, press Ctrl&#43;F5 or use Debug &gt; Start Without Debugging.</p>
<p>SEE ALSO<br>
Windows Store app UI, start to finish (C#/C&#43;&#43;/VB)<br>
Windows Store app UI, start to finish (JavaScript)</p>
</div>
