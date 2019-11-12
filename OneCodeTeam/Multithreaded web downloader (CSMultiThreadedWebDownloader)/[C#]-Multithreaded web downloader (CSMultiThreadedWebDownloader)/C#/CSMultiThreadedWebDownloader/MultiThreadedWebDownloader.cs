/****************************** Module Header ******************************\
 * Module Name:  MultiThreadedWebDownloader.cs
 * Project:      CSMultiThreadedWebDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * This class is used to download files through internet using multiple threads. 
 * It supplies public  methods to Start, Pause, Resume and Cancel a download. 
 * 
 * Before the download starts, the remote server should be checked 
 * whether it supports "Accept-Ranges" header.
 * 
 * When the download starts, it will check whether the destination file exists. If
 * not, create a file with the same size as the file to be downloaded, then
 * creates multiple HttpDownloadClients to download the file in background threads.
 * 
 * It will fire a DownloadProgressChanged event when it has downloaded a
 * specified size of data. It will also fire a DownloadCompleted event if the 
 * download is completed or canceled.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace CSMultiThreadedWebDownloader
{
    public class MultiThreadedWebDownloader : IDownloader
    {
        // Used while calculating download speed.
        static object locker = new object();


        /// <summary>
        /// The Url of the file to be downloaded. 
        /// </summary>
        public Uri Url { get; private set; }

        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Specify whether the remote server supports "Accept-Ranges" header.
        /// Use the CheckUrl method to initialize this property internally.
        /// </summary>
        public bool IsRangeSupported { get; set; }

        /// <summary>
        /// The total size of the file. Use the CheckUrl method to initialize this 
        /// property internally.
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// This property is a member of IDownloader interface.
        /// </summary>
        public long StartPoint { get; set; }

        /// <summary>
        /// This property is a member of IDownloader interface.
        /// </summary>
        public long EndPoint { get; set; }

        /// <summary>
        /// The local path to store the file.
        /// If there is no file with the same name, a new file will be created.
        /// </summary>
        public string DownloadPath { get; set; }


        /// <summary>
        /// The Proxy of all the download client.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// The downloaded size of the file.
        /// </summary>
        public long DownloadedSize
        {
            get
            {
                return this.downloadClients.Sum(client => client.DownloadedSize);
            }
        }

        public int CachedSize
        {
            get
            {
                return this.downloadClients.Sum(client=>client.CachedSize);
            }
        }

        // Store the used time spent in downloading data. The value does not include
        // the paused time and it will only be updated when the download is paused, 
        // canceled or completed.
        private TimeSpan usedTime = new TimeSpan();

        private DateTime lastStartTime;

        /// <summary>
        /// If the status is Downloading, then the total time is usedTime. Else the 
        /// total should include the time used in current download thread.
        /// </summary>
        public TimeSpan TotalUsedTime
        {
            get
            {
                if (this.Status != DownloadStatus.Downloading)
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

        private long lastNotificationDownloadedSize;

        /// <summary>
        /// If get a number of buffers, then fire DownloadProgressChanged event.
        /// </summary>
        public int BufferCountPerNotification { get; set; }

        /// <summary>
        /// Set the BufferSize when read data in Response Stream.
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// The cache size in memory.
        /// </summary>
        public int MaxCacheSize { get; set; }

        DownloadStatus status;

        /// <summary>
        /// If status changed, fire StatusChanged event.
        /// </summary>
        public DownloadStatus Status
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

        /// <summary>
        /// The max threads count. The real threads count number is the min value of this
        /// value and TotalSize / MaxCacheSize.
        /// </summary>
        public int MaxThreadCount { get; set; }

        public bool HasChecked { get; set; }

        // The HttpDownloadClients to download the file. Each client uses one thread to
        // download part of the file.
        List<HttpDownloadClient> downloadClients = null;

        public int DownloadThreadsCount
        {
            get
            {
                if (downloadClients != null)
                {
                    return downloadClients.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        public event EventHandler StatusChanged;

        /// <summary>
        /// Download the whole file. The default buffer size is 1KB, memory cache is
        /// 1MB, buffer count per notification is 64, threads count is the double of 
        /// logic processors count.
        /// </summary>
        public MultiThreadedWebDownloader(string url)
            : this(url, 1024, 1048576, 64, Environment.ProcessorCount * 2)
        {
        }

        public MultiThreadedWebDownloader(string url, int bufferSize, int cacheSize,
            int bufferCountPerNotification, int maxThreadCount)
        {          

            this.Url = new Uri(url);
            this.StartPoint = 0;
            this.EndPoint = long.MaxValue;
            this.BufferSize = bufferSize;
            this.MaxCacheSize = cacheSize;
            this.BufferCountPerNotification = bufferCountPerNotification;

            this.MaxThreadCount = maxThreadCount;

            // Set the maximum number of concurrent connections allowed by 
            // a ServicePoint object
            ServicePointManager.DefaultConnectionLimit = maxThreadCount;

            // Initialize the HttpDownloadClient list.
            downloadClients = new List<HttpDownloadClient>();

            // Set the Initialized status.
            this.Status = DownloadStatus.Initialized;
        }


        public void CheckUrlAndFile(out string fileName)
        {
            CheckUrl(out fileName);
            CheckFileOrCreateFile();

            this.HasChecked = true;
        }

        /// <summary>
        /// Check the Uri to find its size, and whether it supports "Pause". 
        /// </summary>
        public void CheckUrl(out string fileName)
        {
            fileName = DownloaderHelper.CheckUrl(this);           
        }

        /// <summary>
        /// Check whether the destination file exists. If  not, create a file with the same
        /// size as the file to be downloaded.
        /// </summary>
        void CheckFileOrCreateFile()
        {
            DownloaderHelper.CheckFileOrCreateFile(this, locker);          
        }

        void EnsurePropertyValid()
        {
            if (this.StartPoint < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "StartPoint cannot be less then 0. ");
            }

            if (this.EndPoint < this.StartPoint)
            {
                throw new ArgumentOutOfRangeException(
                    "EndPoint cannot be less then StartPoint ");
            }

            if (this.BufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferSize cannot be less then 0. ");
            }

            if (this.MaxCacheSize < this.BufferSize)
            {
                throw new ArgumentOutOfRangeException(
                    "MaxCacheSize cannot be less then BufferSize ");
            }

            if (this.BufferCountPerNotification <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferCountPerNotification cannot be less then 0. ");
            }

            if (this.MaxThreadCount < 1)
            {
                throw new ArgumentOutOfRangeException(
                       "maxThreadCount cannot be less than 1. ");
            }
        }

        /// <summary>
        /// Start to download.
        /// </summary>
        public void Download()
        {

            // Only idle download client can be started.
            if (this.Status != DownloadStatus.Initialized)
            {
                throw new ApplicationException(
                    "Only Initialized download client can be started.");
            }

            this.EnsurePropertyValid();

            // Set the status.
            this.Status = DownloadStatus.Downloading;

            if (!this.HasChecked)
            {
                string filename = null;
                this.CheckUrlAndFile(out filename);
            }

            HttpDownloadClient client = new HttpDownloadClient(
                    this.Url.AbsoluteUri,
                    0,
                    long.MaxValue,
                    this.BufferSize,
                    this.BufferCountPerNotification * this.BufferSize,
                    this.BufferCountPerNotification);

            // Set the HasChecked flag, so the client will not check the URL again.
            client.TotalSize = this.TotalSize;
            client.DownloadPath = this.DownloadPath;
            client.HasChecked = true;
            client.Credentials = this.Credentials;
            client.Proxy = this.Proxy;

            // Register the events of HttpDownloadClients.
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.StatusChanged += client_StatusChanged;
            client.DownloadCompleted += client_DownloadCompleted;

            this.downloadClients.Add(client);
            client.Download();
        }



        /// <summary>
        /// Start to download.
        /// </summary>
        public void BeginDownload()
        {

            // Only idle download client can be started.
            if (this.Status != DownloadStatus.Initialized)
            {
                throw new ApplicationException("Only Initialized download client can be started.");
            }

            this.Status = DownloadStatus.Waiting;

            ThreadPool.QueueUserWorkItem(DownloadInternal);
        }

        void DownloadInternal(object obj)
        {

            if (this.Status != DownloadStatus.Waiting)
            {
                return;
            }

            try
            {
                this.EnsurePropertyValid();

                // Set the status.
                this.Status = DownloadStatus.Downloading;

                if (!this.HasChecked)
                {
                    string filename = null;
                    this.CheckUrlAndFile(out filename);
                }



                // If the file does not support "Accept-Ranges" header, then create one 
                // HttpDownloadClient to download the file in a single thread, else create
                // multiple HttpDownloadClients to download the file.
                if (!IsRangeSupported)
                {
                    HttpDownloadClient client = new HttpDownloadClient(
                        this.Url.AbsoluteUri,
                        0,
                        long.MaxValue,
                        this.BufferSize,
                        this.BufferCountPerNotification * this.BufferSize,
                        this.BufferCountPerNotification);

                    // Set the HasChecked flag, so the client will not check the URL again.
                    client.TotalSize = this.TotalSize;
                    client.DownloadPath = this.DownloadPath;
                    client.HasChecked = true;
                    client.Credentials = this.Credentials;
                    client.Proxy = this.Proxy;

                    this.downloadClients.Add(client);
                }
                else
                {
                    // Calculate the block size for each client to download.
                    int maxSizePerThread =
                        (int)Math.Ceiling((double)this.TotalSize / this.MaxThreadCount);
                    if (maxSizePerThread < this.MaxCacheSize)
                    {
                        maxSizePerThread = this.MaxCacheSize;
                    }

                    long leftSizeToDownload = this.TotalSize;

                    // The real threads count number is the min value of MaxThreadCount and 
                    // TotalSize / MaxCacheSize.              
                    int threadsCount =
                        (int)Math.Ceiling((double)this.TotalSize / maxSizePerThread);

                    for (int i = 0; i < threadsCount; i++)
                    {
                        long endPoint = maxSizePerThread * (i + 1) - 1;
                        long sizeToDownload = maxSizePerThread;

                        if (endPoint > this.TotalSize)
                        {
                            endPoint = this.TotalSize - 1;
                            sizeToDownload = endPoint - maxSizePerThread * i;
                        }

                        // Download a block of the whole file.
                        HttpDownloadClient client = new HttpDownloadClient(
                            this.Url.AbsoluteUri,
                            maxSizePerThread * i,
                            endPoint);

                        // Set the HasChecked flag, so the client will not check the URL again.
                        client.DownloadPath = this.DownloadPath;
                        client.HasChecked = true;
                        client.TotalSize = sizeToDownload;
                        client.Credentials = this.Credentials;
                        client.Proxy = this.Proxy;
                        this.downloadClients.Add(client);
                    }
                }

                // Set the lastStartTime to calculate the used time.
                lastStartTime = DateTime.Now;

                // Start all HttpDownloadClients.
                foreach (var client in this.downloadClients)
                {
                    if (this.Proxy != null)
                    {
                        client.Proxy = this.Proxy;
                    }

                    // Register the events of HttpDownloadClients.
                    client.DownloadProgressChanged += client_DownloadProgressChanged;
                    client.StatusChanged += client_StatusChanged;
                    client.DownloadCompleted += client_DownloadCompleted;


                    client.BeginDownload();
                }
            }
            catch (Exception ex)
            {
                this.Cancel();
                this.OnDownloadCompleted(new DownloadCompletedEventArgs(
                    null,
                    this.DownloadedSize,
                    this.TotalSize,
                    this.TotalUsedTime,
                    ex));
            }
        }

        /// <summary>
        /// Pause the download.
        /// </summary>
        public void Pause()
        {
            // Only downloading downloader can be paused.
            if (this.Status != DownloadStatus.Downloading)
            {
                throw new ApplicationException(
                    "Only downloading downloader can be paused.");
            }

            this.Status = DownloadStatus.Pausing;

            // Pause all the HttpDownloadClients. If all of the clients are paused,
            // the status of the downloader will be changed to Paused.
            foreach (var client in this.downloadClients)
            {
                if (client.Status == DownloadStatus.Downloading)
                {
                    client.Pause();
                }
            }
        }


        /// <summary>
        /// Resume the download.
        /// </summary>
        public void Resume()
        {
            // Only paused downloader can be paused.
            if (this.Status != DownloadStatus.Paused)
            {
                throw new ApplicationException(
                    "Only paused downloader can be resumed. ");
            }

            // Set the lastStartTime to calculate the used time.
            lastStartTime = DateTime.Now;

            // Set the downloading status.
            this.Status = DownloadStatus.Waiting;

            // Resume all HttpDownloadClients.
            foreach (var client in this.downloadClients)
            {
                if (client.Status != DownloadStatus.Completed)
                {
                    client.Resume();
                }
            }
        }

        /// <summary>
        /// Resume the download.
        /// </summary>
        public void BeginResume()
        {
            // Only paused downloader can be paused.
            if (this.Status != DownloadStatus.Paused)
            {
                throw new ApplicationException(
                    "Only paused downloader can be resumed. ");
            }

            // Set the lastStartTime to calculate the used time.
            lastStartTime = DateTime.Now;

            // Set the downloading status.
            this.Status = DownloadStatus.Waiting;

            // Resume all HttpDownloadClients.
            foreach (var client in this.downloadClients)
            {
                if (client.Status != DownloadStatus.Completed)
                {
                    client.BeginResume();
                }
            }

        }

        /// <summary>
        /// Cancel the download
        /// </summary>
        public void Cancel()
        {

            if (this.Status == DownloadStatus.Initialized
                || this.Status == DownloadStatus.Waiting
                || this.Status == DownloadStatus.Completed
                || this.Status == DownloadStatus.Paused
                || this.Status == DownloadStatus.Canceled)
            {
                this.Status = DownloadStatus.Canceled;
            }
            else if (this.Status == DownloadStatus.Canceling
                || this.Status == DownloadStatus.Pausing
                || this.Status == DownloadStatus.Downloading)
            {
                this.Status = DownloadStatus.Canceling;
            }

            // Cancel all HttpDownloadClients.
            foreach (var client in this.downloadClients)
            {
                client.Cancel();
            }

        }

        /// <summary>
        /// Handle the StatusChanged event of all the HttpDownloadClients.
        /// </summary>
        void client_StatusChanged(object sender, EventArgs e)
        {

            // If all the clients are completed, then the status of this downloader is 
            // completed.
            if (this.downloadClients.All(
                client => client.Status == DownloadStatus.Completed))
            {
                this.Status = DownloadStatus.Completed;
            }

            // If all the clients are Canceled, then the status of this downloader is 
            // Canceled.
            else if (this.downloadClients.All(
                client => client.Status == DownloadStatus.Canceled))
            {
                this.Status = DownloadStatus.Canceled;
            }
            else
            {

                // The completed clients will not be taken into consideration.
                var nonCompletedClients = this.downloadClients.
                    Where(client => client.Status != DownloadStatus.Completed);

                // If all the nonCompletedClients are Waiting, then the status of this 
                // downloader is Waiting.
                if (nonCompletedClients.All(
                    client => client.Status == DownloadStatus.Waiting))
                {
                    this.Status = DownloadStatus.Waiting;
                }

                // If all the nonCompletedClients are Paused, then the status of this 
                // downloader is Paused.
                else if (nonCompletedClients.All(
                    client => client.Status == DownloadStatus.Paused))
                {
                    this.Status = DownloadStatus.Paused;
                }
                else if (this.Status != DownloadStatus.Pausing
                    && this.Status != DownloadStatus.Canceling)
                {
                    this.Status = DownloadStatus.Downloading;
                }
            }

        }

        /// <summary>
        /// Handle the DownloadProgressChanged event of all the HttpDownloadClients, and 
        /// calculate the download speed.
        /// </summary>
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (locker)
            {
                if (DownloadProgressChanged != null)
                {
                    int speed = 0;
                    DateTime current = DateTime.Now;
                    TimeSpan interval = current - lastNotificationTime;

                    if (interval.TotalSeconds < 60)
                    {
                        speed = (int)Math.Floor((this.DownloadedSize + this.CachedSize-
                            this.lastNotificationDownloadedSize) / interval.TotalSeconds);
                    }

                    lastNotificationTime = current;
                    lastNotificationDownloadedSize = this.DownloadedSize + this.CachedSize;

                    var downloadProgressChangedEventArgs =
                        new DownloadProgressChangedEventArgs(
                            DownloadedSize, TotalSize, speed);
                    this.OnDownloadProgressChanged(downloadProgressChangedEventArgs);
                }

            }
        }

        /// <summary>
        /// Handle the DownloadCompleted event of all the HttpDownloadClients.
        /// </summary>
        void client_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if (e.Error != null
                && this.Status != DownloadStatus.Canceling
                && this.Status != DownloadStatus.Canceled)
            {
                this.Cancel();
                this.OnDownloadCompleted(new DownloadCompletedEventArgs(
                    null,
                    this.DownloadedSize,
                    this.TotalSize,
                    this.TotalUsedTime,
                    e.Error));
            }
        }

        /// <summary>
        /// Raise DownloadProgressChanged event. If the status is Completed, then raise
        /// DownloadCompleted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDownloadProgressChanged(
            DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(this, e);
            }
        }

        /// <summary>
        /// Raise StatusChanged event.
        /// </summary>
        protected virtual void OnStatusChanged(EventArgs e)
        {

            switch (this.Status)
            {
                case DownloadStatus.Waiting:
                case DownloadStatus.Downloading:
                case DownloadStatus.Paused:
                case DownloadStatus.Canceled:
                case DownloadStatus.Completed:
                    if (this.StatusChanged != null)
                    {
                        this.StatusChanged(this, e);
                    }
                    break;
                default:
                    break;
            }

            if (this.Status == DownloadStatus.Paused
                || this.Status == DownloadStatus.Canceled
                || this.Status == DownloadStatus.Completed)
            {
                this.usedTime += DateTime.Now - lastStartTime;
            }

            if (this.Status == DownloadStatus.Canceled)
            {
                Exception ex = new Exception("Downloading is canceled by user's request. ");
                this.OnDownloadCompleted(
                    new DownloadCompletedEventArgs(
                        null,
                        this.DownloadedSize,
                        this.TotalSize,
                        this.TotalUsedTime,
                        ex));
            }

            if (this.Status == DownloadStatus.Completed)
            {
                this.OnDownloadCompleted(
                    new DownloadCompletedEventArgs(
                        new FileInfo(this.DownloadPath),
                        this.DownloadedSize,
                        this.TotalSize,
                        this.TotalUsedTime,
                        null));
            }
        }

        /// <summary>
        /// Raise DownloadCompleted event.
        /// </summary>
        protected virtual void OnDownloadCompleted(
            DownloadCompletedEventArgs e)
        {
            if (DownloadCompleted != null)
            {
                DownloadCompleted(this, e);
            }
        }
    }
}
