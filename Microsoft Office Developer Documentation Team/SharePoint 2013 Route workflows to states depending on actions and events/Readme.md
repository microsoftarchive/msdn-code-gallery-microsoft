# SharePoint 2013: Route workflows to states depending on actions and events
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- Workflows
## Updated
- 04/23/2013
## Description

<div id="header">Demonstrates how to use a state machine-style workflow to route a workflow to different states depending on different actions and events. It also shows how to create custom events and use the workflow client object model (CSOM).</div>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<p><span class="label">Provided by: </span><a href="http://social.msdn.microsoft.com/profile/andrew%20connell%20%5bmvp%5d/" target="_blank">Andrew Connell</a>,
<a href="http://www.andrewconnell.com" target="_blank">www.AndrewConnell.com</a></p>
</div>
<div class="section" id="sectionSection0">
<p>This sample allows a company, such as a cable company, to better manage is fleet of vehicles. A fleet manager workflow instant starts when a new vehicle is added to the fleet (as an item in a SharePoint list). Three months (simulated in the sample as three
 minutes, but easily changed) following the last maintenance cycle, the workflow instance takes the vehicle out of service and assigns a task to the maintenance department to do an oil change. When the task is completed, the vehicle is put back in service.</p>
<p>If out-of-cycle maintenance is required on a vehicle, the app allows users to enter a maintenance request. This maintenance request page sends a custom event to the workflow to put the vehicle into maintenance.</p>
</div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>SharePoint 2013 installation that is connected to a configured Workflow Manager 1.0 farm.</p>
</li><li>
<p>Workflow Manager 1.0 that has the March 2013 cumulative update applied.</p>
</li><li>
<p>Service Bus 1.0, with the Mach 2013 cumulative update applied.</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012</p>
</li></ul>
</div>
<h1 class="heading">Key components of the sample</h1>
<div class="section" id="sectionSection2">
<ul>
<li>
<p><strong>FleetManagementApp.sln</strong> is the Visual Studio solution that contains the entire SharePoint-hosted app and workflow.</p>
</li><li>
<p>The <strong>Fleet List</strong> folder in the Visual Studio solution contains a list used by the workflow.</p>
</li><li>
<p>The <strong>Workflow Dependencies</strong> folder in the Visual Studio solution contains a custom task outcome column, content type, and the workflow lists used by the workflow.</p>
</li><li>
<p>The <strong>Fleet Management Workflow</strong> contains the workflow and the custom initiation form.</p>
</li><li>
<p>The <strong>Pages</strong> folder contains MaintenanceRequest.aspx, which serves as the trigger for the custom event.</p>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<p>Follow these steps to configure the sample app:</p>
<ol>
<li>
<p>Ensure that you have a site collection created using the <span class="ui">Developer Site</span> template.</p>
</li><li>
<p>Open the solution using Visual Studio 2012.</p>
</li><li>
<p>Change the <span><span class="keyword">Site URL</span></span> property on the project to be the URL of the Developer Site.</p>
</li><li>
<p>Ensure you have at least one person in Active Directory.</p>
</li><li>
<p>Run a full synchronization of the User Profile Application.</p>
</li><li>
<p>Grant the users access to the Developer Site.</p>
</li></ol>
</div>
<h1 class="heading">Build the sample</h1>
<div class="section" id="sectionSection4">
<p>Build <span class="ui">FleetManagementApp.sln</span>.</p>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection5">
<ol>
<li>
<p>Open the <span class="ui">FleetManagementApp.sln</span> and press F5 (ensuring it has been configured to point to the Developer Site).</p>
</li><li>
<p>When the app deploys, it opens the home page that contains links to the Fleet Management List.</p>
</li><li>
<p>Click the <span class="ui">Fleet Management List</span> link and create a new vehicle. Only two fields are required, but leave the
<span class="ui">Service Status</span> field to <span class="ui">In Service</span>. This field will change when the vehicle is taken out of service for maintenance.</p>
</li><li>
<p>Navigate to the document's detail page, and start the Manuscript Approval Workflow.</p>
<p>After you submit the item, the workflow starts automatically.</p>
</li><li>
<p>Watch the workflow debug console window, as it reports the current transition and state the workflow is in. After three minutes, it will show the workflow has transitioned into a different state.</p>
</li><li>
<p>Refresh the item in the browser to see that the status has changed to <span class="ui">
Out of Service</span>.</p>
</li><li>
<p>Go to the workflow status page and notice the status of the workflow. When you open the associated task, you see that it was created for scheduled service.</p>
</li><li>
<p>Edit the task and click <span class="ui">Complete</span>. Notice in the workflow debug console window that the transition and state changed.</p>
</li><li>
<p>Refresh the list item and notice that the <span class="ui">Service Status</span> field has been updated.</p>
</li><li>
<p>Before another three minutes pass, go to the <span class="ui">All Items</span> view of the Fleet Management List and use the
<span class="ui">Item</span> menu (also known as Edit Control Block) to select the
<span class="ui">Manual Service Request</span> custom action.</p>
</li><li>
<p>Enter a reason for the service, and click the <span class="ui">Submit Maintenance Request</span> button.</p>
</li><li>
<p>Go back to the workflow status page to see that a new maintenance request has been submitted that also includes the request.</p>
</li></ol>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection6">
<p>If the workflow fails, it is likely due to one of two things: either the users have not been imported into the User Profile Application, or the environment has not been configured to send email messages. The latter reason reports errors, but it does not
 keep the workflow from running.</p>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection7">
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<td>
<p>First release</p>
</td>
<td>
<p>January 2013</p>
</td>
</tr>
<tr>
<td>
<p>Revised version</p>
</td>
<td>
<p>April 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Related content</h1>
<div class="section" id="sectionSection8">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp179930.aspx" target="_blank">Apps for SharePoint overview</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj163794.aspx" target="_blank">Develop apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp142379.aspx" target="_blank">How to: Create a basic SharePoint-hosted app</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj163986.aspx" target="_blank">Workflows in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj163276.aspx" target="_blank">Start: Set up and configure SharePoint 2013 Workflow Manager</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/windowsazure/jj193528(v=azure.10).aspx" target="_blank">Workflow Manager 1.0</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/ee264171(v=VS.110).aspx" target="_blank">State Machine Workflows</a></p>
</li></ul>
</div>
</div>
</div>
