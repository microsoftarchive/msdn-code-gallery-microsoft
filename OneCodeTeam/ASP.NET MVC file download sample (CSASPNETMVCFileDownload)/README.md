# ASP.NET MVC file download sample (CSASPNETMVCFileDownload)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- ASP.NET MVC
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>ASP.NET MVC APPLICATION : CSASPNETMVCFileDownload Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
&nbsp;The project illustrates how to perform file downloading in ASP.NET MVC<br>
&nbsp;web application. <br>
&nbsp;<br>
&nbsp;The detailed functionality include how to use MVC routing to register <br>
&nbsp;a custom url routing pattern for our file download processing. Include:<br>
&nbsp;<br>
&nbsp;* how to display a file list for downloading<br>
&nbsp;* how to stream out file content in MVC web page<br>
&nbsp;<br>
&nbsp;and the sample application also include a custom ActionResult class<br>
&nbsp; which is used to output the file content(binary format).<br>
<br>
</p>
<h3>Prerequisite:</h3>
<p style="font-family:Courier New"><br>
Visual Studio 2008 SP1 with ASP.NET MVC 1.0 extension installed. <br>
<br>
*ASP.NET MVC 1.0 RTM download:<br>
<a target="_blank" href="http://www.microsoft.com/downloads/details.aspx?FamilyID=53289097-73ce-43bf-b6a6-35e00103cb4b&displaylang=en">http://www.microsoft.com/downloads/details.aspx?FamilyID=53289097-73ce-43bf-b6a6-35e00103cb4b&displaylang=en</a><br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New">&nbsp;<br>
*open the project<br>
<br>
*select &nbsp;default.aspx page and view it in browser<br>
<br>
*in the main page UI, select the &quot;FileDownload&quot; tab<br>
<br>
*click any file link to download the certain file<br>
<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
Step1. Create a Visual Studio 2008 SP1 ASP.NET MVC Web Application project<br>
<br>
Step2. Clear unnecessary parts in the project, including: scripts for jquery, <br>
&nbsp; &nbsp; &nbsp; controller and view for account management...<br>
<br>
Step3: Add our FileDownload Controller class and the corresponding view files<br>
&nbsp; &nbsp; &nbsp; For more information about how to create Controller class, refer to:<br>
&nbsp; &nbsp; &nbsp; <a target="_blank" href="http://www.asp.net/learn/mvc/#MVC_Controllers">
http://www.asp.net/learn/mvc/#MVC_Controllers</a><br>
&nbsp; &nbsp; &nbsp; &nbsp;<br>
Step4: Add a custom ActionResult class for output file content<br>
<br>
Step5: Add the proper URL routing rules in global.asax file's events. The<br>
&nbsp; &nbsp; &nbsp; Routing rules are very important for connecting the requests to the
<br>
&nbsp; &nbsp; &nbsp; proper MVC Controller and Action methods. For detailed info, refer to:<br>
&nbsp; &nbsp; &nbsp; <a target="_blank" href="http://weblogs.asp.net/scottgu/archive/2007/12/03/asp-net-mvc-framework-part-2-url-routing.aspx">
http://weblogs.asp.net/scottgu/archive/2007/12/03/asp-net-mvc-framework-part-2-url-routing.aspx</a><br>
<br>
<br>
Key components:<br>
<br>
* web.config file: contains all the necessary configuration information of <br>
&nbsp;this web application<br>
<br>
* global.asax: contains all the URL routing rules<br>
<br>
* HomeController class: contains the main application <br>
&nbsp; &nbsp;navigation logic(such as default page and about page)<br>
<br>
* FileController class: contains the navigation and processing logic of the <br>
&nbsp;FileDownloading function(include file list and file download)<br>
<br>
* Home Views: the page UI elements for HomeController<br>
<br>
* File Views: the page UI elements for FileController<br>
<br>
* shared Views & Site.Master: those UI elements shared by all page UI<br>
<br>
* BinaryContentResult: Custom ActionResult class used for outputting file <br>
&nbsp;content(as binary format)<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
#ASP.NET MVC Tutorials<br>
<a target="_blank" href="http://www.asp.net/Learn/mvc/">http://www.asp.net/Learn/mvc/</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
