# SharePoint 2013: Access an external list with REST
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- REST
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- data and storage
## Updated
- 02/06/2013
## Description

<p><span style="font-size:small">This project shows how to use Visual Studio 2012 and SharePoint development tools in Visual Studio 2012 to create an app for SharePoint using Business Connectivity Services (BCS) to expose complex data from an external system.</span></p>
<p><span style="font-size:small">The main objectives for this sample are:</span></p>
<ul>
<li><span style="font-size:small">Set up and use the simulated, self-hosted OData service to provide data that the auto-generation tools in Visual Studio 2012 can use to create external content types</span>
</li><li><span style="font-size:small">Create a new app for SharePoint </span></li><li><span style="font-size:small">Create an external content type that describes data from the self-hosted OData service.</span>
</li><li><span style="font-size:small">Create an external list to display the data</span>
</li><li><span style="font-size:small">Create a custom user interface within the home.aspx page of the app.</span>
</li><li><span style="font-size:small">Modify the JavaScript to wire BCS data manipulation functions to the new user interface elements.</span>
</li><li><span style="font-size:small">Edit the style sheet to modify the look and feel of the app.</span>
</li></ul>
<p><span style="font-size:small">This sample will use the Employee data entity to display a &ldquo;Contact card&rdquo; for each employee. The code will use REST calls via the client object model to retrieve a list of employees.&nbsp; The code will then enumerate
 through the return result set and create a new html div on the home page for each employee listed.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">Completion of steps in SharePoint 2013: <a href="http://code.msdn.microsoft.com/sharepoint/SharePoint-2013-Create-ffc9af9f" target="_blank">
Create external lists based on app-scoped external content type</a></span> </li><li><span style="font-size:small">SharePoint 2013</span> </li><li><span style="font-size:small">Visual Studio 2012</span> </li><li><span style="font-size:small">Internet Information Services (IIS)</span> </li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The SampleBCSApp.zip file includes the following</span></p>
<ul>
<li><span style="font-size:small">Visual Studio project files</span> </li><li><span style="font-size:small">Local OData service (CannedDataService)</span> </li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">To run the samples included in this project, do the following:</span></p>
<ul>
<li><span style="font-size:small">Extract the SampleBCSApp.zip file to your hard drive.</span>
</li><li><span style="font-size:small">Start the simulated OData service.&nbsp; This service is hosted by a local instance of IIS.&nbsp; It simply attaches to a port in IIS and provides an OData endpoint that you will use in your app.</span>
</li><li><span style="font-size:small">Open the Visual Studio project files.</span> </li></ul>
<h1>Build the sample</h1>
<p><span style="font-size:small">Follow these steps to build the sample.</span></p>
<ul>
<li><span style="font-size:small">Press <strong>F5</strong></span>. </li></ul>
<h1>Run and test the sample</h1>
<p><span style="font-size:small">When you run the app, you will see a contact card on the home.aspx page for each entry in the data service.</span></p>
<h1>Troubleshooting</h1>
<p><span style="font-size:small">If you cannot get the &ldquo;Canned&rdquo; data service to work, make sure that all the files are in the same folder on your hard drive.</span></p>
<h1>Change log</h1>
<p><span style="font-size:small">First release.</span></p>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/11d7adb5-5388-4517-ae03-beb7be1c6981" target="_blank">External content types in SharePoint 2013</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/7a87e5bf-4428-4055-b113-7665a93e7326" target="_blank">Using OData sources with Business Connectivity Services in SharePoint 2013</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" target="_blank">How to: Set up an on-premises development environment for apps for SharePoint</a></span>
</li><li><span style="font-size:small"><a href="http://www.odata.org" target="_blank">Open Data Protocol</a></span>
</li></ul>
