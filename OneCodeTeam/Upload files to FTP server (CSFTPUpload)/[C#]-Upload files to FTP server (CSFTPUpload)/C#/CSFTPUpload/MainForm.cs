/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSFTPUpload
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
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace CSFTPUpload
{
    public partial class MainForm : Form
    {

        FTPClientManager client = null;

        NetworkCredential currentCredentials = null;

        public MainForm()
        {
            InitializeComponent();

            RefreshUI();
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
                client.FileUploadCompleted +=
                    new EventHandler<FileUploadCompletedEventArgs>(client_FileUploadCompleted);
                client.NewMessageArrived +=
                    new EventHandler<NewMessageEventArg>(client_NewMessageArrived);

                // Refresh the UI and list the sub directories and files.
                RefreshUI();
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
                client_NewMessageArrivedHandler), sender, e);
        }

        void client_NewMessageArrivedHandler(object sender, NewMessageEventArg e)
        {
            string log = string.Format("{0} {1}",
                 DateTime.Now, e.NewMessage);
            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// Log the FileUploadCompleted event when a file was uploaded.
        /// </summary>
        void client_FileUploadCompleted(object sender, FileUploadCompletedEventArgs e)
        {
            this.Invoke(new EventHandler<FileUploadCompletedEventArgs>(
                client_FileUploadCompletedHandler), sender, e);
        }

        void client_FileUploadCompletedHandler(object sender, FileUploadCompletedEventArgs e)
        {
            string log = string.Format("{0} Upload from {1} to {2} is completed. Length: {3}. ",
                DateTime.Now, e.LocalFile.FullName, e.ServerPath, e.LocalFile.Length);

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
            RefreshUI();

            string log = string.Format("{0} FTPClient status changed to {1}. ",
             DateTime.Now, client.Status.ToString());

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        void RefreshUI()
        {
            // Disable all the buttons if the client is uploading file.
            if (client == null ||
                client.Status != FTPClientManagerStatus.Idle)
            {

                btnBrowseLocalFolder.Enabled = false;
                btnUploadFolder.Enabled = false;

                btnBrowseLocalFile.Enabled = false;
                btnUploadFile.Enabled = false;

                btnDelete.Enabled = false;

                btnNavigateParentFolder.Enabled = false;
                lstFileExplorer.Enabled = false;
            }
            else
            {

                btnBrowseLocalFolder.Enabled = true;
                btnUploadFolder.Enabled = true;

                btnBrowseLocalFile.Enabled = true;
                btnUploadFile.Enabled = true;

                btnDelete.Enabled = true;

                btnNavigateParentFolder.Enabled = true;
                lstFileExplorer.Enabled = true;
            }

            btnConnect.Enabled = client == null ||
                client.Status == FTPClientManagerStatus.Idle;

            RefreshSubDirectoriesAndFiles();

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
            if (client == null)
            {
                return;
            }

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

        #region Upload a Folder

        /// <summary>
        /// Handle the Click event of btnBrowseLocalFolder.
        /// </summary>
        private void btnBrowseLocalFolder_Click(object sender, EventArgs e)
        {
            BrowserLocalFolder();
        }

        /// <summary>
        /// Handle the Click event of btnUploadFolder.
        /// </summary>
        private void btnUploadFolder_Click(object sender, EventArgs e)
        {

            // If the tbLocalFolder.Text is empty, then show a FolderBrowserDialog.
            if (string.IsNullOrWhiteSpace(tbLocalFolder.Text)
                && BrowserLocalFolder() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                DirectoryInfo dir = new DirectoryInfo(tbLocalFolder.Text);

                if (!dir.Exists)
                {
                    throw new ApplicationException(
                        string.Format(" The folder {0} does not exist!", dir.FullName));
                }

                // Upload the selected items.
                client.UploadFolder(dir, client.Url, chkCreateFolder.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Show a FolderBrowserDialog.
        /// </summary>
        DialogResult BrowserLocalFolder()
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(tbLocalFolder.Text))
                {
                    folderBrowser.SelectedPath = tbLocalFolder.Text;
                }
                var result = folderBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tbLocalFolder.Text = folderBrowser.SelectedPath;
                }
                return result;
            }
        }

        #endregion


        #region Upload files

        private void btnBrowseLocalFile_Click(object sender, EventArgs e)
        {
            BrowserLocalFiles();
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (tbLocalFile.Tag == null
                && BrowserLocalFiles() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                List<FileInfo> files = new List<FileInfo>();
                string[] selectedFiles = tbLocalFile.Tag as string[];

                foreach (var selectedFile in selectedFiles)
                {
                    FileInfo fileInfo = new FileInfo(selectedFile);
                    if (!fileInfo.Exists)
                    {
                        throw new ApplicationException(
                            string.Format(" The file {0} does not exist!", selectedFile));
                    }
                    else
                    {
                        files.Add(fileInfo);
                    }
                }

                if (files.Count > 0)
                {
                    client.UploadFoldersAndFiles(files, client.Url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Show a FolderBrowserDialog.
        /// </summary>
        DialogResult BrowserLocalFiles()
        {
            using (OpenFileDialog fileBrowser = new OpenFileDialog())
            {
                fileBrowser.Multiselect = true;
                var result = fileBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tbLocalFile.Tag = fileBrowser.FileNames;

                    StringBuilder filesText = new StringBuilder();
                    foreach (var file in fileBrowser.FileNames)
                    {
                        filesText.Append(file + ";");
                    }
                    tbLocalFile.Text = filesText.ToString();
                }
                return result;
            }
        }


        #endregion

        #region Delete files

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstFileExplorer.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select the items to delete in the FTP File Explorer");
            }

            var itemsToDelete = 
                lstFileExplorer.SelectedItems.Cast<FTPFileSystem>().ToList();

            this.client.DeleteItemsOnFTPServer(itemsToDelete);

            RefreshUI();
        }

        #endregion

    }
}
