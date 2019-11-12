# Remote Authentication in SharePoint Online Using the Client Object Model
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Office 365
- Sharepoint Online
## Topics
- client side object model
- remote authentication
- client authentication
## Updated
- 04/28/2011
## Description

<h1>Introduction</h1>
<div><span style="font-size:xx-small">
<div><span style="font-size:small">This sample demonstrates how to authenticate against Microsoft SharePoint Online in client applications using the managed SharePoint client-side object models.</span></div>
<div><span style="font-size:small"><br>
</span></div>
</span></div>
<h1><span>Prerequisites</span></h1>
<p><span style="font-size:small">This sample requires the SharePoint Foundation 2010 client object model redistributable. This distributable is included as part of the SharePoint 2010 installation, so if you are running the sample on a computer on which SharePoint
 2010 is installed, no further action is necessary.</span></p>
<p><span style="font-size:small">For computers on which SharePoint 2010 is not installed, you can download the SharePoint Foundation 2010 client object model redistributable from the following location:</span></p>
<p><span style="font-size:small"><a href="http://www.microsoft.com/downloads/en/details.aspx?displaylang=en&FamilyID=b4579045-b183-4ed4-bf61-dc2f0deabe47">http://www.microsoft.com/downloads/en/details.aspx?displaylang=en&amp;FamilyID=b4579045-b183-4ed4-bf61-dc2f0deabe47</a></span></p>
<p><span style="font-size:small"><br>
</span></p>
<h1><span>Building the Sample</span></h1>
<div><span style="font-size:small">To build the sample using Visual Studio 2010:</span><br>
<span style="font-size:x-small">&nbsp;</span> <span style="font-size:small">&nbsp;&nbsp; &nbsp; 1. Open Windows Explorer and navigate to the&nbsp; directory.</span><br>
<span style="font-size:small">&nbsp;&nbsp;&nbsp;&nbsp; 2. Double-click the icon for the .sln (solution) file to open the file in Visual Studio.</span><br>
<span style="font-size:small">&nbsp;&nbsp;&nbsp;&nbsp; 3. In the Build menu, select Build Solution. The application will be built in the default \Debug or \Release directory.</span></div>
<div><span style="font-size:small"><br>
</span></div>
<h1><span>Description</span></h1>
<div><span style="font-size:small">This code sample demonstrates this technique of adding the SharePoint federation cookies to the ClientContext object. It provides a set of classes that you can use to perform federated user authentication. You start with the
 sample program so that you can see what changes you must make when using this code compared to using an HTTP authenticated web server.</span></div>
<div><span style="font-size:small"><br>
</span></div>
<h1>More Information</h1>
<div><span style="font-size:small">For more information, please refer to the MSDN technical article
<a href="http://msdn.microsoft.com/en-us/library/hh147177.aspx">Remote Authentication in SharePoint Online Using the Client Object Model</a></span></div>
