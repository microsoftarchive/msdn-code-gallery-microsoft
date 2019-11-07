================================================================================
       Windows APPLICATION: CSFTPUpload Overview                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to list subdirectories and files of a folder on the FTP 
server and upload a whole local folder to it.

The operations includes
1. List subdirectories and files of a folder on the FTP server.
2. Delete a file or directory on the FTP server. 
3. Upload a file to the FTP server. 
4. Create a directory on the FTP server. 
 
////////////////////////////////////////////////////////////////////////////////
Demo:
 
Step1. Build this sample in VS2010.
 
Step2. Run CSFTPUpload.exe.
 
Step3. Type the url of a valid FTP server in the FTPServer textbox. The url should start
       with ftp:// like ftp://localhost 

* List subdirectories and files
 
Step4. Press the button "Connect". The application will show a dialog to input the 
       credentials. If the FTP server supports anonymous users, you can check 
	   "Log on anonymously".
 
	   It may take several seconds to connect to the FTP server for the first time, then 
	   you will see the files and subdirectories in the “FTP File Explorer”. The items in
	   the list box are like 

			2010-12-1 12:00 <DIR> FolderA

			2010-12-1 12:00 1024 FileB


	   <DIR> means a folder.
 
Step5. Double click an item contains <DIR> in the “FTP File Explorer”, the  “FTP File Explorer”
       will navigate to this sub directory and list the files and subdirectories of it.


Step6. Click the button “Parent Folder”, if current path is not the root path of the server, 
       the  “FTP File Explorer” will navigate to this parent directory and list the files and
       subdirectories of it.
 
* Delete a file or directory on the FTP server. 

Step7. Select one or more items in the  “FTP File Explorer” , click the button “Delete”. This 
       application will delete the selected items.

* Upload a file to the FTP server.   

Step8. Click the "Browse" button in the "Upload Files" group box to select one or more files. 
       And then click the "Upload Files" button, this application will upload the selected 
	   files to the FTP server.


/////////////////////////////////////////////////////////////////////////////
Code Logic:


1. Design a class FTPClientManager. This class supplies following features.

   1.1. Verify whether a file or a directory exists on the FTP server.
   1.2. Delete files or directories on the FTP server.

		To delete a file, create a FtpWebRequest and set the Method property to 
        WebRequestMethods.Ftp.DeleteFile. 

        To remove a directory, create a FtpWebRequest and set the Method property to 
        WebRequestMethods.Ftp.RemoveDirectory. 

   1.3. Create a directory on the FTP server.

		To create a directory, create a FtpWebRequest and set the Method property to 
		WebRequestMethods.Ftp.MakeDirectory. 

   1.4. Manage the FTPUploadClient to upload files to the FTP server. 

        To upload a file, create a FtpWebRequest and set the Method property to 
        WebRequestMethods.Ftp.UploadFile. 

   1.5. Supply the events UrlChanged, ErrorOccurred, StatusChanged, FileUploadCompleted
        NewMessageArrived.

2. Design a class FTPUploadClient to upload files from FTP server.

3. Design the classes ErrorEventArgs, FileUploadCompletedEventArgs, NewMessageEventArg
   and FTPClientManagerStatus used by the class FTPClientManager.

4. Design a class FTPFileSystem that represents a file on the remote FTP server. It also
   contains static methods to parse the response of a ListDirectoryDetails FtpWebRequest.

    When set the Method property of an FtpWebRequest to 
    WebRequestMethods.Ftp.ListDirectoryDetails(the FTP LIST protocol method to get a 
    detailed listing of the files on an FTP server), the response of server will 
    contain many records of information, and each record represents a file or a directory. 
    Depended on the FTP Directory Listing Style of the server, the record is like 
    a. MSDOS
       1.1. Directory
            12-13-10  12:41PM  <DIR>  Folder A
       1.2. File
            12-13-10  12:41PM  [Size] File B  
            
      NOTE: The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit
            years is not checked in IIS or other FTP servers..
           
    b. UNIX
       2.1. Directory
            drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
       2.2. File
            -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
     
       NOTE: The date segment does not contains year.
	   
5. In the MainForm, register the UrlChanged event of an FTPClientManager, if the Url changed, 
   list files and sub directories of the new url in the “FTP File Explorer”. 
   It also registers the other events of FTPClientManager and log the events. 
 
/////////////////////////////////////////////////////////////////////////////
References:

FtpWebRequest Class
http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx

WebRequestMethods.FTP Class
http://msdn.microsoft.com/en-us/library/system.net.webrequestmethods.ftp.aspx

Unix File Permissions
http://www.unix.com/tips-tutorials/19060-unix-file-permissions.html

/////////////////////////////////////////////////////////////////////////////

