# SharePoint 2013: Employee recruitment process manager app
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- Sharepoint Online
- SharePoint Server 2013
- SharePoint Foundation 2013
## Topics
- data and storage
## Updated
- 07/31/2013
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Employee recruitment process manager app</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p>Demonstrates how to use JavaScript and jQuery in an app for SharePoint that manages job vacancies, descriptions, prerequisites, salary, locations, interviews, and job offers.</p>
</div>
<div>
<p><span>Provided by:</span> <a href="http://mvp.microsoft.com/en-us/mvp/Martin%20Harwar-4025664" target="_blank">
Martin Harwar</a>, <a href="http://point8020.com/Default.aspx" target="_blank">Point8020.com</a></p>
<p>In this app, a Human Resources (HR) administrator tracks the progress of a job vacancy, from the job description through interviews to an offer letter.</p>
<p>This solution is based on the SharePoint-hosted app template provided by Visual Studio 2012. The solution uses the JavaScript implementation of the client object model to read, create, update, and delete data from lists based on user actions. The lists included
 in this solution represent job vacancies (including data such as job titles, number of vacancies, location, role descriptions, job prerequisites, and salary) and calendared interviews (including interviewee data, interviewer data, interview location, notes,
 and outcomes).</p>
<p>The lists are related to each other through lookup fields, and the user interface (UI) ensures that all data operations synchronize with their list items so that the relationships are maintained. The UI is implemented with HTML elements and cascading style
 sheet (CSS) styles to present a modern look and feel. JavaScript and jQuery are used to control all aspects of the UI, and the solution contains no server-side code.</p>
</div>
<h1>Prerequisites</h1>
<div id="sectionSection0">
<p>This sample requires the following:</p>
<ul>
<li>
<p>Visual Studio 2012</p>
</li><li>
<p>Office Developer Tools for Visual Studio 2012</p>
</li><li>
<p>Either of the following:</p>
<ul>
<li>
<p>Access to an Office 365 Enterprise site that has been configured to host apps (recommended).</p>
</li><li>
<p>SharePoint Server 2013 configured to host apps, and with a Developer Site Collection already created.</p>
</li></ul>
</li></ul>
</div>
<h1>Key components</h1>
<div id="sectionSection1">
<p>The sample app contains the following:</p>
<ul>
<li>
<p>The <span><strong><span class="keyword">Default.aspx</span></strong></span> webpage, which is used by HR administrators to create job vacancies, print job descriptions, schedule interviews, record details of any job offers to candidates, and to print job
 offer letters.</p>
