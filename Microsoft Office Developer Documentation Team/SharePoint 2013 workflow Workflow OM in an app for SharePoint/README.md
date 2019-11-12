# SharePoint 2013 workflow: Workflow OM in an app for SharePoint
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- SharePoint Server 2013
- apps for SharePoint
## Topics
- Workflows
## Updated
- 03/08/2013
## Description

<p><span style="font-size:small">The new SharePoint 2013 workflow object model supports advanced scenarios to harness the power of the new SharePoint workflow platform. This sample illustrates ways in which you can use the SharePoint object model to enable
 these advanced scenarios.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">SharePoint 2013</span> </li><li><span style="font-size:small"><span style="font-size:small">&#65279;Workflow Manager Client 1.0</span></span>
</li><li><span style="font-size:small">Visual Studio 2012, either the Ultimate or Professional version</span>
</li><li><span style="font-size:small">SharePoint development tools in Visual Studio 2012</span>
</li></ul>
<h1>Using the workflow object model</h1>
<p><span style="font-size:small">The new SharePoint 2013 workflow object model supports advanced scenarios to harness the power of the new SharePoint workflow platform.&nbsp;</span></p>
<p><span style="font-size:small">The object model allows you to deploy workflows, to manage workflow instances, and supports messaging to workflow instances.&nbsp;</span></p>
<p><span style="font-size:small">The SharePoint 2013 workflow object model is available in various forms. There is a SharePoint Server object model, which is a managed API, as well as client object model (or CSOM); there is a JavaScript object model (JSOM),
 and a SharePoint REST API.</span></p>
<p><span style="font-size:small">A SharePoint-hosted app for SharePoint can use the SharePoint JSOM and SharePoint REST APIs to access the Workflow object model. A self-hosted and an auto-hosted SharePoint app can also use SharePoint CSOM in addition to SharePoint
 JSOM and SharePoint REST API to access workflow object model.</span></p>
<p><span style="font-size:small">For more information about selecting the best APIs for your programming tasks, see
<a href="http://technet.microsoft.com/library/f36645da-77c5-47f1-a2ca-13d4b62b320d.aspx">
Choose the right API set in SharePoint 2013</a>.</span></p>
<p><span style="font-size:small">The <strong>WorkflowOMTest</strong> code sample is an example of an interactive SharePoint-hosted app that is using SharePoint workflow JSOM to deploy workflow definitions to both an app web and to a &quot;parent web&quot; (that is, a
 SharePoint web hosting the app).</span></p>
<h1>Change log</h1>
<p><span style="font-size:small">First release.&nbsp;July 16, 2012</span></p>
<h1>Related content</h1>
<ul>
<span style="font-size:small">
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/jj163917.aspx" target="_blank">Get started with workflows in SharePoint 2013</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/ffaccd6b-426d-4ca0-b62f-bc7b14641a49" target="_blank">SharePoint 2013 workflow samples</a></span>
</span></li></ul>
