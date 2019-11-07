/****************************** Module Header ******************************\
*Module Name:  UnZipService.cs
*Project:      CSAzureUnzipFilesToBlobStorage
*Copyright (c) Microsoft Corporation.
* 
*For users with large amounts of unstructured data to store in the cloud, Blob storage offers a cost-effective and 
*scalable solution ,users can store documents ,social data ,images and text etc.
*
*This project  demonstrates how to unzip files to Azure blob storage in Azure.
*Uploading thousands of small files one-by-one is very slow. 
*It would be great if we could upload a zip file to Azure and unzip it directly into blob storage in Azure.
* 
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using Ionic.Zip;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnZipWCFService
{
    public class UnZipService : IUnZipWcfService
    {
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


        public static string strLoacalStorage = string.Empty;

        /// <summary>
        /// Uses DoNetZip to unzip the file to local storage
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="strContainerName"></param>
        /// <returns></returns>
        public bool UnZipFiles(string strPath, string strContainerName)
        {
            bool bln = true;
            try
            {
                if (!string.IsNullOrEmpty(strPath) && !string.IsNullOrEmpty(strContainerName))
                {
                    ZipFile zipFile = ZipFile.Read(strPath);
                    zipFile.ExtractAll(strLoacalStorage, ExtractExistingFileAction.OverwriteSilently);
                    DirectoryInfo TheFolder = new DirectoryInfo(strLoacalStorage);
                    ListAllFiles(TheFolder, strContainerName);
                }
            }
            catch (Exception ex)
            {
                bln = false;
                throw ex;
            }

            return bln;
        }

        /// <summary>
        /// Lists all files of the specified directory
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="strContainerName"></param>

        private void ListAllFiles(DirectoryInfo dirInfo, string strContainerName)
        {
            try
            {
                foreach (var file in dirInfo.GetFileSystemInfos())
                {
                    if (file is FileInfo)
                    {
                        FileInfo fileInfo = (FileInfo)file;
                        UploadToBlob((FileInfo)file, strContainerName);
                    }
                    else
                    {
                        DirectoryInfo newinfo = (DirectoryInfo)file;
                        ListAllFiles(newinfo, strContainerName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// UpLoads file to blob storage
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="strContainerName"></param>
        private void UploadToBlob(FileInfo fileInfo, string strContainerName)
        {
            try
            {
                CloudBlobClient blobClient = Csa_storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(strContainerName);
                blobContainer.CreateIfNotExists();
                blobContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

                //Generates  a blobName
                string strPath = fileInfo.FullName;
                string strNewBlobName = string.Empty;
                int intIndex = strPath.IndexOf(strLoacalStorage, 0);
                if (intIndex >= 0)
                {
                    int intStartIndex = intIndex + strLoacalStorage.Length;
                    int intLength = strPath.Length - intStartIndex;
                    strPath = strPath.Substring(intStartIndex, intLength);
                }
                string[] strArrPathName = strPath.Split('\\');
                if (strArrPathName.Length > 0)
                {
                    for (int i = 0; i < strArrPathName.Length; i++)
                    {
                        strNewBlobName += strArrPathName[i] + "_";
                    }
                    strNewBlobName = strNewBlobName.Substring(0, strNewBlobName.Length - 1);
                }

                if (!string.IsNullOrEmpty(strNewBlobName))
                {
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(strNewBlobName);
                    //upload files
                    using (FileStream stream = fileInfo.OpenRead())
                    {
                        blob.UploadFromStream(stream);
                    }
                    fileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                throw ex;
            }
        }

        /// <summary>
        /// Lists all containers of the specified storageAccount 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllContainer()
        {
            List<string> lstContainer = new List<string>();
            try
            {
                //list all Containers and add them to checkboxlist
                CloudBlobClient blobClient = Csa_storageAccount.CreateCloudBlobClient();
                foreach (CloudBlobContainer container in blobClient.ListContainers())
                {
                    if (!lstContainer.Contains(container.Name))
                    {
                        lstContainer.Add(container.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstContainer;
        }


        /// <summary>
        /// Lists all blobs of the container selected 
        /// </summary>
        /// <param name="strContainerName"></param>
        /// <returns></returns>
        public List<string> GetAllBlob(string strContainerName)
        {
            List<string> lstBlob = new List<string>();
            try
            {
                if (!string.IsNullOrEmpty(strContainerName))
                {
                    CloudBlobClient blobClient = Csa_storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer blobContainer = blobClient.GetContainerReference(strContainerName);
                    if (blobContainer.Exists())
                    {
                        //list all blobs 
                        foreach (var blob in blobContainer.ListBlobs(null, true))
                        {
                            #region
                            //get NewName(NewName is container name connect blob name)
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
                                //get NewName(NewName is container name connect blob name)
                                string strBlobName = "";
                                for (int i = intIndex + 1; i < strUrlArr.Length; i++)
                                {
                                    strBlobName += strUrlArr[i] + "/";
                                }

                                if (!string.IsNullOrEmpty(strBlobName))
                                {
                                    strBlobName = strBlobName.Substring(0, strBlobName.Length - 1);

                                    if (!lstBlob.Contains(strBlobName))
                                    {
                                        lstBlob.Add(strBlobName);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstBlob;
        }

    }
}
