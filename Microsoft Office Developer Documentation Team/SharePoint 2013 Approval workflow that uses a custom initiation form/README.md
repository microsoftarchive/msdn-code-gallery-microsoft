# SharePoint 2013: Approval workflow that uses a custom initiation form
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

<div id="header"><span class="label">Summary:</span> Demonstrates how to use a flowchart-style workflow to route a workflow to different people depending on the pathway the workflow takes. It also shows how to copy items from one list to another and how
 to use custom initiation forms.</div>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<div class="section" id="sectionSection0">
<p><span class="label">Provided by: </span><a href="http://social.msdn.microsoft.com/profile/andrew%20connell%20%5bmvp%5d/" target="_blank">Andrew Connell</a>,
<a href="http://www.andrewconnell.com" target="_blank">www.AndrewConnell.com</a></p>
<p>This basic workflow scenario supports a document review and approval/reject process. The workflow starts when a document is added to a specified &quot;Drafts&quot; SharePoint library. The workflow assigns a task to a reviewer, who evaluates the draft, makes comments
 as necessary, and then concludes the task by either routing the document back to the writer for revisions, or forwarding it to the editor for editing and release. Additional tasks are assigned, depending on the branch. If the document was returned to the writer
 for revisions, the writer completes the revision task, and the workflow loops back to the reviewer. When the document is forwarded to the editor, the task completion adds the finished doc to another document library named &quot;manuscripts.&quot;</p>
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
<p>ApprovalWorkflowApp.sln contains the entire SharePoint-hosted app and workflow.</p>
</li><li>
<p>The Libraries folder in the Visual Studio solution contains libraries used by the workflow.</p>
</li><li>
<p>The Workflow Dependencies folder in the Visual Studio solution contains a custom task outcome columns, content types, and the workflow lists used by the workflow.</p>
</li><li>
<p>The Manuscript Approval workflow contains the workflow itself.</p>
</li><li>
<p>The Pages folder contains the custom initiation form.</p>
</li></ul>
</div>
<h1 class="heading">Configure and build the sample</h1>
<div class="section" id="sectionSection3">
<p>Follow these steps to configure the sample.</p>
<ol>
<li>
<p>Make sure that you have a site collection created using the Developer Site template.</p>
</li><li>
<p>Open the solution Visual Studio 2012.</p>
</li><li>
<p>Change the <span><span class="keyword">Site URL</span></span> property on the project to be the URL of the developer site.</p>
</li><li>
<p>Make sure that you have three people in Active Directory (for instance, Cliff Schoonover, Ross Mulkey, and Jess Liddell).</p>
</li><li>
<p>Run a full synchronization of the User Profile Application.</p>
</li><li>
<p>Grant all three fictitious users access to the developer site.</p>
</li><li>
<p>Build <span class="ui">ApprovalWorkflowApp.sln</span>.</p>
</li></ol>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection4">
<p>&nbsp;</p>
<div class="subSection">
<ol>
<li>
<p>Open <span class="ui">ApprovalWorkflowApp.sln</span> and press <span class="ui">
F5</span> (ensuring it has been configured to point to the developer site).</p>
</li><li>
<p>When the app deploys, it opens the home page that contains links to the <span class="ui">
Drafts</span> library.</p>
</li><li>
<p>Click the <span class="ui">Drafts</span> link and upload a document.</p>
</li><li>
<p>Navigate to the document's detail page and start the Manuscript Approval workflow.</p>
</li><li>
<p>The initiation form loads. When it loads, specify two different users for the reviewer and editor, and click the
<span class="ui">Start</span> button.</p>
</li><li>
<p>Navigate to the workflow status page for this item. You should see a new task created for the reviewer.</p>
</li><li>
<p>Open the task and click <span class="ui">Proceed to Editor</span>.</p>
</li><li>
<p>Navigate to the workflow status page for this item. You should see a new task created for the editor.</p>
</li><li>
<p>Open the task and click <span class="ui">Approve</span>.</p>
<p>After you approve the suggestion, the document is copied to the Manuscripts folder.</p>
</li><li>
<p>Rerun the workflow, but this time, when the reviewer completes the task, click
<span class="ui">Send Back to Author</span>. This routes the document back to the author for further work.</p>
</li></ol>
</div>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection5">
<p>If the workflow fails, it is likely due to one of two things: either the users have not been imported into the User Profile Application, or the environment has not been configured to send email messages. The latter condition reports errors, but it does not
 keep the workflow from running.</p>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection6">
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
<p>Revised</p>
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
<div class="section" id="sectionSection7">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp179923.aspx" target="_blank">How to: Set up an on-premises development environment for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj163276.aspx" target="_blank">Start: Set up and configure SharePoint 2013 Workflow Manager</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp179930.aspx" target="_blank">Apps for SharePoint overview</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp142379.aspx" target="_blank">How to: Create a basic SharePoint-hosted app</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj163986.aspx" target="_blank">Workflows in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj163276.aspx" target="_blank">Start: Set up and configure SharePoint 2013 Workflow Manager</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/windowsazure/jj193528(v=azure.10).aspx" target="_blank">Workflow Manager 1.0</a></p>
</li></ul>
</div>
</div>
</div>
</div>
