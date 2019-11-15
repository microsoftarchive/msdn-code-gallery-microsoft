# Office 365: Create a task in Exchange Online
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Exchange Online
- Office 365
- Sharepoint Online
## Topics
- Sharepoint Online
- Exchange Web Services (EWS) Managed API
## Updated
- 03/07/2014
## Description

<p id="header"><span class="label">Summary:</span> The solution in this sample demonstrates how to use Exchange Web Services (EWS) Managed API to create a task in Office 365 Exchange Online, showing how other Exchange 2013 objects may be managed.</p>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<div>&nbsp;</div>
</div>
<h1 class="heading">Description of the sample</h1>
<div class="section" id="sectionSection0">
<p>The purpose of this sample is to demonstrate how an ASP.NET Model-View-Controller (MVC) application uses Exchange Web Services (EWS) Managed API to create a task in Office 365 Exchange Online. The sample uses the email ID and password from the login to authenticate
 the credentials; the task title and message entered are used to call the web service to create the task. When the task is created, a list of the current tasks entered for the given credentials is displayed.</p>
</div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<div>This sample requires the following:</div>
<ul>
<li>
<div>An Office 365 Developer Site.</div>
</li><li>
<div>An Exchange Web Services (EWS) Managed API; see <a href="http://www.microsoft.com/en-us/download/details.aspx?id=35371" target="_blank">
Microsoft Exchange Web Services Managed API 2.0</a>.</div>
</li><li>
<div>Visual Studio 2012 installed on your computer.</div>
</li><li>
<div>Office Developer Tools for Visual Studio 2012 installed on your computer.</div>
</li></ul>
</div>
<h1 class="heading">Key components of the sample</h1>
<div class="section" id="sectionSection2">
<div>The sample zip file contains the following:</div>
<ul>
<li>
<div>A Visual Studio 2012 solution named O365_managingtasksusingEWS_cs that contains the code for the ASP.NET application.</div>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<ol>
<li>
<div>Open Visual Studio 2012 with administrator privileges.</div>
</li><li>
<div>On the <span class="ui">File</span> menu, click <span class="ui">Open</span>, and then click
<span class="ui">Project/Solution</span>.</div>
</li><li>
<div>Navigate to the folder where you unzipped the O365_managingtasksusingEWS_cs.sln file, select the file, and click
<span class="ui">Open</span>.</div>
</li></ol>
</div>
<h1 class="heading">Build the sample</h1>
<div class="section" id="sectionSection4">
<p>Press F6 to build the sample.</p>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection5">
<ol>
<li>
<div>Click F5 to run the application. A login dialog box opens; make sure that the browser version of the application is not earlier than Internet Explorer 8.</div>
</li><li>
<div>Enter the following data:</div>
<ol>
<li>
<div>For <span class="ui">Task creator Email ID</span> and <span class="ui">Password</span>, enter your Office 365 email ID and password.</div>
</li><li>
<div>For <span class="ui">Task Title</span>, enter any title for your task.</div>
</li><li>
<div>For <span class="ui">Task Message</span>, enter any message text for the task.</div>
</li></ol>
</li><li>
<div>Click the <span class="ui">Create and Display Task</span> button to create a new task for the Office 365 user.</div>
</li><li>
<div>After several minutes, the complete list of tasks for the user entered is shown appended to the web page, as shown in the following figure.</div>
<div><img id="76630" src="http://i1.code.msdn.s-msft.com/sharepoint-online-set-up-a-d5207541/image/file/76630/1/o365_aspnet_manage_tasks_entry.jpg" alt="O365_ASPNet_Manage_Tasks_Entry" width="615" height="421"></div>
<div>&nbsp;</div>
</li><li>
<div>The new task can also be verified by logging in to the Office 365 site. Click the
<span class="ui">Outlook</span> section, and then click the <span class="ui">
Tasks</span> section to display all tasks, as shown in the following figure.</div>
<div><img id="110259" src="http://i1.code.msdn.s-msft.com/office/sharepoint-online-set-up-a-d5207541/image/file/110259/1/o365_aspnet_o365_task_list.jpg" alt="O365_ASPNet_O365_Task_List" width="670" height="324"></div>
</li></ol>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection6">
<p>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how you can solve them.</p>
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<div>Problem</div>
</th>
<th>
<div>Solution</div>
</th>
</tr>
<tr>
<td>
<div>0x800a1391 - JavaScript runtime error: 'JSON' is undefined</div>
</td>
<td>
<div>Make sure your browser version is set to Internet Explorer 8.</div>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection7">
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<div>Version</div>
</th>
<th>
<div>Date</div>
</th>
</tr>
<tr>
<td>
<div>First version</div>
</td>
<td>February 28, 2013</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Related content</h1>
<div class="section" id="sectionSection8">
<ul>
<li>
<div><a href="http://msdn.microsoft.com/en-us/library/exchange/dd633710(v=exchg.80).aspx" target="_blank">Explore the EWS Managed API</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/exchange/dd633696(v=exchg.80).aspx" target="_blank">Working with the EWS Managed API</a></div>
</li></ul>
</div>
</div>
</div>
