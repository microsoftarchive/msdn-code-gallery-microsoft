================================================================================
	            Windows APPLICATION: CSWebDownloader
===============================================================================
/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to show progress during the download.

The class System.Net.WebClient has a DownloadProgressChanged event, and you can register
this event to show the progress. But this class does not support Pause/Resume.

The class HttpDownloadClient in this sample could be used to download data through 
internet and supports following features:
1. Set the buffer and cache size.
2. Download a specified block data of the whole file. 
3. Start, Pause, Resume and Cancel a download.  
4. Supply the file size, download speed and used time.
5. Expose the events StatusChanged, DownloadProgressChanged and DownloadCompleted.

NOTE: To enable the Feature2 and Feature3, the server must support the http
      "Accept-Ranges" header. 
   
////////////////////////////////////////////////////////////////////////////////
Demo:
Step1. Build the sample project in Visual Studio 2010.

Step2. Run CSWebDownloader.exe

Step3. Type following link as url.
http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe

Step4. Type a local path like D:\DotNetFx4.exe.

Step5. Click the button "Download", you will see the status "Downloading" and the summary 
"Received: ***KB, Total: ***KB, Speed: ***KB/s", the progressbar will also change.
       
	   In Windows Explorer, you will find a file  D:\DotNetFx4.exe.tmp.

Step6. Click the button "Pause", you will see the status "Paused" and the summary 
"Received: ***KB, Total: ***KB, Time: ***", the progressbar will also stop.
       
       If the server does not support "Accept-Ranges" header, the "Pause" button is
       disabled. 

Step7. Click the button "Resume", you will see the status "Downloading" and the summary 
"Received: ***KB, Total: ***KB, Speed: ***KB/s", the progressbar will also change.

Step8. When the download completes, you will see the status "Completed" and the summary 
"Received: ***KB, Total: ***KB, Time: ***", the progressbar will also stop.
       
	   In Windows Explorer, you will find a file  D:\DotNetFx4.exe.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. When a HttpDownloadClient object is created, initialize the fields/properties
   StartPoint, EndPoint, BufferSize, MaxCacheSize, BufferCountPerNotification, Url
   DownloadPath and Status.

2. When the download starts, check whether the destination file exists. If not, create
   a local file with the same size as the file to be downloaded, then download 
   the file in a background thread.

3. The download thread will read a buffer of bytes from the response stream, and store the
   buffer to a MemoryStream cache first. If the cache is full, or the download is paused, 
   canceled or completed, write the data in cache to local file.

4. Raise the event DownloadProgressChanged when read a specified number of buffers.

5. If the download is paused, store the downloaded size. When it is resumed, start to 
   download the file from a start point.

6. Update the used time and status when the current download stops.

7. Raise the event DownloadCompleted when the download is completed or canceled.


/////////////////////////////////////////////////////////////////////////////
References:

HttpWebRequest Class 
http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx
AddRange Method 
http://msdn.microsoft.com/en-us/library/7fy67z6d.aspx
How can i check if file download completed
http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/e115d4a1-5800-4f2a-98d8-079de6cf8a1a
/////////////////////////////////////////////////////////////////////////////
