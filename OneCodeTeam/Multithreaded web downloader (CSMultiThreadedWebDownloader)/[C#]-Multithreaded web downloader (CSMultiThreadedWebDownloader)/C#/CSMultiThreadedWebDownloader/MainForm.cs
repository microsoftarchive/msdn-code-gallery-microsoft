/****************************** Module Header ******************************\
 * Module Name:  MainForm.cs
 * Project:      CSMultiThreadedWebDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the main form of this application. It is used to initialize the UI and 
 * handle the events.
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
using System.Configuration;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Linq;

namespace CSMultiThreadedWebDownloader
{
    public partial class MainForm : Form
    {

        MultiThreadedWebDownloader downloader = null;

        DateTime lastNotificationTime;

        WebProxy proxy = null;

        public string FileToDownload
        {
            get
            {
                return tbURL.Text;
            }
            set
            {
                tbURL.Text = value;
            }
        }

        public string DownloadPath
        {
            get
            {
                return tbPath.Text;
            }
            set
            {
                tbPath.Text = value;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            // Initialize proxy from App.Config
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyUrl"]))
            {
                proxy = new WebProxy(
                    System.Configuration.ConfigurationManager.AppSettings["ProxyUrl"]);

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyUser"])
                    && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyPwd"]))
                {
                    NetworkCredential credential = new NetworkCredential(
                        ConfigurationManager.AppSettings["ProxyUser"],
                        ConfigurationManager.AppSettings["ProxyPwd"]);

                    proxy.Credentials = credential;
                }
                else
                {
                    proxy.UseDefaultCredentials = true;
                }
            }

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Uri url = null;
                bool result = Uri.TryCreate(args.Last(), UriKind.Absolute, out url);
                if (result)
                {
                    this.FileToDownload = url.ToString();
                }
            }

        }

        /// <summary>
        /// Check the file information.
        /// </summary>
        private void btnCheck_Click(object sender, EventArgs e)
        {

            // Initialize an instance of MultiThreadedWebDownloader.
            downloader = new MultiThreadedWebDownloader(tbURL.Text);
            downloader.Proxy = this.proxy;

            // Register the events of HttpDownloadClient.
            downloader.DownloadCompleted += DownloadCompleted;
            downloader.DownloadProgressChanged += DownloadProgressChanged;
            downloader.StatusChanged += StatusChanged;

            try
            {
                string filename = string.Empty;
                downloader.CheckUrl(out filename);

                if (string.IsNullOrEmpty(filename))
                {
                    this.DownloadPath = string.Format("{0}\\{1}",
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        downloader.Url.Segments.Last());
                }
                else
                {
                    this.DownloadPath = string.Format("{0}\\{1}",
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        filename);
                }

                // Update the UI.
                tbURL.Enabled = false;
                btnCheck.Enabled = false;
                tbPath.Enabled = true;
                btnDownload.Enabled = true;

            }
            catch
            {
                // If there is any exception, like System.Net.WebException or 
                // System.Net.ProtocolViolationException, it means that there may be an 
                // error while reading the information of the file and it cannot be 
                // downloaded. 
                MessageBox.Show("There is an error while get the information of the file."
                   + " Please make sure the url is accessible.");
            }
        }

        /// <summary>
        /// Handle btnDownload Click event.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {

                // Check whether the file exists.
                if (File.Exists(tbPath.Text.Trim()))
                {
                    string message = "There is already a file with the same name, "
                            + "do you want to delete it? "
                            + "If not, please change the local path. ";
                    var result = MessageBox.Show(
                        message,
                        "File name conflict: " + tbPath.Text.Trim(),
                        MessageBoxButtons.OKCancel);

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        File.Delete(tbPath.Text.Trim());
                    }
                    else
                    {
                        return;
                    }
                }

                if (File.Exists(tbPath.Text.Trim() + ".tmp"))
                {
                    File.Delete(tbPath.Text.Trim() + ".tmp");
                }

                // Set the download path.
                downloader.DownloadPath = tbPath.Text.Trim() + ".tmp";
               
                // Start to download file.
                downloader.BeginDownload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        /// <summary>
        /// Handle StatusChanged event.
        /// </summary>
        void StatusChanged(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(StatusChangedHanlder),sender,e);
        }

        void StatusChangedHanlder(object sender, EventArgs e)
        {
           // Refresh the status.
            lbStatus.Text = downloader.Status.ToString();

            // Refresh the buttons.
            switch (downloader.Status)
            {
                case DownloadStatus.Waiting:
                    btnCheck.Enabled = false;
                    btnCancel.Enabled = true;
                    btnDownload.Enabled = false;
                    btnPause.Enabled = false;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
                case DownloadStatus.Canceled:
                case DownloadStatus.Completed:
                    btnCheck.Enabled = true;
                    btnCancel.Enabled = true;
                    btnDownload.Enabled = false;
                    btnPause.Enabled = false;
                    tbPath.Enabled = false;
                    tbURL.Enabled = true;
                    break;
                case DownloadStatus.Downloading:
                    btnCheck.Enabled = false;
                    btnCancel.Enabled = true;
                    btnDownload.Enabled = false;
                    btnPause.Enabled = true & downloader.IsRangeSupported;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
                case DownloadStatus.Paused:
                    btnCheck.Enabled = false;
                    btnCancel.Enabled = true;
                    btnDownload.Enabled = false;

                    // The "Resume" button.
                    btnPause.Enabled = true & downloader.IsRangeSupported;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
            }

            if (downloader.Status == DownloadStatus.Paused)
            {
                lbSummary.Text =
                   String.Format("Received: {0}KB, Total: {1}KB, Time: {2}:{3}:{4}",
                   downloader.DownloadedSize / 1024, downloader.TotalSize / 1024,
                   downloader.TotalUsedTime.Hours, downloader.TotalUsedTime.Minutes,
                   downloader.TotalUsedTime.Seconds);

                btnPause.Text = "Resume";
            }
            else
            {
                btnPause.Text = "Pause";
            }


        }
        

        /// <summary>
        /// Handle DownloadProgressChanged event.
        /// </summary>
        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Invoke(
                new EventHandler<DownloadProgressChangedEventArgs>(DownloadProgressChangedHandler),
                sender, e);
        }

        void DownloadProgressChangedHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            // Refresh the summary every second.
            if (DateTime.Now > lastNotificationTime.AddSeconds(1))
            {
                lbSummary.Text = String.Format(
                    "Received: {0}KB Total: {1}KB Speed: {2}KB/s  Threads: {3}",
                    e.ReceivedSize / 1024, 
                    e.TotalSize / 1024,
                    e.DownloadSpeed / 1024,
                    downloader.DownloadThreadsCount);
                prgDownload.Value = (int)(e.ReceivedSize * 100 / e.TotalSize);
                lastNotificationTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Handle DownloadCompleted event.
        /// </summary>
        void DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            this.Invoke(
                new EventHandler<DownloadCompletedEventArgs>(DownloadCompletedHandler),
                sender,e);
        }

        void DownloadCompletedHandler(object sender, DownloadCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                lbSummary.Text =
                                String.Format("Received: {0}KB, Total: {1}KB, Time: {2}:{3}:{4}",
                                e.DownloadedSize / 1024, e.TotalSize / 1024, e.TotalTime.Hours,
                                e.TotalTime.Minutes, e.TotalTime.Seconds);

                File.Move(tbPath.Text.Trim() + ".tmp", tbPath.Text.Trim());

                prgDownload.Value = 100;
            }
            else
            {
                lbSummary.Text = e.Error.Message;
                if (File.Exists(tbPath.Text.Trim() + ".tmp"))
                {
                    File.Delete(tbPath.Text.Trim() + ".tmp");
                }

                if (File.Exists(tbPath.Text.Trim()))
                {
                    File.Delete(tbPath.Text.Trim());
                }

                prgDownload.Value = 0;
            }
        }

        /// <summary>
        /// Handle btnCancel Click event.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (downloader != null)
            {
                downloader.Cancel();
            }          
        }

        /// <summary>
        /// Handle btnPause Click event.
        /// </summary>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (downloader.Status == DownloadStatus.Paused)
            {
                downloader.Resume();
            }
            else if (downloader.Status == DownloadStatus.Downloading)
            {
                downloader.Pause();
            }
        }

    }
}
