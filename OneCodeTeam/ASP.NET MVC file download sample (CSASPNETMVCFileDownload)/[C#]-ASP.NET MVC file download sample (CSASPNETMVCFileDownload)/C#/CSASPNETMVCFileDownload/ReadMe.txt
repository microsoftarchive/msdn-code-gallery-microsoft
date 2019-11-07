========================================================================
       ASP.NET MVC APPLICATION : CSASPNETMVCFileDownload Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

  The project illustrates how to perform file downloading in ASP.NET MVC
  web application. 
  
  The detailed functionality include how to use MVC routing to register 
  a custom url routing pattern for our file download processing. Include:
  
  * how to display a file list for downloading
  * how to stream out file content in MVC web page
  
  and the sample application also include a custom ActionResult class
   which is used to output the file content(binary format).


/////////////////////////////////////////////////////////////////////////////
Prerequisite:

Visual Studio 2008 SP1 with ASP.NET MVC 1.0 extension installed. 

*ASP.NET MVC 1.0 RTM download:
http://www.microsoft.com/downloads/details.aspx?FamilyID=53289097-73ce-43bf-b6a6-35e00103cb4b&displaylang=en


/////////////////////////////////////////////////////////////////////////////
Demo:
  
*open the project

*select  default.aspx page and view it in browser

*in the main page UI, select the "FileDownload" tab

*click any file link to download the certain file


/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step1. Create a Visual Studio 2008 SP1 ASP.NET MVC Web Application project

Step2. Clear unnecessary parts in the project, including: scripts for jquery, 
       controller and view for account management...

Step3: Add our FileDownload Controller class and the corresponding view files
       For more information about how to create Controller class, refer to:
       http://www.asp.net/learn/mvc/#MVC_Controllers
        
Step4: Add a custom ActionResult class for output file content

Step5: Add the proper URL routing rules in global.asax file's events. The
       Routing rules are very important for connecting the requests to the 
       proper MVC Controller and Action methods. For detailed info, refer to:
       http://weblogs.asp.net/scottgu/archive/2007/12/03/asp-net-mvc-framework-part-2-url-routing.aspx


Key components:

* web.config file: contains all the necessary configuration information of 
  this web application

* global.asax: contains all the URL routing rules

* HomeController class: contains the main application 
    navigation logic(such as default page and about page)

* FileController class: contains the navigation and processing logic of the 
  FileDownloading function(include file list and file download)

* Home Views: the page UI elements for HomeController

* File Views: the page UI elements for FileController

* shared Views & Site.Master: those UI elements shared by all page UI

* BinaryContentResult: Custom ActionResult class used for outputting file 
  content(as binary format)


/////////////////////////////////////////////////////////////////////////////
References:

#ASP.NET MVC Tutorials
http://www.asp.net/Learn/mvc/


/////////////////////////////////////////////////////////////////////////////