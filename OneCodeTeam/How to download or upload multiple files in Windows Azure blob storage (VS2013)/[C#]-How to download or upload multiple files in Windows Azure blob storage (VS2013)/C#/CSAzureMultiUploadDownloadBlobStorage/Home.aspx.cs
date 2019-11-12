/****************************** Module Header ******************************\
Module Name:  Home.aspx.cs
Project:      CSAzureMultiUploadDownloadBlobStorage
Copyright (c) Microsoft Corporation.
 
For users with large amounts of unstructured data to store in the cloud, Blob storage offers a cost-effective and 
scalable solution ,users can store documents ,social data ,images and text etc.

This project  demonstrates How to download or upload multiple files in windows azure blob storage.
Users can choose multiple files of different kinds to upload to Blob storage.
Users can choose multiple files of different kinds to download from Blob storage.
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Text;
using System.Text.RegularExpressions;
namespace CSAzureMultiUploadDownloadBlobStorage
{
    public partial class Home : System.Web.UI.Page
    {

        Dictionary<string, CloudBlockBlob> dicBBlob = new Dictionary<string, CloudBlockBlob>();
        Dictionary<string, CloudPageBlob> dicPBlob = new Dictionary<string, CloudPageBlob>();
        Dictionary<string, List<string>> dicSelectedBlob = new Dictionary<string, List<string>>();
        List<string> lstContainer = new List<string>();

        CloudStorageAccount csa_storageAccount;
        
        /// <summary>
        /// Gets a CloudStorageAccount
        /// </summary>
        public CloudStorageAccount Csa_storageAccount
        {
            get
            {
                if (csa_storageAccount == null)
                {
                    string strAccount = "Storage Account";
                    string strKey = "Primary Access Key";

                    StorageCredentials credential = new StorageCredentials(strAccount, strKey);
                    csa_storageAccount = new CloudStorageAccount(credential, true);
                }
                return csa_storageAccount;
            }
        }

        
        protected void Page_Load(object sender, EventArgs e)
        {      
            if (!IsPostBack)
            {
                //Lists all containers of the specified storageAccount 
                GetContainerListByStorageAccount(Csa_storageAccount);
            }
            else
            {
                //Clears the data that is added previously 
                dicBBlob.Clear();
                dicPBlob.Clear();

                if (ViewState["selectContainer"] != null)
                {
                    List<string> lst = (List<string>)ViewState["selectContainer"];
                    foreach (string item in lst)
                    {
                        string strContainer = item;
                        if (!string.IsNullOrEmpty(strContainer))
                        {
                            // Displays blobs of the container selected  
                            GetBlobListByContainer(strContainer, Csa_storageAccount);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lists all containers of the specified storageAccount 
        /// </summary>
        /// <param name="storageAcount"></param>
        private void GetContainerListByStorageAccount(CloudStorageAccount storageAcount)
        {
            //clear all items
            cbxl_container.Items.Clear();

            //Gets Container of ViewState
            List<string> lstSelectContainer = new List<string>();
            if (ViewState["selectContainer"] != null)
            {
                lstSelectContainer = (List<string>)ViewState["selectContainer"];
            }
            //Lists all Containers and add them to CheckBoxList    
            CloudBlobClient blobClient = storageAcount.CreateCloudBlobClient();
            foreach (var container in blobClient.ListContainers())
            {
                ListItem item = new ListItem();
                item.Value = container.Uri.ToString();
                item.Text = container.Name;
                if (lstSelectContainer.Contains(container.Name))
                {
                    item.Selected = true;
                }
                cbxl_container.Items.Add(item);
            }
        }

        /// <summary>
        /// Displays blobs of the container selected  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbxl_container_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Clears the data that is added previously 
            lstContainer.Clear();
            dicBBlob.Clear();
            dicPBlob.Clear();

           //Gets Container of ViewState
            List<string> lstSelectContainer = new List<string>();
            if (ViewState["selectContainer"] != null)
            {
                lstSelectContainer = (List<string>)ViewState["selectContainer"];
            }

            foreach (ListItem item in cbxl_container.Items)
            {
                if (item.Selected)
                {
                    string strContainer = item.Text;
                    if (!string.IsNullOrEmpty(strContainer))
                    {
                        if (!lstContainer.Contains(strContainer))
                        {
                            lstContainer.Add(strContainer);
                        }
                    }
                }
            }

            //Clears blobs
            ClearBlobList(lstSelectContainer);

            foreach (string key in lstContainer)
            {
                GetBlobListByContainer(key, Csa_storageAccount);
            }

            
            // Saves the names of the selected containers   
            if (ViewState["selectContainer"] != null)
            {
                ViewState["selectContainer"] = lstContainer;
            }
            else
            {
                ViewState.Add("selectContainer", lstContainer);
            }
        }


        /// <summary>
        /// Displays blobs of the container selected  
        /// </summary>
        /// <param name="strContainerName"></param>
        /// <param name="storageAcount"></param>
        private void GetBlobListByContainer(string strContainerName, CloudStorageAccount storageAcount)
        {
            //Adds the container name as table title 
            TableCell celltitle = new TableCell();
            celltitle.Text = strContainerName;
            tbl_blobList.Rows[0].Cells.Add(celltitle);

            CloudBlobClient blobClient = storageAcount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(strContainerName);
            if (blobContainer.Exists())
            {
                TableCell cell = new TableCell();
                CheckBoxList chkl_blobList = new CheckBoxList();
                chkl_blobList.ID = strContainerName;
                chkl_blobList.AutoPostBack = false;
                chkl_blobList.EnableViewState = true;

                //Lists all blobs and add them to CheckBoxList
                foreach (var blob in blobContainer.ListBlobs(null, true))
                {
                    ListItem item = new ListItem();
                    string strUrl = blob.Uri.ToString();
                    string[] strUrlArr = strUrl.Split('/');
                    if (strUrlArr.Length > 0)
                    {
                        int intIndex = 0;
                        for (int i = 0; i < strUrlArr.Length; i++)
                        {
                            if (strUrlArr[i] == strContainerName)
                            {
                                intIndex = i;
                                break;
                            }
                        }
                        //Generates a new name in the format of ContainerName+BlobName
                        string strBlobName = "";
                        for (int i = intIndex + 1; i < strUrlArr.Length; i++)
                        {
                            strBlobName += strUrlArr[i] + "/";
                        }
                        if (!string.IsNullOrEmpty(strBlobName))
                        {
                            strBlobName = strBlobName.Substring(0, strBlobName.Length - 1);
                            item.Text = strBlobName;
                            item.Value = blob.Uri.ToString();                          
                            chkl_blobList.Items.Add(item);
                            if (blob is CloudBlockBlob && !dicBBlob.ContainsKey(strBlobName))
                            {
                                dicBBlob.Add(strBlobName, (CloudBlockBlob)blob);
                            }
                            else if (blob is CloudPageBlob && !dicPBlob.ContainsKey(strBlobName))
                            {
                                dicPBlob.Add(strBlobName, (CloudPageBlob)blob);
                            }
                        }
                    }
                }

                //Adds CheckBoxList to Table 
                cell.Controls.Add(chkl_blobList);
                tbl_blobList.Rows[1].Cells.Add(cell);
            }
        }

        /// <summary>
        /// Clears  blobs added in pageload event which have not been selected this time
        /// </summary>
        /// <param name="lstSelectContainer"></param>
        private void ClearBlobList(List<string> lstSelectContainer)
        {
            tbl_blobList.Rows[0].Cells.Clear();
            tbl_blobList.Rows[1].Cells.Clear();     
        }

        /// <summary>
        /// Gets blobs that are selected 
        /// </summary>
        /// <param name="lstSelectContainer"></param>
        private void GetSelectedBlob(List<string> lstSelectContainer)
        {
            List<int> lstCell = new List<int>();
            if (lstSelectContainer.Count > 0)
            {
                for (int i = 0; i < lstSelectContainer.Count; i++)
                {
                    for (int j = 0; j < tbl_blobList.Rows[0].Cells.Count; j++)
                    {
                        TableCell cell = tbl_blobList.Rows[0].Cells[j];
                        if (cell.Text == lstSelectContainer[i])
                        {
                            lstCell.Add(j);
                        }
                    }
                }
            }
            if (lstCell!=null&&lstCell.Count > 0)
            {
                for (int i = 0; i < lstCell.Count; i++)
                {
                    foreach (Control con in tbl_blobList.Rows[1].Cells[i].Controls)
                    {
                        if (con is CheckBoxList)
                        {
                            CheckBoxList cbx = (CheckBoxList)con;
                            foreach (ListItem item in cbx.Items)
                            {
                                if (item.Selected)
                                {
                                    string strBlob = item.Text;
                                    if (!string.IsNullOrEmpty(strBlob))
                                    {
                                        if (!dicSelectedBlob.ContainsKey(cbx.ID))
                                        {
                                            List<string> lst = new List<string>();
                                            lst.Add(strBlob);
                                            dicSelectedBlob.Add(cbx.ID, lst);
                                        }
                                        else
                                        {
                                            dicSelectedBlob[cbx.ID].Add(strBlob);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        
        /// <summary>
        /// Download files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Downlaod_Click(object sender, EventArgs e)
        {
            if (ViewState["selectContainer"] != null)
            {
                List<string> lst = (List<string>)ViewState["selectContainer"];

                //Gets blobs that are selected 
                GetSelectedBlob(lst);

                if(dicSelectedBlob.Count>0)
                {
                    foreach (string strContainer in lst)
                    {
                        if (dicSelectedBlob.ContainsKey(strContainer))
                        {
                            List<string> lstBlob = new List<string>();
                            lstBlob = dicSelectedBlob[strContainer];
                            foreach (string KeyName in lstBlob)
                            {
                                DownLoadBlobByBlobName(KeyName, strContainer);
                            }
                        }
                    }
                    Response.Write("<script>alert('Files Successfully downloaded！');</script>");
                }
                else
                {
                    Response.Write("<script>alert('Select the blobs you want to download！');</script>");
                }                         
            }
            else
            {
                Response.Write("<script>alert('Select the container which contains the blobs you want to download！');</script>");              
            }
        }

        /// <summary>
        /// Gets blob based on the given Blob name and downloads to local disk 
        /// </summary>
        /// <param name="strBlobName"></param>
        private void DownLoadBlobByBlobName(string strBlobName,string strContainer)
        {
            string filePath = Server.MapPath("DownLoad/");
            if (Directory.Exists(filePath) == false) 
            {
                Directory.CreateDirectory(filePath);
            }    
     
            //Generates  a new name
            string[] strName = strBlobName.Split('/');
            string strNewName = "";
            if (strName.Length > 0)
            {
                for (int i = 0; i < strName.Length; i++)
                {
                    strNewName += strName[i]+"_";
                }
            }
            strNewName = strNewName.Substring(0, strNewName.Length - 1);
            strNewName = strContainer + "_" + strNewName;

            try
            {
                //Download blob
                if (dicBBlob.ContainsKey(strBlobName))
                {
                    CloudBlockBlob bblob = dicBBlob[strBlobName];
                    bblob.DownloadToFile(filePath + strNewName, FileMode.Create);
                }
                if (dicPBlob.ContainsKey(strBlobName))
                {
                    CloudPageBlob bblob = dicPBlob[strBlobName];
                    bblob.DownloadToFile(filePath + strNewName, FileMode.Create);
                }
            }
            catch
            {

            }
        }   
   

        
        /// <summary>
        /// Upload files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            //Generates  container name
            string strContainerName = txt_container.Text;
            try
            {
                CloudBlobClient blobClient = Csa_storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(strContainerName);

                blobContainer.CreateIfNotExists();
                blobContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

                //uplaod files
                HttpFileCollection httpFiles = Request.Files;
                if (httpFiles != null)
                {
                    for (int i = 0; i < httpFiles.Count; i++)
                    {
                        HttpPostedFile file = httpFiles[i];

                        //Generates  a blobName
                        string blockName = file.FileName;
                        string[] strName = file.FileName.Split('\\');
                        if (strName.Length > 0)
                        {
                            blockName = strName[strName.Length - 1];
                        }
                        if (!string.IsNullOrEmpty(blockName))
                        {
                            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blockName);
                            //upload files
                            blob.UploadFromStream(file.InputStream);
                        }

                    }
                }

             
            }
            catch
            {

            }
            //Rereads the containers and blobs  
            GetContainerListByStorageAccount(Csa_storageAccount);
            cbxl_container_SelectedIndexChanged(cbxl_container, null);
        } 
    }
}