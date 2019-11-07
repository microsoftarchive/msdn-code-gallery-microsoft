# ASP.NET Chart control demo (CSASPNETChart)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- Chart Controls
## Updated
- 11/21/2016
## Description

<p><a href="http://blogs.msdn.com/b/onecode"><img src="-onecodesampletopbanner1" alt=""></a></p>
<h1>ASP.NET Chart control demo (CSASPNETChart)</h1>
<h2><span>Introduction</span></h2>
<p><span style="font-size:small">The project illustrates how to use the new Chart control to create an chart&nbsp;in the web page.</span></p>
<h2><span><span>Sample prerequisites</span></span></h2>
<ul>
<li><span style="font-size:small">Microsoft Visual Studio 2010 or later version(s).</span>
</li></ul>
<h2>Using the code</h2>
<p><span style="font-size:small"><strong>Step1.</strong> Create a C# ASP.NET Web Application in Visual Studio 2010 RC /Visual Web Developer 2010 and name it as CSASPNETChart.</span></p>
<p><span style="font-size:small"><strong>Step2.</strong> Delete the following default folders and files created automatically&nbsp;by Visual Studio.</span></p>
<ol>
<li><span style="font-size:small">Account folder</span> </li><li><span style="font-size:small">Script folder</span> </li><li><span style="font-size:small">Style folder</span> </li><li><span style="font-size:small">About.aspx</span> </li><li><span style="font-size:small">fileDefault.aspx</span> </li><li><span style="font-size:small">fileGlobal.asax</span> </li><li><span style="font-size:small">fileSite.Master file</span> </li></ol>
<p><span style="font-size:small"><strong>Step3.</strong> Add a new web form page to the website and name it as Default.aspx.</span></p>
<p><span style="font-size:small"><strong>Step4.</strong> Add a Chart control into the page. You can find it in the Data&nbsp;category of the Toolbox.</span></p>
<p><span style="font-size:small">[NOTE] When a Chart control is added into the page, such a Register Info willbe added to the same page automatically.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>
<pre class="hidden">&lt;%@ Register Assembly=&quot;System.Web.DataVisualization, Version=4.0.0.0, 
   Culture=neutral, PublicKeyToken=31bf3856ad364e35&quot;
   Namespace=&quot;System.Web.UI.DataVisualization.Charting&quot; TagPrefix=&quot;asp&quot; %&gt;</pre>
<div class="preview">
<pre class="html"><span class="html__tag_start">&lt;%@&nbsp;Register</span>&nbsp;<span class="html__attr_name">Assembly</span>=<span class="html__attr_value">&quot;System.Web.DataVisualization,&nbsp;Version=4.0.0.0,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;Culture=neutral,&nbsp;PublicKeyToken=31bf3856ad364e35&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="html__attr_name">Namespace</span>=<span class="html__attr_value">&quot;System.Web.UI.DataVisualization.Charting&quot;</span>&nbsp;<span class="html__attr_name">TagPrefix</span>=<span class="html__attr_value">&quot;asp&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:small">Also, a new reference /System.Web.DataVisualization/ will be added to the webapplication as well.</span></div>
<div class="endscriptcode"></div>
<p>&nbsp;</p>
<p><span style="font-size:small"><strong>Step5.</strong> Add two Series into the Chart tag as the sample below.</span></p>
<p><span style="font-size:small">&nbsp;</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>
<pre class="hidden">   &lt;Series&gt;
       &lt;asp:Series Name=&quot;Series1&quot;&gt;
       &lt;/asp:Series&gt;
       &lt;asp:Series Name=&quot;Series2&quot;&gt;
       &lt;/asp:Series&gt;
   &lt;/Series&gt;</pre>
<div class="preview">
<pre class="html">&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;Series</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;asp</span>:Series&nbsp;<span class="html__attr_name">Name</span>=<span class="html__attr_value">&quot;Series1&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/asp:Series&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;asp</span>:Series&nbsp;<span class="html__attr_name">Name</span>=<span class="html__attr_value">&quot;Series2&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/asp:Series&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/Series&gt;</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">[NOTE] The Series collection property stores Series objects, which are used to&nbsp;store data that is to be displayed, along with attributes of that data.</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><strong>Step6.</strong> Edit the two Series to add ChartType property which equals to Column andChartArea property with the value as ChartArea1.</div>
<div class="endscriptcode"></div>
<div class="endscriptcode">[NOTE] The Series ChartType value that indicates the chart type that will be&nbsp;used to represent the series. For all items in this collectin, please refer&nbsp;to this link: http://msdn.microsoft.com/en-us/library/system.web.ui.datavisualization.charting.seriescharttype(VS.100).aspxThe
 ChartAreas collection property stores ChartArea objects, which are primarily&nbsp;used to draw one or more charts using one set of axes. You will finally find the&nbsp;HTML code looks like this.</div>
