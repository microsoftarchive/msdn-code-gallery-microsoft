'/****************************** Module Header ******************************\
'Module Name:  IUnZipWcfService.vb
'Project:      VBAzureUnzipFilesToBlobStorage
'Copyright (c) Microsoft Corporation.
' 
'For users with large amounts of unstructured data to store in the cloud, Blob storage offers a cost-effective and 
'scalable solution ,users can store documents ,social data ,images and text etc.
'
'This project  demonstrates how to unzip files to Azure blob storage in Azure.
'Uploading thousands of small files one-by-one is very slow. 
'It would be great if we could upload a zip file to Azure and unzip it directly into blob storage in Azure.
' 
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
'All other rights reserved.
' 
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/
Imports System.ServiceModel

<ServiceContract>
Friend Interface IUnZipWcfService
    <OperationContract>
    Function UnZipFiles(ByVal strPath As String, ByVal strContainerName As String) As Boolean

    <OperationContract>
    Function GetAllContainer() As List(Of String)

    <OperationContract>
    Function GetAllBlob(ByVal strContainerName As String) As List(Of String)

End Interface

