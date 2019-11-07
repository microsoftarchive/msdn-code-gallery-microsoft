# ASP.NET upload/download file to remote server(CSASPNETRemoteUploadAndDownload)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Upload
- Download
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>ASP.NET APPLICATION : CSASPNETRemoteUploadAndDownload Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The CSASPNETRemoteUploadAndDownload sample shows how to upload files to and <br>
download files from remote server in an ASP.NET application. <br>
<br>
This project is created by using WebClient and FtpWebRequest object in C# <br>
language. Both WebClient and FtpWebRequest classes provide common methods for <br>
sending data to URI of server and receiving data from a resource identified <br>
by URI as well. When uploading or downloading files, these classes will do <br>
webrequest to the URL which user types in.<br>
<br>
The UploadData() method sends a data buffer(without encoding it) to a <br>
resource using HTTP or FTP method specified in the method parameter, and then <br>
returns the web response from the server. The DownloadData() method posts an <br>
HTTP or FTP download request to the remote server and get outputstream from <br>
the server.<br>
<br>
</p>
<h3>Prerequisites:</h3>
<p style="font-family:Courier New"><br>
1. The virtual upload path of remote server need to be enabled the writing <br>
permissions for an Asp.net account or other accessible user accounts, which <br>
means the directory on server should be writable. Otherwise the remote <br>
server will return 403 error(Regarding server status code definitions, see <br>
<a target="_blank" href="http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html).">http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html).</a><br>
<br>
2. If remote server(http or ftp) does not enable anonymous access, then we <br>
need to provide a user account and set WebClient.Credentials property in <br>
order to verify the user, allowing the server to know who you are. <br>
(the following documentation will also refer to it).<br>
<br>
3. Attention:<br>
If the server is running IIS7 verion, we need to do a few more steps to set <br>
user permissions:<br>
<br>
Install WebDev Extension for IIS7. Please follow the instructions in this <br>
article. <br>
<a target="_blank" href="http://learn.iis.net/page.aspx/350/installing-and-configuring-webdav-on-iis-7/">http://learn.iis.net/page.aspx/350/installing-and-configuring-webdav-on-iis-7/</a><br>
<br>
If the server machine does not have the WebDev Extension program, you can <br>
download it from the links below<br>
<br>
&nbsp; &nbsp;32bit: <br>
<a target="_blank" href="&lt;a target=" href="http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en">http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en</a>'&gt;<a target="_blank" href="http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en">http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en</a><br>
&nbsp; &nbsp;<br>
&nbsp; &nbsp;64bit:<br>
<a target="_blank" href="&lt;a target=" href="http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en">http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en</a>'&gt;<a target="_blank" href="http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en">http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en</a><br>
<br>
&nbsp;If you do not install and enable this extension service, IIS7 will forbid <br>
&nbsp;RemoteUpload method and return the 404 or 403 http error code.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
1.Open CSRemoteUploadAndDownload.sln file.<br>
<br>
2.Right click the Default.aspx, choose &quot;view in browser&quot; or view <br>
&quot;Default.aspx&quot; in browser to run this application.<br>
<br>
3.There are two features you can see in Default.aspx page.<br>
<br>
&nbsp;For RemoteUpload:<br>
&nbsp; &nbsp;1) Enter a server url which you want to upload to. <br>
&nbsp; &nbsp;(e.g. <a target="_blank" href="http://www.example.com/upload/,">http://www.example.com/upload/,</a> ftp://ftpserver/)<br>
&nbsp; &nbsp;2) Choose your local file by using UploadFile control.<br>
&nbsp; &nbsp;3) Click the upload button and then you will see the result on screen.<br>
<br>
&nbsp;For RemoteDownload:<br>
&nbsp; &nbsp;1) Enter a server url from which you want to download. <br>
&nbsp; &nbsp; &nbsp;(e.g. <a target="_blank" href="http://www.example.com/download/test.txt,">
http://www.example.com/download/test.txt,</a> ftp://ftpserver/test.txt)<br>
&nbsp; &nbsp;2) Enter a destination local path or directory which saves the file.
<br>
&nbsp; &nbsp;(e.g. C:\Temp\)<br>
&nbsp; &nbsp;3) Click the download button and then result will be shown on screen.<br>
&nbsp; &nbsp;<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
Use WebClient and FtpWebRequest to upload, download data in .NET application.<br>
Notice that adding reference of System.IO, System.Net at the very beginning.<br>
<br>
-------------<br>
Upload:<br>
RemoteUpload class has some necessary preperties and constructors,<br>
which should be used in UploadFile method.<br>
<br>
UrlString preperty: The URI of the resource to receive the data<br>
FileNamePath : The Full physical path of Upload file.<br>
NewFileName: The Remote server upload file name.<br>
<br>
1. Get the file name of FileNamePath preperty.<br>
<br>
2. Get the array of bytes in post filedata.<br>
<br>
3. Get the correct remote server url.<br>
<br>
4. * Http: <br>
&nbsp; Create WebClient instance(importing the reference of namespace System.Net),
<br>
&nbsp; using DefaultCredentials to sets the network credentials that are sent to <br>
&nbsp; the host and used this account to authenticate the request,which means use
<br>
&nbsp; DefaultCredentials to login in remote server(about WebClient.Credentials <br>
&nbsp; Property.<br>
<br>
&nbsp; MSDN: <a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.webclient.credentials.aspx">
http://msdn.microsoft.com/en-us/library/system.net.webclient.credentials.aspx</a>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; <a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.networkcredential.aspx)">
http://msdn.microsoft.com/en-us/library/system.net.networkcredential.aspx)</a><br>
&nbsp; <br>
&nbsp; * FTP:<br>
&nbsp; Create FtpWebRequest instance. Use WebRequestMethods.Ftp.UploadFile method
<br>
&nbsp; to upload file. In this sample, the Ftp Server can be access by anonymous <br>
&nbsp; users, so it is unnecessary for us to specify username and password.<br>
&nbsp; If Server needs to authenticate users before connecting it. We should also
<br>
&nbsp; provide our username and password. (Regarding FtpWebRequest.Credentials <br>
&nbsp; Property<br>
<br>
&nbsp; MSDN: <a target="_blank" href="http://msdn.microsoft.com/en-us/library/354e0act.aspx)">
http://msdn.microsoft.com/en-us/library/354e0act.aspx)</a><br>
<br>
5. Get the bytes of uploaded file which store in memory(FileStream).<br>
<br>
6. Upload data buffer to remote server by using HTTP method &quot;PUT&quot;. If the
<br>
request method is empty, default value is &quot;POST&quot; for http server and &quot;STOR&quot;
<br>
for ftp server.<br>
<br>
7. Then UploadData() method return the boolean value if Upload succeed or <br>
failed.<br>
<br>
8. Close and release WebClient or WebRequest resource.<br>
<br>
-------------<br>
Download:<br>
The primary function of the DownloadFile method is to download data from <br>
remote server to local application through URI with a specified url address.<br>
<br>
1. Get the remote server url address.<br>
<br>
2. Get the destination file path.<br>
<br>
3. Use WebRequest object to check if the file exists or not on the server <br>
side (importing the reference of namespace System.Net).<br>
<br>
4. * HTTP: <br>
&nbsp; Create WebClient(System.Net,similar to UploadFile method metioned above) <br>
&nbsp; instance, access DownloadData() method to download file buffer resource <br>
&nbsp; with the specified URI to local path. Actually, for HTTP resource, the <br>
&nbsp; &quot;GET&quot; method is used.<br>
<br>
&nbsp; * FTP:<br>
&nbsp; Create FtpWebRequest instance, by using WebRequestMethods.Ftp.DownloadFile
<br>
&nbsp; method, we can retrive response stream from the server.This method uses <br>
&nbsp; the &quot;RETR&quot; command to download an FTP resource.<br>
<br>
5. This DownloData method will return a Byte arry of download resource.What <br>
we need to do is just using FileStream(using System.IO) to write a block of <br>
bytes to local server path from this download file buffer.<br>
<br>
6. Close and release FileStream resource.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
WebClient Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.webclient.aspx">http://msdn.microsoft.com/en-us/library/system.net.webclient.aspx</a><br>
<br>
UploadData method<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.webclient.uploaddata.aspx">http://msdn.microsoft.com/en-us/library/system.net.webclient.uploaddata.aspx</a><br>
<br>
DonwloadData method<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.webclient.downloaddata.aspx">http://msdn.microsoft.com/en-us/library/system.net.webclient.downloaddata.aspx</a><br>
<br>
FtpWebRequest<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx">http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx</a><br>
<br>
How to: Upload Files with FTP<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms229715.aspx">http://msdn.microsoft.com/en-us/library/ms229715.aspx</a><br>
<br>
</p>
<h3></h3>
<p style="font-family:Courier New"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
