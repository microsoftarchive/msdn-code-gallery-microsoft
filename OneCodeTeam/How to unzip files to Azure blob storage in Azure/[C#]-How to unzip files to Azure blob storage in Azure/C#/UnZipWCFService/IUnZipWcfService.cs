/****************************** Module Header ******************************\
*Module Name:  UnZipWCFService.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace UnZipWCFService
{
    [ServiceContract]
    interface  IUnZipWcfService
    {
        [OperationContract]
        bool UnZipFiles(string strPath,string strContainerName);
       
        [OperationContract]
        List<string> GetAllContainer();
       
        [OperationContract]
        List<string> GetAllBlob(string strContainerName);

    }
    
}
