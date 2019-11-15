# Add query string to ASP.NET breadcrumb (CSASPNETBreadcrumbWithQueryString)
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- Sitemap
- Breadcrumbs
## Updated
- 12/03/2012
## Description

<h1>Add query string to ASP.NET breadcrumb (CSASPNETBreadcrumbWithQueryString)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">By default, SiteMapPath control is very static. It only shows the nodes whose location can be matched in the site map. Sometimes we want to change SiteMapPath control's titles and paths according to Query String values. And sometimes
 we want to create the SiteMapPath dynamically. This code sample shows how to achieve these goals by handling SiteMap.SiteMapResolve event<span style="">.
</span></p>
<h2>Running the Sample<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.25in"><span style=""><span style="">Step 1:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Open the CSASPNETBreadcrumbWithQueryString.sln.<span style="">
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 2:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Expand the CSASPNETBreadcrumbWithQueryString web application and press Ctrl &#43; F5 to show the Default.aspx.<br>
<span style=""><img src="71723-image.png" alt="" width="359" height="263" align="middle">
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 3:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">Click a link in the categories list to navigate to page Category.aspx, and then click a link to navigate to page Item.aspx. You can see that the breadcrumb is showing the dynamic nodes according to Query String values.<br>
<span style=""><img src="71724-image.png" alt="" width="218" height="135" align="middle">
</span></span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 4:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">Open page DynamicBreadcrumb.aspx to see the dynamically created breadcrumb.<br>
<span style=""><img src="71725-image.png" alt="" width="429" height="103" align="middle">
</span></span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.25in"><span style=""><span style="">Step 5:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Validation finished.</p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.25in"><span style=""><span style="">Step 1:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2012 or Visual Web Developer 2012. Name it as &quot;CSASPNETBreadcrumbWithQueryString&quot;.</p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:.25in"><span style=""><span style="">Step 2:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">&nbsp;</span>Add <span style="">four</span> web forms in the root directory, name them as &quot;Default.aspx&quot;, &quot;<span style="">Category</span>.aspx&quot;, &quot;<span style="">DynamicBreadcrumb</span>.aspx&quot;, &quot;<span style="">Item</span>.aspx&quot;.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.25in"><span style=""><span style="">Step 3:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span><span style="">T</span>he point of this sample project is that we handle SiteMap.SiteMapResolve event to dynamically create/change current SiteMapNode.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
SiteMap.SiteMapResolve &#43;= new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);


&nbsp;&nbsp;&nbsp; SiteMapNode SiteMap_SiteMapResolve(object sender, SiteMapResolveEventArgs e)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Only need one execution in one request.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SiteMap.SiteMapResolve -= new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (SiteMap.CurrentNode != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // SiteMap.CurrentNode is readonly, so we need to clone one to operate.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; currentNode.Title = Request.QueryString[&quot;name&quot;];


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Use the changed one in the breadcrumb.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return currentNode;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return null;
&nbsp;&nbsp;&nbsp; }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst" style="margin-left:.25in"><span style=""><span style="">Step 4:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Add a class file and rename it as &quot;Database&quot;;<span style="font-size:9.5pt; line-height:115%; font-family:Consolas; background:white">
</span><span style="background:white">it is a very simple in-code database for demo purpose</span><span style="">.</span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:.25in"><span style=""><span style="">Step 5:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;
</span></span></span>Build the application and you can debug it<span style="">.</span></p>
<h2>More Information</h2>
<p class="MsoNormal">SiteMapPath Web Server Control Overview<br>
<a href="http://msdn.microsoft.com/en-us/library/x20z8c51.aspx">http://msdn.microsoft.com/en-us/library/x20z8c51.aspx</a><br>
SiteMap Class<br>
<a href="http://msdn.microsoft.com/en-us/library/system.web.sitemap.aspx">http://msdn.microsoft.com/en-us/library/system.web.sitemap.aspx</a><br>
SiteMap.SiteMapResolve Event<br>
<a href="http://msdn.microsoft.com/en-us/library/system.web.sitemap.sitemapresolve.aspx">http://msdn.microsoft.com/en-us/library/system.web.sitemap.sitemapresolve.aspx</a><span style="">
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
