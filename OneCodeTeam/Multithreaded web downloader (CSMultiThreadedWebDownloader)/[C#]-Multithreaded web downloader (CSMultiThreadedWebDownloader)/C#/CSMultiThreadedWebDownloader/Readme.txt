================================================================================
	            Windows APPLICATION: CSMultiThreadedWebDownloader                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to download a file on Web using multiple threads.

The class System.Net.WebClient has a DownloadProgressChanged event, and you can register
this event to show the progress. But this class does not support Pause/Resume.

The class MultiThreadedWebDownloader in this sample could be used to download data through 
internet and supports following features:
1. Download the whole file using multiple threads. 
2. Set the proxy.
3. Set the buffer and cache size.
4. Start, Pause, Resume and Cancel a download.  
5. Supply the file size, download speed and used time.
6. Expose the events StatusChanged, DownloadProgressChanged and DownloadCompleted.

NOTE:

1. To pause and resume a download, or download a file using multi-threads, the server 
   must support "Accept-Ranges" header, so that we can add range to the webrequset
   to download a specified block of the file.

2. If you are trying to download a single file using multiple threads/ multiple 
   HttpWebRequest’s, multiple requests (each asking for a different range) will all 
   be pipelined over a single connection (not necessarily all). You have to take into
   consideration as to how many threads you are spinning, and how many max Connections
   you have set for the ServicePointManager. By default, ServicePointManager has a 
   default of 2 max connections for console applications, so only 2 requests can be 
   active at a time and will be downloading 2 parts of the large file. So yes, your 
   approach will speed up the download, but you also need to take into account the 
   maximum number of connections. If you increase the number of available connections,
   you will get a better download rate. But, do not increase it to a very large value, 
   otherwise you will end up spiking the CPU due to the overhead of creating multiple
   connections simultaneously. Try to limit the max Conn number = 12 * # CPU which 
   asp.net uses. 
 
   
////////////////////////////////////////////////////////////////////////////////
Demo:
Step1. Build the sample project in Visual Studio 2010.

Step2. Run CSMultiThreadedWebDownloader.exe. 

Step3. Type following link as url.
http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe

Step4. Click the button "Check Url". Then the textbox "Url" and the button "Check Url" 
       will be disabled, the textbox "Local path" and the button "Download" will be eabled. 

Step5. Type a local path like D:\DotNetFx4.exe.

Step6. Click the button "Download", you will see the status "Downloading" and the summary 
       "Received: ***KB, Total: ***KB, Speed: ***KB/s Thread: *", the progressbar will
	   also change.
       
	   In Windows Explorer, you will find a file  D:\DotNetFx4.exe.tmp.

Step7. Click the button "Pause", you will see the status "Paused" and the summary 
       "Received: ***KB, Total: ***KB, Time: ***", the progressbar will also stop.

Step8. Click the button "Resume", you will see the status "Downloading" and the summary 
       "Received: ***KB, Total: ***KB, Speed: ***KB/s", the progressbar will also change.

Step9. When the download completes, you will see the status "Completed" and the summary 
       "Received: ***KB, Total: ***KB, Time: ***", the progressbar will also stop.
       
	   In Windows Explorer, you will find a file  D:\DotNetFx4.exe.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. When the application starts, it loads the proxy settings in app.config. 

2. Before downloading the file, MultiThreadedWebDownloader has to check whether
   2.1 The file exists.
   2.2 The remote server supports "Accept-Ranges" header.
   2.3 The size of the file is more than 0.

3. If the check is passed, then user can specify the local path and start the download.
   
   When the download starts, check whether the destination file exists. If not, create
   a local file with the same size as the file to be downloaded.

   If the remote server does not support "Accept-Ranges" header, the downloader will
   create just one HttpDownloadClient to download the file in a single thread. Else, the 
   downloader will create multiple HttpDownloadClients to download the file in multiple
   threads.
 
4. When a HttpDownloadClient object is created, initialize the fields/properties
   StartPoint, EndPoint, BufferSize, MaxCacheSize, BufferCountPerNotification, Url
   DownloadPath and Status.

5. Each download thread will read a buffer of bytes from the response stream, and store the
   buffer to a MemoryStream cache first. If the cache is full, or the download is paused, 
   canceled or completed, write the data in cache to local file.

6. Raise the event DownloadProgressChanged when read a specified number of buffers.

7. If the download is paused, each HttpDownloadClient will store the downloaded size. When
   it is resumed, start to download the file from a start point.

8. Update the used time and status when the current download stops.

9. Raise the event DownloadCompleted when the download is completed or canceled.


/////////////////////////////////////////////////////////////////////////////
References:

system.net.webrequest class
http://msdn.microsoft.com/en-us/library/system.net.webrequest.aspx

How can i check if file download completed 
http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/e115d4a1-5800-4f2a-98d8-079de6cf8a1a
/////////////////////////////////////////////////////////////////////////////
