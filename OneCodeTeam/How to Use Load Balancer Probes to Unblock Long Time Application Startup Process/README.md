# How to Use Load Balancer Probes to Unblock Long Time Application Startup Process
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- Azure
- Cloud
- Web Role
- Azure Cloud Services
## Topics
- Azure
- Web Role
- Load Balancer Probes
## Updated
- 04/04/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodesampletopbanner">
</a></div>
<h1>How to Use Load Balancer Probes to Unblock Long Time Application Startup Process in Azure
<span class="SpellE">Webrole</span></h1>
<p class="MsoNormal"></p>
<h2>Introduction</h2>
<p class="MsoNormal">There is circumstance that Azure Web Role will have long run application start (<span class="SpellE">Application_Start</span> in
<span class="SpellE">global.asax.cs</span>), which will block the Web Role instance healthy status to be Ready.</p>
<p class="MsoNormal">Meanwhile, Azure Fabric will recycle/heal the non-ready status instance in a period of time, which could easily bring the instance into recycling over and over again.</p>
<p class="MsoNormal">This example is to help you to build Azure Web Role with more flexible instance startup process</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Make the instance into Ready status as soon as possible</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Business initialization process run in backend as usual</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Internet-facing Load Balancer will route the traffic until the entire startup process is completed</p>
<h2>Prerequisites</h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Azure Cloud Service (Classic) Web/Worker Role architecture and initialization process</p>
<p class="MsoNormal" style="margin-left:.25in">There are 3 steps of web role instance startup process: startup task cmdlet,
<span class="SpellE">OnStart</span> method in the WAIISHost.exe, and <span class="SpellE">
application_start</span> method in the <span class="SpellE">Global.asax.cs</span>, those steps will execute in sequence.</p>
<p class="MsoNormal" style="margin-left:.25in">Once the <span class="SpellE">
OnStart</span> method execution completed, Run method will be called, and the role instance status will be in Ready.<br>
<br>
<a href="https://blogs.msdn.microsoft.com/kwill/2011/05/05/windows-azure-role-architecture/">https://blogs.msdn.microsoft.com/kwill/2011/05/05/windows-azure-role-architecture/</a>
<br style="">
<br style="">
</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Load Balancer Probes</p>
<p class="MsoNormal" style="margin-left:.25in">There are 2 types of Load Balancers in Cloud Services, Custom Load Balancer probe and Guest Agent Probe. Custom Load Balancer will override the default Guest Agent Probe. In this example, we will use Http custom
 Load Balancer as it's Web Role.<br>
