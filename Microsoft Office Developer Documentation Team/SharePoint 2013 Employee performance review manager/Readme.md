# SharePoint 2013: Employee performance review manager
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- data and storage
## Updated
- 06/27/2013
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Employee performance review manager</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<p>&nbsp;</p>
<div>
<p>Demonstrates how to use JavaScript and jQuery in an app for SharePoint to implement a scenario for managing employee review periods and associated objectives.</p>
</div>
<div>
<p><span>Provided by:</span> <a href="http://mvp.microsoft.com/en-US/findanmvp/Pages/profile.aspx?MVPID=c558e0ed-382f-4008-8002-4634a9167b99" target="_blank">
Martin Harwar</a>, <a href="http://point8020.com/Default.aspx" target="_blank">Point8020.com</a></p>
<p>The solution is based on the SharePoint-hosted app template provided by Visual Studio 2012. The solution uses the JavaScript implementation of the client object model to read, create, update, and delete data from lists based on user actions. The list data
 in this solution represents employees, review periods, and objectives.</p>
<p>In this app, users are either reviewers or employees. Reviewers create review periods that contain objectives. Employees can add comments to an objective. At the end of the review period, reviewers can mark the objectives as completed, deferred, or cancelled,
 and then create the next review period. Deferred objectives from the previous review period are automatically added to the new review.</p>
<p>The user interface is implemented with simple HTML elements and Cascading Style Sheet (CSS) styles to present a modern look and feel. The user interface also includes a client-side implementation of the SharePoint People Picker control. JavaScript and jQuery
 are used to control all aspects of the user interface, and the solution contains no server-side code.</p>
