=============================================================================
   ASP.NET APPLICATION : CSASPNETRemoteUploadAndDownload Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The CSASPNETRemoteUploadAndDownload sample shows how to upload files to and 
download files from remote server in an ASP.NET application. 

This project is created by using WebClient and FtpWebRequest object in C# 
language. Both WebClient and FtpWebRequest classes provide common methods for 
sending data to URI of server and receiving data from a resource identified 
by URI as well. When uploading or downloading files, these classes will do 
webrequest to the URL which user types in.

The UploadData() method sends a data buffer(without encoding it) to a 
resource using HTTP or FTP method specified in the method parameter, and then 
returns the web response from the server. The DownloadData() method posts an 
HTTP or FTP download request to the remote server and get outputstream from 
the server.


/////////////////////////////////////////////////////////////////////////////
Prerequisites:

1. The virtual upload path of remote server need to be enabled the writing 
 permissions for an Asp.net account or other accessible user accounts, which 
 means the directory on server should be writable. Otherwise the remote 
 server will return 403 error(Regarding server status code definitions, see 
 http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html).

2. If remote server(http or ftp) does not enable anonymous access, then we 
 need to provide a user account and set WebClient.Credentials property in 
 order to verify the user, allowing the server to know who you are. 
 (the following documentation will also refer to it).

3. Attention:
 If the server is running IIS7 verion, we need to do a few more steps to set 
 user permissions:

 Install WebDev Extension for IIS7. Please follow the instructions in this 
 article. 
 http://learn.iis.net/page.aspx/350/installing-and-configuring-webdav-on-iis-7/

 If the server machine does not have the WebDev Extension program, you can 
 download it from the links below

    32bit: 
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en
    
    64bit:
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=13e97aaa-fb1b-4cf8-b95f-19ae02321385&displaylang=en

  If you do not install and enable this extension service, IIS7 will forbid 
  RemoteUpload method and return the 404 or 403 http error code.


/////////////////////////////////////////////////////////////////////////////
Demo:

1.Open CSRemoteUploadAndDownload.sln file.

2.Right click the Default.aspx, choose "view in browser" or view 
"Default.aspx" in browser to run this application.

3.There are two features you can see in Default.aspx page.

  For RemoteUpload:
    1) Enter a server url which you want to upload to. 
    (e.g. http://www.example.com/upload/, ftp://ftpserver/)
    2) Choose your local file by using UploadFile control.
    3) Click the upload button and then you will see the result on screen.

  For RemoteDownload:
    1) Enter a server url from which you want to download. 
      (e.g. http://www.example.com/download/test.txt, ftp://ftpserver/test.txt)
    2) Enter a destination local path or directory which saves the file. 
    (e.g. C:\Temp\)
    3) Click the download button and then result will be shown on screen.
    

/////////////////////////////////////////////////////////////////////////////
Implementation:

Use WebClient and FtpWebRequest to upload, download data in .NET application.
Notice that adding reference of System.IO, System.Net at the very beginning.

-------------
Upload:
RemoteUpload class has some necessary preperties and constructors,
which should be used in UploadFile method.

UrlString preperty: The URI of the resource to receive the data
FileNamePath : The Full physical path of Upload file.
NewFileName: The Remote server upload file name.

1. Get the file name of FileNamePath preperty.

2. Get the array of bytes in post filedata.

3. Get the correct remote server url.

4. * Http: 
   Create WebClient instance(importing the reference of namespace System.Net), 
   using DefaultCredentials to sets the network credentials that are sent to 
   the host and used this account to authenticate the request,which means use 
   DefaultCredentials to login in remote server(about WebClient.Credentials 
   Property.

   MSDN: http://msdn.microsoft.com/en-us/library/system.net.webclient.credentials.aspx 
         http://msdn.microsoft.com/en-us/library/system.net.networkcredential.aspx)
   
   * FTP:
   Create FtpWebRequest instance. Use WebRequestMethods.Ftp.UploadFile method 
   to upload file. In this sample, the Ftp Server can be access by anonymous 
   users, so it is unnecessary for us to specify username and password.
   If Server needs to authenticate users before connecting it. We should also 
   provide our username and password. (Regarding FtpWebRequest.Credentials 
   Property

   MSDN: http://msdn.microsoft.com/en-us/library/354e0act.aspx)

5. Get the bytes of uploaded file which store in memory(FileStream).

6. Upload data buffer to remote server by using HTTP method "PUT". If the 
request method is empty, default value is "POST" for http server and "STOR" 
for ftp server.

7. Then UploadData() method return the boolean value if Upload succeed or 
failed.

8. Close and release WebClient or WebRequest resource.

-------------
Download:
The primary function of the DownloadFile method is to download data from 
remote server to local application through URI with a specified url address.

1. Get the remote server url address.

2. Get the destination file path.

3. Use WebRequest object to check if the file exists or not on the server 
 side (importing the reference of namespace System.Net).

4. * HTTP: 
   Create WebClient(System.Net,similar to UploadFile method metioned above) 
   instance, access DownloadData() method to download file buffer resource 
   with the specified URI to local path. Actually, for HTTP resource, the 
   "GET" method is used.

   * FTP:
   Create FtpWebRequest instance, by using WebRequestMethods.Ftp.DownloadFile 
   method, we can retrive response stream from the server.This method uses 
   the "RETR" command to download an FTP resource.

5. This DownloData method will return a Byte arry of download resource.What 
we need to do is just using FileStream(using System.IO) to write a block of 
bytes to local server path from this download file buffer.

6. Close and release FileStream resource.


/////////////////////////////////////////////////////////////////////////////
References:

WebClient Class
http://msdn.microsoft.com/en-us/library/system.net.webclient.aspx

UploadData method
http://msdn.microsoft.com/en-us/library/system.net.webclient.uploaddata.aspx

DonwloadData method
http://msdn.microsoft.com/en-us/library/system.net.webclient.downloaddata.aspx

FtpWebRequest
http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx

How to: Upload Files with FTP
http://msdn.microsoft.com/en-us/library/ms229715.aspx


/////////////////////////////////////////////////////////////////////////////