<br>
<a href="https://azure.microsoft.com/en-us/documentation/articles/load-balancer-custom-probe-overview/">https://azure.microsoft.com/en-us/documentation/articles/load-balancer-custom-probe-overview/</a>
</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Custom Load Balancer Probe Schema</p>
<p class="MsoNormal" style="margin-left:.25in">Introducing how we can configure the custom Load Balancer Probe<br>
<br>
<a href="https://msdn.microsoft.com/en-us/library/azure/jj151530.aspx">https://msdn.microsoft.com/en-us/library/azure/jj151530.aspx</a>
</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>ASP.NET MVC and Web API</p>
<p class="MsoNormal" style="margin-left:.25in">This sample role project is based on ASP.NET MVC and using Web API as probe checking URL.</p>
<p class="MsoNormal" style="margin-left:.25in">Web API can more easily control the Http request return code.</p>
<p class="MsoNormal" style="margin-left:.25in"><a href="http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api">http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api</a>
</p>
<p class="MsoNormal"></p>
<h2>Building the sample</h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create Cloud Service with ASP.NET MVC/Web API role in Visual Studio 2015<br style="">
<br style="">
</p>
<p class="MsoNormal" style="margin-left:.25in">The solution contains 2 projects: Cloud Service project and Web Role project. When creating the web role project, check the MVC &amp; Web API checkbox.</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Add Endpoints in Role configuration</p>
<p class="MsoNormal" style="margin-left:.25in"><span style=""><img src="150453-image.png" alt="" width="624" height="157" align="middle">
</span><br style="">
<br style="">
</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Configure the <span class="SpellE">ServiceDefinition.csdef</span> to add the load balancer probe<br style="">
<br style="">
</p>
<p class="MsoNormal" style="margin-left:.25in">Add &lt;<span class="SpellE">LoadBalancerProbes</span>&gt; node and set the endpoint which will used for probe check.<br style="">
<br style="">
</p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&lt;?</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">xml</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">version</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">1.0</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">encoding</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">utf-8</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">?&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">ServiceDefinition</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">WebRoleWithCustomLoadBalanceProbe</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">xmlns</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceDefinition</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">schemaVersion</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">2015-04.2.6</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:yellow">LoadBalancerProbes</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:yellow">LoadBalancerProbe</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">webPortTest</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">protocol</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">http</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">path</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">/<span class="SpellE">api</span>/values/ping</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">port</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">8080</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">intervalInSeconds</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">5</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">timeoutInSeconds</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">11</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow"><span style="">&nbsp;
</span>&lt;/</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:yellow">LoadBalancerProbes</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;
</span>&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">WebRole</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">WebRoleAPI</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">vmsize</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">Small</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Runtime</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">executionContext</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">elevated</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Sites</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Site</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">Web</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Bindings</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Binding</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">Endpoint1</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">endpointName</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">Endpoint1</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Binding</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">Endpoint2</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">endpointName</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">Endpoint2</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;/</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Bindings</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;/</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Site</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;/</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Sites</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">ConfigurationSettings</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;/</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">ConfigurationSettings</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Endpoints</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">InputEndpoint</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">Endpoint1</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">protocol</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">http</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">port</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">80</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:yellow">InputEndpoint</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">Endpoint2</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">protocol</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">http</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">port</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">8080</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:yellow">loadBalancerProbe</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">webPortTest</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:yellow">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:yellow">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;/</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Endpoints</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Imports</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Import</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">moduleName</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">RemoteAccess</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Import</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">moduleName</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">RemoteForwarder</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;/</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">Imports</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">LocalResources</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>&lt;</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">LocalStorage</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">name</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">LocalStorage1</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">cleanOnRoleRecycle</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">false</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">sizeInMB</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">500</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">
 /&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;&nbsp;&nbsp;
