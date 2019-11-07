# Multithreaded web downloader (CSMultiThreadedWebDownloader)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows General
- Network
## Topics
- HTTP Download
## Updated
- 08/08/2011
## Description

<p style="font-family:Courier New"></p>
<h2>Windows APPLICATION: CSMultiThreadedWebDownloader </h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New">The sample demonstrates how to download a file on Web using multiple threads.<br>
<br>
The class System.Net.WebClient has a DownloadProgressChanged event, and you can register<br>
this event to show the progress. But this class does not support Pause/Resume.<br>
<br>
The class MultiThreadedWebDownloader in this sample could be used to download data through
<br>
internet and supports following features:<br>
1. Download the whole file using multiple threads. <br>
2. Set the proxy.<br>
3. Set the buffer and cache size.<br>
4. Start, Pause, Resume and Cancel a download. &nbsp;<br>
5. Supply the file size, download speed and used time.<br>
6. Expose the events StatusChanged, DownloadProgressChanged and DownloadCompleted.<br>
<br>
NOTE:<br>
<br>
1. To pause and resume a download, or download a file using multi-threads, the server
<br>
&nbsp; must support &quot;Accept-Ranges&quot; header, so that we can add range to the webrequset<br>
&nbsp; to download a specified block of the file.<br>
<br>
2. If you are trying to download a single file using multiple threads/ multiple <br>
&nbsp; HttpWebRequest’s, multiple requests (each asking for a different range) will all
<br>
&nbsp; be pipelined over a single connection (not necessarily all). You have to take into<br>
&nbsp; consideration as to how many threads you are spinning, and how many max Connections<br>
&nbsp; you have set for the ServicePointManager. By default, ServicePointManager has a
<br>
&nbsp; default of 2 max connections for console applications, so only 2 requests can be
<br>
&nbsp; active at a time and will be downloading 2 parts of the large file. So yes, your
<br>
&nbsp; approach will speed up the download, but you also need to take into account the
<br>
&nbsp; maximum number of connections. If you increase the number of available connections,<br>
&nbsp; you will get a better download rate. But, do not increase it to a very large value,
<br>
&nbsp; otherwise you will end up spiking the CPU due to the overhead of creating multiple<br>
&nbsp; connections simultaneously. Try to limit the max Conn number = 12 * # CPU which
<br>
&nbsp; asp.net uses. <br>
<br>
&nbsp; </p>
<h3>Demo:</h3>
<p style="font-family:Courier New">Step1. Build the sample project in Visual Studio 2010.<br>
<br>
Step2. Run CSMultiThreadedWebDownloader.exe. <br>
<br>
Step3. Type following link as url.<br>
<a target="_blank" href="http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe">http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe</a><br>
<br>
Step4. Click the button &quot;Check Url&quot;. Then the textbox &quot;Url&quot; and the button &quot;Check Url&quot;
<br>
&nbsp; &nbsp; &nbsp; will be disabled, the textbox &quot;Local path&quot; and the button &quot;Download&quot; will be eabled.
<br>
<br>
Step5. Type a local path like D:\DotNetFx4.exe.<br>
<br>
Step6. Click the button &quot;Download&quot;, you will see the status &quot;Downloading&quot; and the summary
<br>
&nbsp; &nbsp; &nbsp; &quot;Received: ***KB, Total: ***KB, Speed: ***KB/s Thread: *&quot;, the progressbar will<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; also change.<br>
&nbsp; &nbsp; &nbsp; <br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; In Windows Explorer, you will find a file &nbsp;D:\DotNetFx4.exe.tmp.<br>
<br>
Step7. Click the button &quot;Pause&quot;, you will see the status &quot;Paused&quot; and the summary
<br>
&nbsp; &nbsp; &nbsp; &quot;Received: ***KB, Total: ***KB, Time: ***&quot;, the progressbar will also stop.<br>
<br>
Step8. Click the button &quot;Resume&quot;, you will see the status &quot;Downloading&quot; and the summary
<br>
&nbsp; &nbsp; &nbsp; &quot;Received: ***KB, Total: ***KB, Speed: ***KB/s&quot;, the progressbar will also change.<br>
<br>
Step9. When the download completes, you will see the status &quot;Completed&quot; and the summary
<br>
&nbsp; &nbsp; &nbsp; &quot;Received: ***KB, Total: ***KB, Time: ***&quot;, the progressbar will also stop.<br>
&nbsp; &nbsp; &nbsp; <br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; In Windows Explorer, you will find a file &nbsp;D:\DotNetFx4.exe.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. When the application starts, it loads the proxy settings in app.config. <br>
<br>
2. Before downloading the file, MultiThreadedWebDownloader has to check whether<br>
&nbsp; 2.1 The file exists.<br>
&nbsp; 2.2 The remote server supports &quot;Accept-Ranges&quot; header.<br>
&nbsp; 2.3 The size of the file is more than 0.<br>
<br>
3. If the check is passed, then user can specify the local path and start the download.<br>
&nbsp; <br>
&nbsp; When the download starts, check whether the destination file exists. If not, create<br>
&nbsp; a local file with the same size as the file to be downloaded.<br>
<br>
&nbsp; If the remote server does not support &quot;Accept-Ranges&quot; header, the downloader will<br>
&nbsp; create just one HttpDownloadClient to download the file in a single thread. Else, the
<br>
&nbsp; downloader will create multiple HttpDownloadClients to download the file in multiple<br>
&nbsp; threads.<br>
<br>
4. When a HttpDownloadClient object is created, initialize the fields/properties<br>
&nbsp; StartPoint, EndPoint, BufferSize, MaxCacheSize, BufferCountPerNotification, Url<br>
&nbsp; DownloadPath and Status.<br>
<br>
5. Each download thread will read a buffer of bytes from the response stream, and store the<br>
&nbsp; buffer to a MemoryStream cache first. If the cache is full, or the download is paused,
<br>
&nbsp; canceled or completed, write the data in cache to local file.<br>
<br>
6. Raise the event DownloadProgressChanged when read a specified number of buffers.<br>
<br>
7. If the download is paused, each HttpDownloadClient will store the downloaded size. When<br>
&nbsp; it is resumed, start to download the file from a start point.<br>
<br>
8. Update the used time and status when the current download stops.<br>
<br>
9. Raise the event DownloadCompleted when the download is completed or canceled.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
system.net.webrequest class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.net.webrequest.aspx">http://msdn.microsoft.com/en-us/library/system.net.webrequest.aspx</a><br>
<br>
How can i check if file download completed <br>
<a target="_blank" href="http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/e115d4a1-5800-4f2a-98d8-079de6cf8a1a">http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/e115d4a1-5800-4f2a-98d8-079de6cf8a1a</a><br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