<div class="endscriptcode"></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>
<pre class="hidden">&lt;asp:Chart ID=&quot;Chart1&quot; runat=&quot;server&quot; Height=&quot;400px&quot; Width=&quot;500px&quot;&gt;
   &lt;Series&gt;
       &lt;asp:Series Name=&quot;Series1&quot; ChartType=&quot;Column&quot; ChartArea=&quot;ChartArea1&quot;&gt;
       &lt;/asp:Series&gt;
       &lt;asp:Series Name=&quot;Series2&quot; ChartType=&quot;Column&quot; ChartArea=&quot;ChartArea1&quot;&gt;
       &lt;/asp:Series&gt;
   &lt;/Series&gt;
   &lt;ChartAreas&gt;
       &lt;asp:ChartArea Name=&quot;ChartArea1&quot;&gt;
       &lt;/asp:ChartArea&gt;
   &lt;/ChartAreas&gt;
&lt;/asp:Chart&gt;</pre>
<div class="preview">
<pre class="html"><span class="html__tag_start">&lt;asp</span>:Chart&nbsp;<span class="html__attr_name">ID</span>=<span class="html__attr_value">&quot;Chart1&quot;</span>&nbsp;<span class="html__attr_name">runat</span>=<span class="html__attr_value">&quot;server&quot;</span>&nbsp;<span class="html__attr_name">Height</span>=<span class="html__attr_value">&quot;400px&quot;</span>&nbsp;<span class="html__attr_name">Width</span>=<span class="html__attr_value">&quot;500px&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;Series</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;asp</span>:Series&nbsp;<span class="html__attr_name">Name</span>=<span class="html__attr_value">&quot;Series1&quot;</span>&nbsp;<span class="html__attr_name">ChartType</span>=<span class="html__attr_value">&quot;Column&quot;</span>&nbsp;<span class="html__attr_name">ChartArea</span>=<span class="html__attr_value">&quot;ChartArea1&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/asp:Series&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;asp</span>:Series&nbsp;<span class="html__attr_name">Name</span>=<span class="html__attr_value">&quot;Series2&quot;</span>&nbsp;<span class="html__attr_name">ChartType</span>=<span class="html__attr_value">&quot;Column&quot;</span>&nbsp;<span class="html__attr_name">ChartArea</span>=<span class="html__attr_value">&quot;ChartArea1&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/asp:Series&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/Series&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;ChartAreas</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;asp</span>:ChartArea&nbsp;<span class="html__attr_name">Name</span>=<span class="html__attr_value">&quot;ChartArea1&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/asp:ChartArea&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/ChartAreas&gt;</span>&nbsp;
<span class="html__tag_end">&lt;/asp:Chart&gt;</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><strong>Step7.</strong> Create data source for the Chart control via DataTable in the behindcode. In this step, please directly follow the method CreateDataTable in&nbsp;Default.aspx.cs, as this is not what we are talking about
 in this project.</div>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><strong>Step8.</strong> Bind the data source to the Chart control.</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">   Chart1.Series[0].YValueMembers = &quot;Volume1&quot;;
   Chart1.Series[1].YValueMembers = &quot;Volume2&quot;;
   Chart1.Series[0].XValueMember = &quot;Date&quot;;</pre>
<div class="preview">
<pre class="csharp">&nbsp;&nbsp;&nbsp;Chart1.Series[<span class="cs__number">0</span>].YValueMembers&nbsp;=&nbsp;<span class="cs__string">&quot;Volume1&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;Chart1.Series[<span class="cs__number">1</span>].YValueMembers&nbsp;=&nbsp;<span class="cs__string">&quot;Volume2&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;Chart1.Series[<span class="cs__number">0</span>].XValueMember&nbsp;=&nbsp;<span class="cs__string">&quot;Date&quot;</span>;</pre>
</div>
</div>
</div>
<div class="endscriptcode">[NOTE] Series.YValueMembers property is used to get or set member columns of&nbsp;the chart data source used to bind data to the Y-values of the series. Alike,Series.XValueMember property is for getting or setting the member column
 of&nbsp;the chart data source used to data bind to the X-value of the series.</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><strong>Step9:</strong> Now, you can run the page to see the achievement we did before :-)</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"></div>
</div>
<h2><span>More information</span></h2>
<p><span>MSDN: Chart Class</span></p>
<p><span><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.datavisualization.charting.chart(VS.100).aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.datavisualization.charting.chart(VS.100).aspx</a></span></p>
<p><span>MSDN: Chart Controls Tutorial</span></p>
<p><span><a href="http://msdn.microsoft.com/en-us/library/dd489231(VS.100).aspx">http://msdn.microsoft.com/en-us/library/dd489231(VS.100).aspx</a></span></p>
<p><span>ASP.NET: Chart Control</span></p>
<p><a href="http://www.asp.net/learn/aspnet-4-quick-hit-videos/video-8770.aspx">http://www.asp.net/learn/aspnet-4-quick-hit-videos/video-8770.aspx</a> (Quick Hit Videl)</p>