</li><li>
<p>The <span><strong><span class="keyword">App.js</span></strong></span> file in the
<span><strong><span class="keyword">scripts</span></strong></span> folder, which is used to retrieve and manage job and recruitment data by using the JavaScript implementation of the client object model (JSOM). The
<span><strong><span class="keyword">App.js</span></strong></span> file also contains the UI logic that is implemented in
<span><strong><span class="keyword">Default.aspx</span></strong></span>, including the ability to print from the app.</p>
</li><li>
<p>The <span><strong><span class="keyword">App.css</span></strong></span> file in the
<span><strong><span class="keyword">contents</span></strong></span> folder, which contains style definitions used by the elements in
<span><strong><span class="keyword">Default.aspx</span></strong></span>.</p>
</li><li>
<p>Two list definitions and instances: one for job vacancies tables, and one calendared list for interviews. The lists are linked together by lookup fields.</p>
</li><li>
<p>All other files are automatically provided by the Visual Studio 2012 project template for apps for SharePoint, and they have not been modified in the development of this sample app.</p>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection2">
<p>Follow these steps to configure the sample.</p>
<div>
<ol>
<li>
<p>Open the <span><strong><span class="keyword">SP_RecruitmentProcessManager_js.sln</span></strong></span> file in Visual Studio 2012.</p>
</li><li>
<p>In the <span><strong><span class="keyword">Properties</span></strong></span> window, add the full URL to your Office 365 Enterprise site or SharePoint Server 2013 Developer Site Collection to the
<span><strong><span class="keyword">Site URL</span></strong></span> property. You may be prompted to provide credentials if you add a URL to an Office 365 site.</p>
</li><li>
<p>No other configuration is necessary.</p>
</li></ol>
</div>
</div>
<h1>Build, run, and test the sample</h1>
<div id="sectionSection3">
<p>&nbsp;</p>
<div>
<ol>
<li>
<p>Press Ctrl&#43;Shift&#43;B to build the solution.</p>
</li><li>
<p>Press F5 to run the app.</p>
</li><li>
<p>Sign in to your SharePoint Server 2013 or Office 365 Enterprise site if you are prompted to do so by the browser.</p>
</li><li>
<p>When the app appears, the starting screen resembles Figure 1.</p>
<strong>
<div class="caption">Figure 1. Start screen</div>
</strong><br>
&nbsp;<img src="93382-image.png" alt=""> </li><li>
<p>Click <strong><span class="ui">New Job Vacancy</span></strong> to enter the details for a new job, as shown in Figure 2.</p>
<strong>
<div class="caption">Figure 2. Add New Vacancy form</div>
</strong><br>
&nbsp;<img src="93383-image.png" alt=""> </li><li>
<p>Figure 3 shows that three jobs have been created.</p>
<strong>
<div class="caption">Figure 3. List of job vacancies</div>
</strong><br>
&nbsp;<img src="93384-image.png" alt=""> </li><li>
<p>Click a vacancy name to see its details, as shown in Figure 4. You can update and save the details, and schedule an interview or print a job description document.</p>
<strong>
<div class="caption">Figure 4. Vacancy Details form</div>
</strong><br>
&nbsp;<img src="93385-image.png" alt=""> </li><li>
<p>Click <strong><span class="ui">Print Preview</span></strong> to see the job details in a printable format, as shown in Figure 5. The
<strong><span class="ui">[Print]</span></strong> link prints the document.</p>
<strong>
<div class="caption">Figure 5. Print Preview screen</div>
</strong><br>
&nbsp;<img src="93386-image.png" alt=""> </li><li>
<p>Click <strong><span class="ui">Schedule Interview</span></strong> to enter a location and choose a date and time from the jQuery date picker, as shown in Figure 6.</p>
<strong>
<div class="caption">Figure 6. Adding an interview</div>
</strong><br>
&nbsp;<img src="93387-image.png" alt=""> </li><li>
<p>Figure 7 shows how to use a people picker to select an interviewer from within your organization.</p>
<strong>
<div class="caption">Figure 7. Choosing an interviewer</div>
</strong><br>
&nbsp;<img src="93388-image.png" alt=""> </li><li>
<p>After saving an interview, click a candidate interviewee's name to edit their interview details, as shown in Figure 8. You can update the details of the interview, or add notes about the interview if it has already taken place. You can also specify whether
 a job has been offered to the candidate.</p>
<strong>
<div class="caption">Figure 8. Editing interview details</div>
</strong><br>
&nbsp;<img src="93389-image.png" alt=""> </li><li>
<p>When a job has been offered to a candidate, the candidate is moved from the <strong>
<span class="ui">Interviewees</span></strong> list to the <strong><span class="ui">Job Offered</span></strong> list, as shown in Figure 9.</p>
<strong>
<div class="caption">Figure 9. Job Offered list</div>
</strong><br>
&nbsp;<img src="93379-image.png" alt=""> </li><li>
<p>Click the name of a candidate to whom a job has been offered to see candidate details, as shown in Figure 10. From here, you can print an offer letter to send to the candidate.</p>
<strong>
<div class="caption">Figure 10. Candidate details</div>
</strong><br>
&nbsp;<img src="93380-image.png" alt=""> </li><li>
<p>Figure 11 shows a preview of the job offer letter. The letter's content is built from boilerplate text merged with details of the candidate, job, and interview.</p>
<strong>
<div class="caption">Figure 11. Job offer letter</div>
</strong><br>
&nbsp;<img src="93381-image.png" alt=""> </li></ol>
</div>
</div>
<h1>Troubleshooting</h1>
<div id="sectionSection4">
<p>Ensure that you have SharePoint Server 2013 properly configured to host apps (with a Developer Site Collection already created), or that you have signed up for an Office 365 Enterprise site configured to host apps.</p>
<p>&nbsp;</p>
</div>
<h1>Change log</h1>
<div id="sectionSection5"><strong>
<div class="caption"></div>
</strong>
<div>
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<p>Version</p>
</th>
<th>
<p>Date</p>
</th>
</tr>
<tr>
<td>
<p>First version</p>
</td>
<td>
<p>July 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1>Related content</h1>
<div id="sectionSection6">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp179930.aspx" target="_blank">Apps for SharePoint overview</a></p>
</li><li>
<p><a href="http://www.jQuery.com" target="_blank">jQuery</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj713593.aspx" target="_blank">SharePoint People Picker control</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/ms970435.aspx" target="_blank">JavaScript</a></p>
</li></ul>
</div>
</div>
</div>
<p>&nbsp;</p>
