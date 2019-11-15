# LINQ - Generation Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Generation Operators
## Updated
- 08/12/2011
## Description

<table border="0" cellspacing="2" cellpadding="1" width="100%">
<tbody>
<tr align="left" valign="top">
<td align="left" valign="middle" style="background-color:#c0c0c0"><span style="color:#ffffff; font-size:x-large">&nbsp;Part of the 101 LINQ SAMPLES</span></td>
</tr>
</tbody>
</table>
<div class="RoundedBox">
<div class="boxheader">
<div class="BostonPostCard"></div>
</div>
<div class="boxheader"><span style="font-size:medium; background-color:#ffffff; color:#333333">Learn how to use LINQ in your applications with these code samples, covering the entire range of LINQ functionality and demonstrating LINQ with SQL, DataSets, and
 XML.</span></div>
<div class="boxcontent">
<table border="0" cellspacing="2" cellpadding="1" width="100%">
<tbody>
<tr align="left" valign="top">
<td width="50px" align="left" valign="middle"><a href="http://archive.msdn.microsoft.com/vb2008samples/Release/ProjectReleases.aspx?ReleaseId=1426"><img title="101 Samples for Visual Basic 2005" src="http://i.msdn.microsoft.com/dd183105.download_45(en-us,MSDN.10).jpg" alt="101 Samples for Visual Basic 2005" align="left"></a></td>
<td align="left" valign="middle"><span style="font-size:medium"><strong><a href="http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b/viewsamplepack">Browse all 101 LINQ Samples</a>&nbsp;</strong></span></td>
</tr>
</tbody>
</table>
<hr>
<h1 id="Introduction">Introduction</h1>
<p>This sample shows different uses of Generation Operators:</p>
<ul>
<li><a title="This sample uses Range to generate a sequence of numbers from 100 to 149 that is used to find which numbers in that range are odd and even." href="#Range">Range</a>
</li><li><a title="This sample uses Repeat to generate a sequence that contains the number 7 ten times." href="#Repeat">Repeat</a>
</li></ul>
<h1><span>Building the Sample</span></h1>
<ul>
<li>Open the Program.cs&nbsp; </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ul>
<h1>Description</h1>
<h2 id="Range">Range</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Range to generate a sequence of numbers from 100 to 149 that is used to find which numbers in that range are odd and even.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq65()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;numbers&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;n&nbsp;<span class="cs__keyword">in</span>&nbsp;Enumerable.Range(<span class="cs__number">100</span>,&nbsp;<span class="cs__number">50</span>)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="cs__keyword">new</span>&nbsp;{&nbsp;Number&nbsp;=&nbsp;n,&nbsp;OddEven&nbsp;=&nbsp;n&nbsp;%&nbsp;<span class="cs__number">2</span>&nbsp;==&nbsp;<span class="cs__number">1</span>&nbsp;?&nbsp;<span class="cs__string">&quot;odd&quot;</span>&nbsp;:&nbsp;<span class="cs__string">&quot;even&quot;</span>&nbsp;};&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;n&nbsp;<span class="cs__keyword">in</span>&nbsp;numbers)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;The&nbsp;number&nbsp;{0}&nbsp;is&nbsp;{1}.&quot;</span>,&nbsp;n.Number,&nbsp;n.OddEven);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">The number 100 is even.<br>
The number 101 is odd.<br>
The number 102 is even.<br>
The number 103 is odd.<br>
The number 104 is even.<br>
The number 105 is odd.<br>
The number 106 is even.<br>
The number 107 is odd.<br>
The number 108 is even.<br>
The number 109 is odd.<br>
The number 110 is even.<br>
The number 111 is odd.<br>
The number 112 is even.<br>
The number 113 is odd.<br>
The number 114 is even.<br>
The number 115 is odd.<br>
The number 116 is even.<br>
The number 117 is odd.<br>
The number 118 is even.<br>
The number 119 is odd.<br>
The number 120 is even.<br>
The number 121 is odd.<br>
The number 122 is even.<br>
The number 123 is odd.<br>
The number 124 is even.<br>
The number 125 is odd.<br>
The number 126 is even.<br>
The number 127 is odd.<br>
The number 128 is even.<br>
The number 129 is odd.<br>
The number 130 is even.<br>
The number 131 is odd.<br>
The number 132 is even.<br>
The number 133 is odd.<br>
The number 134 is even.<br>
The number 135 is odd.<br>
The number 136 is even.<br>
The number 137 is odd.<br>
The number 138 is even.<br>
The number 139 is odd.<br>
The number 140 is even.<br>
The number 141 is odd.<br>
The number 142 is even.<br>
The number 143 is odd.<br>
The number 144 is even.<br>
The number 145 is odd.<br>
The number 146 is even.<br>
The number 147 is odd.<br>
The number 148 is even.<br>
The number 149 is odd.</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="Repeat">Repeat</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Repeat to generate a sequence that contains the number 7 ten times.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq66()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;numbers&nbsp;=&nbsp;Enumerable.Repeat(<span class="cs__number">7</span>,&nbsp;<span class="cs__number">10</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;n&nbsp;<span class="cs__keyword">in</span>&nbsp;numbers)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">7<br>
7<br>
7<br>
7<br>
7<br>
7<br>
7<br>
7<br>
7<br>
7</p>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23950&pathId=883530212">Program.cs</a>
</li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>Some Performance Notes on Enumerable LINQ Operators - <a href="http://blogs.msdn.com/b/marcelolr/archive/2010/05/06/some-performance-notes-on-enumerable-linq-operators.aspx" target="_blank">
http://blogs.msdn.com/b/marcelolr/archive/2010/05/06/some-performance-notes-on-enumerable-linq-operators.aspx</a>
</li></ul>
</div>
</div>
