# SharePoint 2013: Corporate calendar app
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
- sites and content
## Updated
- 04/23/2013
## Description

<div id="header">Demonstrates how to create a search-based app that aggregates events from calendars in SharePoint 2013; the app presents a single calendar that aggregates events from the entire farm or from designated calendars.</div>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<p><span class="label">Provided by:</span><a href="http://mvp.microsoft.com/en-us/mvp/Scot%20Hillier-33471" target="_blank">Scot Hillier</a>,
<a href="http://www.criticalpathtraining.com/Pages/default.aspx" target="_blank">
Critical Path Training</a>.</p>
<p>This sample make use of the RESTful endpoint to perform a search for events in SharePoint calendars. The search queries are based on a date range and optionally paths to specific calendars. The results are displayed in the FullCalendar jQuery plug-in, which
 is an open-source component located at <a href="http://arshaw.com/fullcalendar" target="_blank">
http://arshaw.com/fullcalendar</a>.</p>
</div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection0">
<p>This sample requires the following:</p>
<ul>
<li>
<p>A SharePoint 2013 development environment.</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012.</p>
</li><li>
<p>The Search Service application properly set up and configured.</p>
</li></ul>
</div>
<h1 class="heading">Key components</h1>
<div class="section" id="sectionSection1">
<ul>
<li>
<p>The sample is a SharePoint-hosted app written entirely in JavaScript.</p>
</li><li>
<p>The sample uses the <a href="http://arshaw.com/fullcalendar" target="_blank">FullCalendar jQuery plug-in</a> for display.</p>
</li><li>
<p>The key code for the sample can be found in the library named <strong>wingtip.events.js</strong>, which is located in the
<strong>Scripts</strong> folder. The library contains all the functionality needed to perform searches and to package the events into an array for consumption by the FullCalendar plug-in.</p>
</li><li>
<p>The <span><span class="keyword">App.js</span></span> library contains the start-up code for the app.</p>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection2">
<p>The app requires that you create managed properties named <span><span class="keyword">EventDate</span></span>,
<span><span class="keyword">EndDate</span></span>, and <span><span class="keyword">AllDayEvent</span></span>. Once you create them, map these managed properties to the crawled properties shown in the following table:</p>
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<p>Crawled property</p>
</th>
<th>
<p>Managed property</p>
</th>
</tr>
<tr>
<td>
<p>ows_EventDate</p>
</td>
<td>
<p>EventDate</p>
</td>
</tr>
<tr>
<td>
<p>ows_EndDate</p>
</td>
<td>
<p>EndDate</p>
</td>
</tr>
<tr>
<td>
<p>ows_fAllDayEvent</p>
</td>
<td>
<p>AllDayEvent</p>
</td>
</tr>
</tbody>
</table>
</div>
<p>After mapping the managed properties, perform a full crawl.</p>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection3">
<p>To run and test the sample, do the following:</p>
<div class="subSection">
<ol>
<li>
<p>Open CorporateCalendarApp.sln in Visual Studio 2012.</p>
</li><li>
<p>Edit the <span><span class="keyword">Site URL</span></span> property to refer to a test site where you will deploy the solution.</p>
</li><li>
<p>Press F5.</p>
</li><li>
<p>When prompted, grant permissions for the app to use the search service.</p>
</li><li>
<p>When the app appears, it should be populated with data from all calendars in the farm to which the current user has access.</p>
</li><li>
<p>If you want, click the <span class="ui">Manage Included Calendar Paths</span> link and add new links to the
<span class="ui">Included Paths</span> links list. Any link added to this list will become a property restriction in the search query using the form
<strong>Path:{URL}</strong>.</p>
</li></ol>
</div>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection4">
<p>If the app throws an error, the most likely cause is that the managed properties are not properly configured.</p>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection5">
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<td>
<p>First release</p>
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
<div class="section" id="sectionSection6">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp142385.aspx" target="_blank">Programming using the SharePoint 2013 REST service</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj860569.aspx" target="_blank">SharePoint 2013 REST API, endpoints, and samples</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj163300.aspx" target="_blank">Search in SharePoint 2013</a></p>
</li></ul>
</div>
</div>
</div>
