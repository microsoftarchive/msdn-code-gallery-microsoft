/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSFTPDownload
* Copyright (c) Microsoft Corporation.
* 
* This is the main form of this application. It is used to initialize the UI and 
* handle the events.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace CSFTPDownload
{
    public partial class MainForm : Form
    {

        FTPClientManager client = null;

        NetworkCredential currentCredentials = null;

        public MainForm()
        {
            InitializeComponent();
        }

        #region URL navigation

        /// <summary>
        /// Handle the Click event of btnConnect.
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {

            // Connect to server specified by tbFTPServer.Text.
            Connect(this.tbFTPServer.Text.Trim());

        }

        void Connect(string urlStr)
        {
            try
            {
                Uri url = new Uri(urlStr);

                // The schema of url must be ftp. 
                if (!url.Scheme.Equals("ftp", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ApplicationException("The schema of url must be ftp. ");
                }

                // Set the url to the folder that contains this file. 
                if (url.IsFile)
                {
                    url = new Uri(url, "..");
                }

                // Show the Form UICredentialsProvider to get new Credentials.
                using (UICredentialsProvider provider =
                    new UICredentialsProvider(this.currentCredentials))
                {

                    // Show the Form UICredentialsProvider as a dialog.
                    var result = provider.ShowDialog();

                    // If user typed the Credentials and pressed the "OK" button.
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {

                        // Reset the current Credentials.
                        this.currentCredentials = provider.Credentials;

                    }
                    else
                    {
                        return;
                    }
                }

                // Initialize the FTPClient instance.
                client = new FTPClientManager(url, currentCredentials);

                client.UrlChanged += new EventHandler(client_UrlChanged);
                client.StatusChanged += new EventHandler(client_StatusChanged);
                client.ErrorOccurred += new EventHandler<ErrorEventArgs>(client_ErrorOccurred);
                client.FileDownloadCompleted +=
                    new EventHandler<FileDownloadCompletedEventArgs>(client_FileDownloadCompleted);
                client.NewMessageArrived +=
                    new EventHandler<NewMessageEventArg>(client_NewMessageArrived);

                // List the sub directories and files.
                RefreshSubDirectoriesAndFiles();
            }


            catch (System.Net.WebException webEx)
            {
                if ((webEx.Response as FtpWebResponse).StatusCode == FtpStatusCode.NotLoggedIn)
                {
                    // Reconnect the server.
                    Connect(urlStr);

                    return;
                }
                else
                {
                    MessageBox.Show(webEx.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Log the message of FTPClient.
        /// </summary>
        void client_NewMessageArrived(object sender, NewMessageEventArg e)
        {
            this.Invoke(new EventHandler<NewMessageEventArg>(
                client_NewMessageArrivedHandler),sender,e);
        }

        void client_NewMessageArrivedHandler(object sender, NewMessageEventArg e)
        {
            string log = string.Format("{0} {1}",
                 DateTime.Now, e.NewMessage);
            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }


        /// <summary>
        /// Log the FileDownloadCompleted event when a file was downloaded.
        /// </summary>
        void client_FileDownloadCompleted(object sender, FileDownloadCompletedEventArgs e)
        {
            this.Invoke(new EventHandler<FileDownloadCompletedEventArgs>(
                 client_FileDownloadCompletedHandler), sender, e);
        }

        void client_FileDownloadCompletedHandler(object sender, FileDownloadCompletedEventArgs e)
        {
            string log = string.Format("{0} Download from {1} to {2} is completed. Length: {3}. ",
                DateTime.Now, e.ServerPath, e.LocalFile.FullName, e.LocalFile.Length);

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// Log the ErrorOccurred event if there was an error.
        /// </summary>
        void client_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            this.Invoke(new EventHandler<ErrorEventArgs>(
                  client_ErrorOccurredHandler), sender, e);
        }

        void client_ErrorOccurredHandler(object sender, ErrorEventArgs e)
        {
            this.lstLog.Items.Add(
                string.Format("{0} {1} ", DateTime.Now, e.ErrorException.Message));

            var innerException = e.ErrorException.InnerException;

            // Log all the innerException.
            while (innerException != null)
            {
                this.lstLog.Items.Add(
                              string.Format("\t\t\t {0} ", innerException.Message));
                innerException = innerException.InnerException;
            }

            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// Refresh the UI if the Status of the FTPClient changed.
        /// </summary>
        void client_StatusChanged(object sender, EventArgs e)
        {

            this.Invoke(new EventHandler(client_StatusChangedHandler), sender, e);
        }

        void client_StatusChangedHandler(object sender, EventArgs e)
        {

            // Disable all the buttons if the client is downloading file.
            if (client.Status == FTPClientManagerStatus.Downloading)
            {
                btnBrowseDownloadPath.Enabled = false;
                btnConnect.Enabled = false;
                btnDownload.Enabled = false;
                btnNavigateParentFolder.Enabled = false;
                lstFileExplorer.Enabled = false;
            }
            else
            {
                btnBrowseDownloadPath.Enabled = true;
                btnConnect.Enabled = true;
                btnDownload.Enabled = true;
                btnNavigateParentFolder.Enabled = true;
                lstFileExplorer.Enabled = true;
            }

            string log = string.Format("{0} FTPClient status changed to {1}. ",
               DateTime.Now, client.Status.ToString());

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// Handle the UrlChanged event of the FTPClient.
        /// </summary>
        void client_UrlChanged(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(client_UrlChangedHandler), sender, e);
        }

        void client_UrlChangedHandler(object sender, EventArgs e)
        {
            RefreshSubDirectoriesAndFiles();

            string log = string.Format("{0} The current url changed to {1}. ",
              DateTime.Now, client.Url);

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// Handle the DoubleClick event of lstFileExplorer.
        /// </summary>
        private void lstFileExplorer_DoubleClick(object sender, EventArgs e)
        {
            // if only one item is selected and the item represents a folder, then navigate
            // to a subDirectory.
            if (lstFileExplorer.SelectedItems.Count == 1
                && (lstFileExplorer.SelectedItem as FTPFileSystem).IsDirectory)
            {
                this.client.Naviagte(
                    (lstFileExplorer.SelectedItem as FTPFileSystem).Url);
            }
        }

        /// <summary>
        /// Handle the Click event of btnNavigateParentFolder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNavigateParentFolder_Click(object sender, EventArgs e)
        {

            // Navigate to the parent folder.
            this.client.NavigateParent();
        }

        /// <summary>
        /// List the sub directories and files.
        /// </summary>
        void RefreshSubDirectoriesAndFiles()
        {
            lbCurrentUrl.Text = string.Format("Current Path: {0}", client.Url);

            var subDirs = client.GetSubDirectoriesAndFiles();

            // Sort the list.
            var orderedDirs = from dir in subDirs
                              orderby dir.IsDirectory descending, dir.Name
                              select dir;

            lstFileExplorer.Items.Clear();
            foreach (var subdir in orderedDirs)
            {
                lstFileExplorer.Items.Add(subdir);
            }
        }


        #endregion

        #region Download File/Folders

        /// <summary>
        /// Handle the Click event of btnBrowseDownloadPath.
        /// </summary>
        private void btnBrowseDownloadPath_Click(object sender, EventArgs e)
        {
            BrowserDownloadPath();
        }

        /// <summary>
        /// Handle the Click event of btnDownload.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {

            // One or more files / folders should be selected in the File Explorer.
            if (lstFileExplorer.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Please select one or more files / folders in the File Explorer",
                    "No file is selected");
                return;
            }

            // If the tbDownloadPath.Text is empty, then show a FolderBrowserDialog.
            if (string.IsNullOrEmpty(tbDownloadPath.Text)
                && BrowserDownloadPath() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }


            var directoriesAndFiles = 
                lstFileExplorer.SelectedItems.Cast<FTPFileSystem>().ToList();

            // Download the selected items.
            client.DownloadDirectoriesAndFiles(directoriesAndFiles, tbDownloadPath.Text);

        }

        /// <summary>
        /// Show a FolderBrowserDialog.
        /// </summary>
        DialogResult BrowserDownloadPath()
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(tbDownloadPath.Text))
                {
                    folderBrowser.SelectedPath = tbDownloadPath.Text;
                }
                var result = folderBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tbDownloadPath.Text = folderBrowser.SelectedPath;
                }
                return result;
            }
        }
        #endregion
    }
}
