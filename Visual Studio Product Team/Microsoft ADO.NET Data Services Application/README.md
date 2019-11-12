# Microsoft ADO.NET Data Services Application
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- ADO.NET Entity Framework
- ADO.NET
- WPF
- WCF Data Services
- OData
## Topics
- Data Access
## Updated
- 03/16/2012
## Description

<h1>Introduction</h1>
<p>This sample shows a simple ADO.NET Data Service exposing data through the ADO.NET Entity Framework and a WPF application consuming the ADO.NET Data Service.</p>
<h1 class="heading"><span>Requirements</span></h1>
<div class="section" id="requirementsTitleSection">
<p>This sample requires the Adventure Works sample database. For more information, see
<a href="http://msdn.microsoft.com/en-us/library/5ey0sd99%28VS.80%29.aspx" target="_blank">
How to: Install and Troubleshoot Database Components for Samples</a>.</p>
</div>
<h1><span>Building the Sample</span></h1>
<p>Press F5 (Note: you may need to set the UserInterface project to run by default)<em><br>
</em></p>
<h1>Description</h1>
<p>The solution is made up of the following projects:</p>
<h4>DataServicesWebApp</h4>
<p>This project contains the Entity Data Model that we will use as the basis of our ADO.NET Data Service, as well as the service itself.</p>
<h4>UserInterface</h4>
<p>This project is a user interface implemented in WPF. The entry point for the application is in the code behind file for ProductList.xaml.</p>
<h1>Screenshots</h1>
<p><img src="22534-screenshot.png" alt="" width="640" height="480"></p>
<p><img src="22535-screenshot2.png" alt="" width="330" height="352"></p>
<h1>Sample Code</h1>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>


<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Namespace</span>&nbsp;DataServicesWebApp&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Class</span>&nbsp;AdventureWorks&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Inherits</span>&nbsp;DataService(<span class="visualBasic__keyword">Of</span>&nbsp;AdventureWorksLTEntities)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;summary&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;This&nbsp;method&nbsp;is&nbsp;called&nbsp;only&nbsp;once&nbsp;to&nbsp;initialize&nbsp;service-wide&nbsp;policies.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;/summary&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Shared</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;InitializeService(<span class="visualBasic__keyword">ByVal</span>&nbsp;config&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;IDataServiceConfiguration)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;TODO:&nbsp;set&nbsp;rules&nbsp;to&nbsp;indicate&nbsp;which&nbsp;entity&nbsp;sets&nbsp;and&nbsp;service&nbsp;operations&nbsp;are&nbsp;visible,&nbsp;updatable,&nbsp;etc.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Examples:&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;config.SetEntitySetAccessRule(&quot;MyEntityset&quot;,&nbsp;EntitySetRights.AllRead);&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;config.SetServiceOperationAccessRule(&quot;MyServiceOperation&quot;,&nbsp;ServiceOperationRights.All);&nbsp;</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;For&nbsp;testing&nbsp;purposes&nbsp;use&nbsp;&quot;*&quot;&nbsp;to&nbsp;indicate&nbsp;all&nbsp;entity&nbsp;sets/service&nbsp;operations.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;&quot;*&quot;&nbsp;should&nbsp;NOT&nbsp;be&nbsp;used&nbsp;in&nbsp;production&nbsp;systems.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;This&nbsp;Sample&nbsp;only&nbsp;exposes&nbsp;the&nbsp;entity&nbsp;sets&nbsp;needed&nbsp;by&nbsp;the&nbsp;application&nbsp;we&nbsp;are&nbsp;building.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;This&nbsp;Sample&nbsp;uses&nbsp;EntitySetRight.All&nbsp;which&nbsp;allows&nbsp;both&nbsp;Read&nbsp;and&nbsp;Write&nbsp;access&nbsp;to&nbsp;the&nbsp;Entity&nbsp;Set.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;config.SetEntitySetAccessRule(<span class="visualBasic__string">&quot;Products&quot;</span>,&nbsp;EntitySetRights.All)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;config.SetEntitySetAccessRule(<span class="visualBasic__string">&quot;ProductCategories&quot;</span>,&nbsp;EntitySetRights.All)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;config.SetEntitySetAccessRule(<span class="visualBasic__string">&quot;ProductDescriptions&quot;</span>,&nbsp;EntitySetRights.All)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;config.SetEntitySetAccessRule(<span class="visualBasic__string">&quot;ProductModelProductDescriptions&quot;</span>,&nbsp;EntitySetRights.All)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;config.SetEntitySetAccessRule(<span class="visualBasic__string">&quot;ProductModels&quot;</span>,&nbsp;EntitySetRights.All)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Class</span>&nbsp;&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Namespace</span>&nbsp;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;
<div class="endscriptcode">&nbsp;</div>
</div>
<h1><span>Source Code Files</span></h1>
<ul>
<li>C#
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23478&pathId=157530388">AdventureWorks.svc.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=241167038">Web.config</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=850689570">IProductGateway.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=806403199">ProductGateway.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=309477448">Reference.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=1659779515">ProductList.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=584680197">ProductList.xaml.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=191147453">ProductView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23478&pathId=729262698">ProductView.xaml.cs</a>
</li></ul>
</li><li>VB<br>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=22532&pathId=1555074728">AdventureWorks.svc.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=241167038">Web.config</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=416130164">IProductGateway.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=571450606">ProductGateway.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=873641728">Reference.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=1659779515">ProductList.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=1507011832">ProductList.xaml.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=191147453">ProductView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22532&pathId=1391966629">ProductView.xaml.vb</a>
</li></ul>
</li></ul>
<h1>More Information</h1>
<p>For more information on Microsoft ADO.NET Data Services:<a href="http://social.msdn.microsoft.com/Search/en-US?query=Microsoft ADO.NET Data Services" target="_blank">http://social.msdn.microsoft.com/Search/en-US?query=Microsoft ADO.NET Data Services</a></p>
