# Download files from FTP server (VBFTPDownload)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows General
- Network
## Topics
- Download
- FTP
## Updated
- 06/19/2011
## Description

<p style="font-family:Courier New"></p>
<h2>Windows APPLICATION: VBFTPDownload Overview </h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New">The sample demonstrates how to list subdirectories and files of a folder on a FTP
<br>
server and download all of them.<br>
<br>
The operations includes<br>
1. &nbsp;List subdirectories and files of a folder on the FTP server.<br>
&nbsp; &nbsp;<br>
&nbsp;&nbsp;&nbsp;&nbsp;When set the Method property of an FtpWebRequest to <br>
&nbsp; &nbsp;WebRequestMethods.Ftp.ListDirectoryDetails(the FTP LIST protocol method to get a
<br>
&nbsp; &nbsp;detailed listing of the files on an FTP server), the response of server will
<br>
&nbsp; &nbsp;contain many records of information, and each record represents a file or a directory.
<br>
&nbsp; &nbsp;Depended on the FTP Directory Listing Style of the server, the record is like
<br>
&nbsp; &nbsp;1. MSDOS<br>
&nbsp; &nbsp; &nbsp; 1.1. Directory<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;12-13-10 &nbsp;12:41PM &nbsp;&lt;DIR&gt; &nbsp;Folder A<br>
&nbsp; &nbsp; &nbsp; 1.2. File<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;12-13-10 &nbsp;12:41PM &nbsp;[Size] File B &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp;NOTE: The date segment is like &quot;12-13-10&quot; instead of &quot;12-13-2010&quot; if Four-digit<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;years is not checked in IIS or other FTP servers..<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <br>
&nbsp; &nbsp;2. UNIX<br>
&nbsp; &nbsp; &nbsp; 2.1. Directory<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A<br>
&nbsp; &nbsp; &nbsp; 2.2. File<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;-rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B<br>
&nbsp; &nbsp; <br>
&nbsp; &nbsp; &nbsp; NOTE: The date segment does not contains year.<br>
<br>
2. Download a file on the FTP server. <br>
&nbsp; <br>
&nbsp; To download a file, create a FtpWebRequest and set the Method property to <br>
&nbsp; WebRequestMethods.Ftp.DownloadFile. <br>
<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Step1. Build this sample in VS2010.<br>
<br>
Step2. Run VBFTPDownload.exe.<br>
<br>
Step3. Type the url of a valid FTP server in the FTPServer textbox. The url should start<br>
&nbsp; &nbsp; &nbsp; with ftp:// like ftp://localhost <br>
<br>
* List subdirectories and files<br>
<br>
Step4. Press the button &quot;Connect&quot;.The application will show a dialog to input the
<br>
&nbsp; &nbsp; &nbsp; credentials. If the FTP server supports anonymous users, you can check
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &quot;Log on anonymously&quot;.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; It may take several seconds to connect to the FTP server for the first time, then
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; you will see the files and subdirectories in the “FTP File Explorer”. The items in<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; the list box are like <br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2010-12-1 12:00 &lt;DIR&gt; Folder A<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2010-12-1 12:00 1024 File B<br>
<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &lt;DIR&gt; means a folder.<br>
<br>
Step5. Double click an item contains &lt;DIR&gt; in the “FTP File Explorer”, the &nbsp;“FTP File Explorer”<br>
&nbsp; &nbsp; &nbsp; will navigate to this sub directory and list the files and subdirectories of it.<br>
<br>
<br>
Step6. Click the button “Parent Folder”, if current path is not the root path of the server,
<br>
&nbsp; &nbsp; &nbsp; the &nbsp;“FTP File Explorer” will navigate to this parent directory and list the files and<br>
&nbsp; &nbsp; &nbsp; subdirectories of it.<br>
<br>
* Download files from the FTP server. <br>
<br>
Step7. Select one or more items in the &nbsp;“FTP File Explorer” , click the button “Browse” to
<br>
&nbsp; &nbsp; &nbsp; choose the download path, then click the button “Download”.<br>
<br>
&nbsp; &nbsp; &nbsp; This application will download the selected items to the download path. If the selected<br>
&nbsp; &nbsp; &nbsp; items contains a folder, this application will also download the files and subdirectories<br>
&nbsp; &nbsp; &nbsp; of it.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
<br>
1. Design a class FTPClientManager. This class supplies following features.<br>
<br>
&nbsp; 1.1 Check whether the Url to navigate is valid.<br>
&nbsp; &nbsp; &nbsp; Create a FtpWebRequest of the Url &nbsp;and set the Method property to
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; WebRequestMethods.Ftp.PrintWorkingDirectory. If there is no exception, then the Url
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; is valid.<br>
<br>
&nbsp; 1.2 List files and sub directories of a folder on the FTP server.<br>
&nbsp; &nbsp; &nbsp; Create a FtpWebRequest of the Url &nbsp;and set the Method property to
<br>
&nbsp; &nbsp; &nbsp; WebRequestMethods.Ftp. ListDirectoryDetails. The response of server will contain many
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; records of information, and each record represents a file or a directory.
<br>
<br>
&nbsp; 1.3 Manage the FTPDownloadClient to download files. &nbsp; <br>
<br>
&nbsp; 1.4 Supply the events UrlChanged, ErrorOccurred, StatusChanged, FileDownloadCompleted<br>
&nbsp; &nbsp; &nbsp; NewMessageArrived.<br>
<br>
2. Design a class FTPDownloadClient to download files from FTP server.<br>
&nbsp; When the download starts, it will download the file in a background thread. The downloaded
<br>
&nbsp; data is stored in a MemoryStream first, and then written to local file.<br>
<br>
3. Design the classes ErrorEventArgs, FileDownloadCompletedEventArgs, NewMessageEventArg<br>
&nbsp; and FTPClientManagerStatus used by the class FTPClientManager.<br>
<br>
4. Design a class FTPFileSystem that represents a file on the remote FTP server. It also<br>
&nbsp; contains static methods to parse the response of a ListDirectoryDetails FtpWebRequest.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; <br>
5. In the MainForm, register the UrlChanged event of an FTPClientManager, if the Url changed,
<br>
&nbsp; list files and sub directories of the new url in the “FTP File Explorer”. <br>
&nbsp; It also registers the other events of FTPClientManager and log the events.
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
FtpWebRequest Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx">http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx</a><br>
<br>
WebRequestMethods.FTP Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.webrequestmethods.ftp.aspx">http://msdn.microsoft.com/en-us/library/system.net.webrequestmethods.ftp.aspx</a><br>
<br>
Unix File Permissions<br>
<a target="_blank" href="http://www.unix.com/tips-tutorials/19060-unix-file-permissions.html">http://www.unix.com/tips-tutorials/19060-unix-file-permissions.html</a><br>
</p>
<h3></h3>
<p style="font-family:Courier New"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