</div>
<h1>Prerequisites</h1>
<div id="sectionSection0">
<p>This sample requires the following:</p>
<ul>
<li>
<p>Office Developer Tools for Visual Studio 2012</p>
</li><li>
<p>Visual Studio 2012</p>
</li><li>
<p>Either:</p>
<ul>
<li>
<p>Access to an Office 365 Enterprise site that has been configured to host apps (recommended). In this environment, you will be able to add multiple users to the site, and can then treat those users as employees.</p>
<div>
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong> </th>
</tr>
<tr>
<td>
<p>Using an Office 365 Developer Site is not recommended because you will probably only be able to add one employee.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>SharePoint Server 2013 configured to host apps, and with a Developer Site Collection already created.</p>
</li></ul>
</li></ul>
</div>
<h1>Key components</h1>
<div id="sectionSection1">
<p>This sample app contains the following:</p>
<ul>
<li>
<p>The <span><strong><span class="keyword">Default.aspx</span></strong></span> webpage, which is used to present the employees, review periods, and objectives to reviewers. It also presents a list of current objectives to employees (reviewees).</p>
</li><li>
<p>The <span><strong><span class="keyword">App.js</span></strong></span> file in the
<span><strong><span class="keyword">scripts</span></strong></span> folder, which is used to retrieve and manage employee, review period, and objective data by using the JavaScript implementation of the client object model (JSOM). The
<span><strong><span class="keyword">App.js</span></strong></span> file also contains the user interface logic that is implemented in
<span><strong><span class="keyword">Default.aspx</span></strong></span>.</p>
</li><li>
<p>The <span><strong><span class="keyword">App.css</span></strong></span> file in the
<span><strong><span class="keyword">contents</span></strong></span> folder, which contains style definitions used by the elements in
<span><strong><span class="keyword">Default.aspx</span></strong></span>.</p>
</li><li>
<p>List definition instances for the Employee, Review, Objective, and DeferredObjective lists, in the respective folders. For example, the Employee list definition schema file is
<span><strong><span class="keyword">Schema.xml</span></strong></span> in the <span>
<strong><span class="keyword">Employee</span></strong></span> folder.</p>
</li><li>
<p>All other files are automatically provided by the Visual Studio project template for apps for SharePoint, and they have not been modified in the development of this sample app.</p>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection2">
<p>Follow these steps to configure the sample.</p>
<div>
<ol>
<li>
<p>Open the <span><strong><span class="keyword">SP_EmployeePerformance_js.sln</span></strong></span> file with Visual Studio 2012.</p>
</li><li>
<p>In the <span><strong><span class="keyword">Properties</span></strong></span> window, add the full URL to your Office 365 Enterprise site or SharePoint Server 2013 Developer Site collection to the
<span><strong><span class="keyword">Site URL</span></strong></span> property. You may be prompted to provide credentials if you added a URL to an Office 365 site.</p>
</li><li>
<p>No other configuration is necessary.</p>
</li></ol>
</div>
</div>
<h1>Build, run, and test the sample</h1>
<div id="sectionSection3">
<div>
<ol>
<li>
<p>Press Ctrl&#43;Shift&#43;B to build the solution.</p>
</li><li>
<p>Press F5 to run the app.</p>
</li><li>
<p>Sign in to your SharePoint Server 2013 or Office 365 Enterprise site if you are prompted to do so by the browser.</p>
</li><li>
<p>When the app appears, it determines whether you are a reviewer or an employee based on your SharePoint permissions. If your permissions include &quot;manage web&quot;, you are a reviewer, otherwise you are an employee (reviewee).</p>
<p>If you are a reviewer, the starting screen will resemble Figure 1. From here, you can start adding employees by using the people picker.</p>
<p>Users who are not reviewers will see their starting screen as described in step 14. These users can only enter comments on existing objectives.</p>
<strong>
<div class="caption">Figure 1. Start screen for a reviewer</div>
</strong><br>
<img src="85266-image.png" alt=""> </li><li>
<p>When you start typing in an employee's name, the people picker can find matches for current site members.</p>
<p>When you select a user and then click the <strong><span class="ui">&#43;</span></strong> (plus sign) button, the user is added as an employee, as shown in Figure 2.</p>
<strong>
<div class="caption">Figure 2. New employee added</div>
</strong><br>
<img src="85267-image.png" alt=""> </li><li>
<p>When you click an employee name, their reviews (if any) are displayed, or you are informed that the user does not currently have any reviews.</p>
<p>When you click the <strong><span class="ui">&#43; New Review</span></strong> link, the screen resembles Figure 3. You can enter comments, and you must choose a start date and an end date to represent the review period.</p>
<strong>
<div class="caption">Figure 3. New review form</div>
</strong><br>
<img src="85268-image.png" alt=""> </li><li>
<p>When the new review period has been saved, the screen resembles Figure 4. You can see the review period listed.</p>
<strong>
<div class="caption">Figure 4. New review added</div>
</strong><br>
<img src="85269-image.png" alt=""> </li><li>
<p>When you click a review period, the screen resembles Figure 5. You can now add objectives for this employee's review period.</p>
<strong>
<div class="caption">Figure 5. New review form</div>
</strong><br>
<img src="85270-image.png" alt=""> </li><li>
<p>When you click the <strong><span class="ui">&#43; New Objective</span></strong> link, a client-side jQuery dialog box appears, as shown in Figure 6. You can then enter the objective and set its status and priority.</p>
<strong>
<div class="caption">Figure 6. New objective form</div>
</strong><br>
<img src="85271-image.png" alt=""> </li><li>
<p>Figure 7 shows a review period with multiple objectives.</p>
<strong>
<div class="caption">Figure 7. Review with multiple objectives</div>
</strong><br>
<img src="85272-image.png" alt=""> </li><li>
<p>When you click an objective, you can set the status to one of four values: <span>
<strong><span class="keyword">Active</span></strong></span>, <span><strong><span class="keyword">Completed</span></strong></span>,
<span><strong><span class="keyword">Deferred</span></strong></span>, or <span><strong><span class="keyword">Cancelled</span></strong></span>, as shown in Figure 8. Note that while at least one objective in a review period is
<span><strong><span class="keyword">Active</span></strong></span>, the entire review period is considered
<span><strong><span class="keyword">Active</span></strong></span>, and you will not be allowed to set the review to
<span><strong><span class="keyword">Completed</span></strong></span> or start another review.</p>
<strong>
<div class="caption">Figure 8. Setting an objective's status</div>
</strong><br>
<img src="85273-image.png" alt=""> </li><li>
<p>When all objectives in a review have been set to a value other than <span><strong><span class="keyword">Active</span></strong></span>, you can then mark the review period as
<span><strong><span class="keyword">Completed</span></strong></span>, as shown in Figure 9.</p>
<strong>
<div class="caption">Figure 9. Setting a review period's status</div>
</strong><br>
<img src="85274-image.png" alt=""> </li><li>
<p>When a review has been set to <span><strong><span class="keyword">Completed</span></strong></span>, you can then create another review period for the employee.</p>
<p>If the previous review included any objectives that are marked as <span><strong><span class="keyword">Deferred</span></strong></span>, they are automatically added to the new review period and marked as
<span><strong><span class="keyword">Active</span></strong></span>. Figure 10 shows how the objective named &quot;Learn C#&quot; that was deferred in the original review period has been carried over.</p>
<strong>
<div class="caption">Figure 10. New review form with a previously deferred objective</div>
</strong><br>
<img src="85275-image.png" alt=""> </li><li>
<p>Figure 11 shows an example of the app when an employee has logged in. Their current objectives are listed.</p>
<strong>
<div class="caption">Figure 11. Start screen for an employee</div>
</strong><br>
<img src="85276-image.png" alt=""> </li><li>
<p>When an employee clicks one of the objectives, the screen resembles Figure 12. The employee can add or edit the
<strong><span class="ui">Employee Comments</span></strong> field, but cannot change anything else about the objective.</p>
<strong>
<div class="caption">Figure 12. Objective details form for an employee</div>
</strong><br>
<img src="85277-image.png" alt=""> </li><li>
<p>When a reviewer clicks an objective to which the employee has added a comment, the comment appears as in Figure 13. The reviewer can then change the objective's status as appropriate, as described in step 11.</p>
<strong>
<div class="caption">Figure 13. Reviewer's view of an employee's comment on an objective</div>
</strong><br>
<img src="85278-image.png" alt=""> </li></ol>
</div>
</div>
<h1>Troubleshooting</h1>
<div id="sectionSection4">
<p>Ensure that you have SharePoint Server 2013 configured to host apps (with a Developer Site Collection already created), or that you have signed up for an Office 365 Enterprise site configured to host apps.</p>
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
<p>June 2013</p>
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
<p><a href="http://msdn.microsoft.com/en-us/library/jj713593.aspx" target="_blank">SharePoint People Picker control</a></p>
</li><li>
<p><a href="http://www.jQuery.com" target="_blank">jQuery</a></p>
</li></ul>
<p>&nbsp;</p>
</div>
</div>
</div>
<p>&nbsp;</p>