</span>&lt;/</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">LocalResources</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:0in; margin-left:.25in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white"><span style="">&nbsp;
</span>&lt;/</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">WebRole</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-left:.25in"><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:blue; background:white">&lt;/</span><span class="SpellE"><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:#A31515; background:white">ServiceDefinition</span></span><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:blue">
</span></p>
<p class="MsoNormal"><span style="font-size:9.5pt; line-height:107%; font-family:Consolas; color:blue"></span></p>
<h2>Run the Sample</h2>
<p class="MsoNormal">Right click the cloud service project and choose &quot;Publish&quot; button to publish it to Azure Cloud Service.</p>
<p class="MsoNormal">Enable RDP to check instance status locally on instance.</p>
<p class="MsoNormal"><b style="">NOTE</b>: please use <span class="SpellE">Nuget</span> to get all dependency libraries and packages, otherwise the project couldn't be executed.</p>
<p class="MsoNormal"></p>
<h2>Using code</h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Add Web API listener to API Controller</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>[</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">AllowAnonymous</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">]
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>[</span><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">Route</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;<span class="SpellE">api</span>/values/ping&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">)]
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>[</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">AcceptVerbs</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;Get&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">)]
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">public</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">HttpResponseMessage</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="GramE">Ping(</span>) </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>{ </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">if</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"> (</span><span class="SpellE"><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">InitializedModel</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.Instance</span></span></span><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
 !</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">=
</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">null</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">)
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>{ </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">return</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">new</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">HttpResponseMessage</span></span></span><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span></span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">HttpStatusCode</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.OK</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">);
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>} </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">else</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>{ </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span><span style="font-size:9.5pt; font-family:Consolas; color:green; background:white">// use 503 as service note ready</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:green; background:white">// you can choose your own return code according to your business requirement</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">return</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">new</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">HttpResponseMessage</span></span></span><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span></span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">HttpStatusCode</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.ServiceUnavailable</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">);
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>} </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>} </span></p>
<p class="MsoNormal" style="margin-left:.25in"></p>
<p class="MsoNormal" style="margin-left:.25in; text-indent:.25in">Use <span class="SpellE">
AllowAnonymous</span> and <span class="SpellE">AcceptVerbs</span> attribute to bypass authentication and easy coding. I used 503 return code to stands for service not ready yet, you can choose return code according to your business requirement.</p>
<p class="MsoNormal" style="margin-left:.25in; text-indent:.25in"><span class="SpellE">InitializaedModel.Instance</span> checking is for initialization process, will take about it later.</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create an individual task to run initialization process asynchronously in
<span class="SpellE">Application_Start</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">protected</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">void</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="SpellE">Application_<span class="GramE">Start</span></span><span class="GramE">(</span>)
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>{ </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">AreaRegistration</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.RegisterAllAreas</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">();
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">…</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">Task</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.Run</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(()
 =&gt; { </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:green; background:white">// initialization</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">string</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="SpellE">outPutFileName</span> = </span><span class="SpellE"><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">string</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.Format</span></span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;{1}\\initial-Log-{0}.txt&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">,
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">DateTime</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.Now.ToString</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;<span class="SpellE">yyyyMMdd-hhmmss</span>&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">),
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">RoleEnvironment</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.GetLocalResource</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;LocalStorage1&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">).<span class="SpellE">RootPath</span>);
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">using</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
 (</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">StreamWriter</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"> file =
</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">new</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">StreamWriter</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(<span class="SpellE">outPutFileName</span>))
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>{ </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:green; background:white">// output start log</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">string</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="SpellE">initStart</span> = </span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;{0}: Role initialization starts&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">;
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE"><span class="GramE">file.WriteLine</span></span>(<span class="SpellE">initStart</span>,
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">DateTime</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.Now.ToString</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;<span class="SpellE">hhmmss</span>&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">));
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>} </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:green; background:white">// Use thread sleep 300s as the working process</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE"><span class="GramE">System.Threading.<span style="color:#2B91AF">Thread</span>.Sleep</span></span>(300000);
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">InitializedModel</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.Initialization</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">();
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">using</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"> (</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">StreamWriter</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
 file = </span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">new</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">StreamWriter</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(<span class="SpellE">outPutFileName</span>))
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>{ </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:green; background:white">// output end log</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">string</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="SpellE">initEnd</span> = </span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;{0}: Role initialization completed&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">;
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span class="SpellE"><span class="GramE">file.WriteLine</span></span>(<span class="SpellE">initEnd</span>,
</span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2B91AF; background:white">DateTime</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.Now.ToString</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(</span><span style="font-size:9.5pt; font-family:Consolas; color:#A31515; background:white">&quot;<span class="SpellE">hhmmss</span>&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">));
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;
</span><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>}
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>}); </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>} </span></p>
<p class="MsoNormal"></p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>I used local resource to output temporary logs, it's recommended to use Azure Diagnostics instead.</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>I used a singleton class to check if the asynchronously initialization is completed or not. That's why I need to put the asynchronous code into
<span class="SpellE">Application_Start</span>, as <span class="SpellE">OnStart</span> method in
<span class="SpellE">WebRole.cs</span> will execute in WAIISHost.exe, and <span class="SpellE">
WebAPI</span> controller Ping action will run in W3WP.exe. You can choose your own way to check the initialization process.</p>
<p class="MsoNormal"></p>
<h2>Verification Logs</h2>
<p class="MsoNormal">In the WaAppAgent.log in C:\Logs, you can see my role instance was in ready status at 4/4/2016 2:16:10 UTC</p>
<p class="MsoNormal"><span style=""><img src="150454-image.png" alt="" width="623" height="105" align="middle">
</span></p>
<p class="MsoNormal">And the initialization process started at 2:16:48 UTC and competed at 2:21:48, it's 5mins as I made the threading sleep. As you can see the initialization process started after the role was reported as Ready.</p>
<p class="MsoNormal"><span style=""><img src="150455-image.png" alt="" width="345" height="96" align="middle">
</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers’ pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers’ frequently asked programming tasks, and allow
 developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
