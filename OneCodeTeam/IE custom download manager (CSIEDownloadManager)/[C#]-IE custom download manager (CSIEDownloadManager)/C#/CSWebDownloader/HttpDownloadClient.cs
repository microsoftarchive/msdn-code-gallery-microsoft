/****************************** Module Header ******************************\
 * Module Name:  HttpDownloadClient.cs
 * Project:      CSWebDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * This class is used to download files through internet.  It supplies public
 * methods to Start, Pause, Resume and Cancel a download. 
 * 
 * When the download starts, it will check whether the destination file exists. If
 * not, create a file with the same size as the file to be downloaded, then
 * download the file in a background thread.
 * 
 * The downloaded data is stored in a MemoryStream first, and then written to local
 * file.
 * 
 * It will fire a DownloadProgressChanged event when it has downloaded a
 * specified size of data. It will also fire a DownloadCompleted event if the 
 * download is completed or canceled.
 * 
 * The property DownloadedSize stores the size of downloaded data which will be 
 * used to Resume the download.
 * 
 * The property StartPoint can be used in the multi-thread download scenario, and
 * every thread starts to download a specific block of the whole file. 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSWebDownloader
{
    public class HttpDownloadClient
    {
        // Used when creates or writes a file.
        static object locker = new object();

        // Store the used time spent in downloading data. The value does not include
        // the paused time and it will only be updated when the download is paused, 
        // canceled or completed.
        private TimeSpan usedTime = new TimeSpan();

        private DateTime lastStartTime;

        /// <summary>
        /// If the status is Downloading, then the total time is usedTime. Else the total 
        /// should include the time used in current download thread.
        /// </summary>
        public TimeSpan TotalUsedTime
        {
            get
            {
                if (this.Status != HttpDownloadClientStatus.Downloading)
                {
                    return usedTime;
                }
                else
                {
                    return usedTime.Add(DateTime.Now - lastStartTime);
                }
            }
        }

        // The time and size in last DownloadProgressChanged event. These two fields
        // are used to calculate the download speed.
        private DateTime lastNotificationTime;
        private Int64 lastNotificationDownloadedSize;

        // If get a number of buffers, then fire DownloadProgressChanged event.
        public int BufferCountPerNotification { get; private set; }

        // The Url of the file to be downloaded.
        public Uri Url { get; private set; }

        /// <summary>
        /// Specify whether the remote server supports "Accept-Ranges" header.
        /// </summary>
        public bool IsRangeSupported { get; private set; }


        // The local path to store the file.
        // If there is no file with the same name, a new file will be created.
        public string DownloadPath { get; set; }

        // The properties StartPoint and EndPoint can be used in the multi-thread download scenario, and
        // every thread starts to download a specific block of the whole file. 
        public int StartPoint { get; private set; }

        public int EndPoint { get; private set; }

        // Set the BufferSize when read data in Response Stream.
        public int BufferSize { get; private set; }

        // The cache size in memory.
        public int MaxCacheSize { get; private set; }

        // Ask the server for the file size and store it
        public Int64 TotalSize { get; private set; }


        // The size of downloaded data that has been writen to local file.
        public Int64 DownloadedSize { get; private set; }

        HttpDownloadClientStatus status;

        // If status changed, fire StatusChanged event.
        public HttpDownloadClientStatus Status
        {
            get
            { return status; }

            private set
            {
                if (status != value)
                {
                    status = value;
                    this.OnStatusChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler<HttpDownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<HttpDownloadCompletedEventArgs> DownloadCompleted;

        public event EventHandler StatusChanged;

        /// <summary>
        /// Download the whole file.
        /// </summary>
        public HttpDownloadClient(string url)
            : this(url, 0)
        {
        }

        /// <summary>
        /// Download the file from a start point to the end.
        /// </summary>
        public HttpDownloadClient(string url, int startPoint)
            : this(url, startPoint, int.MaxValue)
        {
        }

        /// <summary>
        /// Download a block of the file. The default buffer size is 1KB, memory cache is
        /// 1MB, and buffer count per notification is 64.
        /// </summary>
        public HttpDownloadClient(string url, int startPoint, int endPoint)
            : this(url, startPoint, endPoint, 1024, 1048576, 64)
        {
        }

        public HttpDownloadClient(string url, int startPoint,
            int endPoint, int bufferSize, int cacheSize, int bufferCountPerNotification)
        {
            if (startPoint < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "StartPoint cannot be less then 0. ");
            }

            if (endPoint < startPoint)
            {
                throw new ArgumentOutOfRangeException(
                    "EndPoint cannot be less then StartPoint ");
            }

            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferSize cannot be less then 0. ");
            }

            if (cacheSize < bufferSize)
            {
                throw new ArgumentOutOfRangeException(
                    "MaxCacheSize cannot be less then BufferSize ");
            }

            if (bufferCountPerNotification <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferCountPerNotification cannot be less then 0. ");
            }

            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.BufferSize = bufferSize;
            this.MaxCacheSize = cacheSize;
            this.BufferCountPerNotification = bufferCountPerNotification;

            this.Url = new Uri(url, UriKind.Absolute);

            // Set the idle status.
            this.status = HttpDownloadClientStatus.Idle;

        }


        public void CheckUrl(out string fileName)
        {
            fileName = string.Empty;

            // The file could be checked only in Idle status.
            if (this.status != HttpDownloadClientStatus.Idle)
            {
                throw new ApplicationException(
                    "The file could be checked only in Idle status.");
            }

            // Check the file information on the remote server.
            var webRequest = (HttpWebRequest)WebRequest.Create(Url);

            webRequest.Credentials = CredentialCache.DefaultCredentials;

            using (var response = webRequest.GetResponse())
            {
                foreach (var header in response.Headers.AllKeys)
                {
                    if (header.Equals("Accept-Ranges", StringComparison.OrdinalIgnoreCase))
                    {
                        this.IsRangeSupported = true;
                    }

                    if (header.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase))
                    {
                        string contentDisposition = response.Headers[header];
                        
                        string pattern = ".[^;]*;\\s+filename=\"(?<file>.*)\"";
                        Regex r = new Regex(pattern);
                        Match m = r.Match(contentDisposition);
                        if (m.Success)
                        {
                            fileName =  m.Groups["file"].Value;
                        }
                    }
                }

                this.TotalSize = response.ContentLength;

                if (TotalSize <= 0)
                {
                    throw new ApplicationException(
                        "The file to download does not exist!");
                }

                if (!IsRangeSupported)
                {
                    this.StartPoint = 0;
                    this.EndPoint = int.MaxValue;
                }
            }

            if (this.IsRangeSupported &&
                (this.StartPoint != 0 || this.EndPoint != int.MaxValue))
            {
                webRequest = (HttpWebRequest)WebRequest.Create(Url);
                if (EndPoint != int.MaxValue)
                {
                    webRequest.AddRange(StartPoint, EndPoint);
                }
                else
                {
                    webRequest.AddRange(StartPoint);
                }
                using (var response = webRequest.GetResponse())
                {
                    this.TotalSize = response.ContentLength;
                }
            }

            // Set the checked status.
            this.Status = HttpDownloadClientStatus.Checked;
        }

        /// <summary>
        /// Start to download.
        /// </summary>
        public void Start()
        {

            // Check whether the destination file exist.
            CheckFileOrCreateFile();



            // Only idle download client can be started.
            if (this.Status != HttpDownloadClientStatus.Checked)
            {
                throw new ApplicationException(
                     "Only Checked downloader can be started. ");
            }

            // Start to download in a background thread.
            BeginDownload();
        }

        /// <summary>
        /// Pause the download.
        /// </summary>
        public void Pause()
        {
            // Only downloading client can be paused.
            if (this.Status != HttpDownloadClientStatus.Downloading)
            {
                throw new ApplicationException("Only downloading client can be paused.");
            }

            // The backgound thread will check the status. If it is Pausing, the download
            // will be paused and the status will be changed to Paused.
            this.Status = HttpDownloadClientStatus.Pausing;
        }

        /// <summary>
        /// Resume the download.
        /// </summary>
        public void Resume()
        {
            // Only paused client can be resumed.
            if (this.Status != HttpDownloadClientStatus.Paused)
            {
                throw new ApplicationException("Only paused client can be resumed.");
            }

            // Start to download in a background thread.
            BeginDownload();
        }

        /// <summary>
        /// Cancel the download
        /// </summary>
        public void Cancel()
        {
            // Only a downloading or paused client can be canceled.
            if (this.Status != HttpDownloadClientStatus.Paused
                && this.Status != HttpDownloadClientStatus.Downloading)
            {
                throw new ApplicationException("Only a downloading or paused client"
                    + " can be canceled.");
            }

            // The backgound thread will check the status. If it is Canceling, the download
            // will be canceled and the status will be changed to Canceled.
            this.Status = HttpDownloadClientStatus.Canceling;
        }

        /// <summary>
        /// Create a thread to download data.
        /// </summary>
        void BeginDownload()
        {
            ThreadStart threadStart = new ThreadStart(Download);
            Thread downloadThread = new Thread(threadStart);
            downloadThread.IsBackground = true;
            downloadThread.Start();
        }

        /// <summary>
        /// Download the data using HttpWebRequest. It will read a buffer of bytes from the
        /// response stream, and store the buffer to a MemoryStream cache first.
        /// If the cache is full, or the download is paused, canceled or completed, write
        /// the data in cache to local file.
        /// </summary>
        void Download()
        {
            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            Stream responseStream = null;
            MemoryStream downloadCache = null;
            lastStartTime = DateTime.Now;

            // Set the status.
            this.Status = HttpDownloadClientStatus.Downloading;

            try
            {

                // Create a request to the file to be  downloaded.
                webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.Method = "GET";
                webRequest.Credentials = CredentialCache.DefaultCredentials;


                // Specify the block to download.
                if (EndPoint != int.MaxValue)
                {
                    webRequest.AddRange(StartPoint + DownloadedSize, EndPoint);
                }
                else
                {
                    webRequest.AddRange(StartPoint + DownloadedSize);
                }

                // Retrieve the response from the server and get the response stream.
                webResponse = (HttpWebResponse)webRequest.GetResponse();

                responseStream = webResponse.GetResponseStream();


                // Cache data in memory.
                downloadCache = new MemoryStream(this.MaxCacheSize);

                byte[] downloadBuffer = new byte[this.BufferSize];

                int bytesSize = 0;
                int cachedSize = 0;
                int receivedBufferCount = 0;

                // Download the file until the download is paused, canceled or completed.
                while (true)
                {

                    // Read a buffer of data from the stream.
                    bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length);

                    // If the cache is full, or the download is paused, canceled or 
                    // completed, write the data in cache to local file.
                    if (this.Status != HttpDownloadClientStatus.Downloading
                        || bytesSize == 0
                        || this.MaxCacheSize < cachedSize + bytesSize)
                    {

                        try
                        {
                            // Write the data in cache to local file.
                            WriteCacheToFile(downloadCache, cachedSize);

                            this.DownloadedSize += cachedSize;

                            // Stop downloading the file if the download is paused, 
                            // canceled or completed. 
                            if (this.Status != HttpDownloadClientStatus.Downloading
                                || bytesSize == 0)
                            {
                                break;
                            }

                            // Reset cache.
                            downloadCache.Seek(0, SeekOrigin.Begin);
                            cachedSize = 0;
                        }
                        catch (Exception ex)
                        {
                            // Fire the DownloadCompleted event with the error.
                            this.OnDownloadCompleted(new HttpDownloadCompletedEventArgs(
                                               this.DownloadedSize, this.TotalSize,
                                               this.TotalUsedTime, ex));
                            return;
                        }

                    }


                    // Write the data from the buffer to the cache in memory.
                    downloadCache.Write(downloadBuffer, 0, bytesSize);

                    cachedSize += bytesSize;

                    receivedBufferCount++;

                    // Fire the DownloadProgressChanged event.
                    if (receivedBufferCount == this.BufferCountPerNotification)
                    {
                        InternalDownloadProgressChanged(cachedSize);
                        receivedBufferCount = 0;
                    }
                }


                // Update the used time when the current doanload is stopped.
                usedTime = usedTime.Add(DateTime.Now - lastStartTime);

                // Update the status of the client. Above loop will be stopped when the 
                // status of the client is pausing, canceling or completed.
                if (this.Status == HttpDownloadClientStatus.Pausing)
                {
                    this.Status = HttpDownloadClientStatus.Paused;
                }
                else if (this.Status == HttpDownloadClientStatus.Canceling)
                {
                    this.Status = HttpDownloadClientStatus.Canceled;

                    Exception ex = new Exception("Downloading is canceled by user's request. ");

                    this.OnDownloadCompleted(new HttpDownloadCompletedEventArgs(
                                   this.DownloadedSize, this.TotalSize, this.TotalUsedTime, ex));
                }
                else
                {
                    this.Status = HttpDownloadClientStatus.Completed;
                    this.OnDownloadCompleted(new HttpDownloadCompletedEventArgs(
                                this.DownloadedSize, this.TotalSize, this.TotalUsedTime, null));
                    return;
                }

            }
            finally
            {
                // When the above code has ended, close the streams.
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (webResponse != null)
                {
                    webResponse.Close();
                }
                if (downloadCache != null)
                {
                    downloadCache.Close();
                }
            }
        }



        /// <summary>
        /// Check whether the destination file exists. If  not, create a file with the same
        /// size as the file to be downloaded.
        /// </summary>
        void CheckFileOrCreateFile()
        {
            // Lock other threads or processes to prevent from creating the file.
            lock (locker)
            {
                FileInfo fileToDownload = new FileInfo(DownloadPath);
                if (fileToDownload.Exists)
                {

                    // The destination file should have the same size as the file to be downloaded.
                    if (fileToDownload.Length != this.TotalSize)
                    {
                        throw new ApplicationException(
                            "The download path already has a file which does not match"
                            + " the file to download. ");
                    }
                }

                // Create a file.
                else
                {
                    if (TotalSize == 0)
                    {
                        throw new ApplicationException("The file to download does not exist!");
                    }

                    using (FileStream fileStream = File.Create(this.DownloadPath))
                    {
                        long createdSize = 0;
                        byte[] buffer = new byte[4096];
                        while (createdSize < TotalSize)
                        {
                            int bufferSize = (TotalSize - createdSize) < 4096 ? (int)(TotalSize - createdSize) : 4096;
                            fileStream.Write(buffer, 0, bufferSize);
                            createdSize += bufferSize;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Write the data in cache to local file.
        /// </summary>
        void WriteCacheToFile(MemoryStream downloadCache, int cachedSize)
        {
            // Lock other threads or processes to prevent from writing data to the file.
            lock (locker)
            {
                using (FileStream fileStream = new FileStream(DownloadPath, FileMode.Open))
                {
                    byte[] cacheContent = new byte[cachedSize];
                    downloadCache.Seek(0, SeekOrigin.Begin);
                    downloadCache.Read(cacheContent, 0, cachedSize);
                    fileStream.Seek(DownloadedSize + StartPoint, SeekOrigin.Begin);
                    fileStream.Write(cacheContent, 0, cachedSize);
                }
            }
        }


        protected virtual void OnDownloadCompleted(HttpDownloadCompletedEventArgs e)
        {
            if (DownloadCompleted != null)
            {
                DownloadCompleted(this, e);
            }
        }

        /// <summary>
        /// Calculate the download speed and fire the  DownloadProgressChanged event.
        /// </summary>
        /// <param name="cachedSize"></param>
        private void InternalDownloadProgressChanged(int cachedSize)
        {
            int speed = 0;
            DateTime current = DateTime.Now;
            TimeSpan interval = current - lastNotificationTime;

            if (interval.TotalSeconds < 60)
            {
                speed = (int)Math.Floor((this.DownloadedSize + cachedSize - this.lastNotificationDownloadedSize) / interval.TotalSeconds);
            }
            lastNotificationTime = current;
            lastNotificationDownloadedSize = this.DownloadedSize + cachedSize;

            this.OnDownloadProgressChanged(new HttpDownloadProgressChangedEventArgs
                           (this.DownloadedSize + cachedSize, this.TotalSize, speed));


        }

        protected virtual void OnDownloadProgressChanged(HttpDownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(this, e);
            }
        }

        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, e);
            }
        }
    }
}
