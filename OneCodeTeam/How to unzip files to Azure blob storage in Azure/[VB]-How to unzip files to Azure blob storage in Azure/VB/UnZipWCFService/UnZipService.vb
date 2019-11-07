'/****************************** Module Header ******************************\
'Module Name:  UnZipService.vb
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

Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Auth
Imports System.IO
Imports Ionic.Zip
Imports Microsoft.WindowsAzure.Storage.Blob

Public Class UnZipService
    Implements IUnZipWcfService
    Private csa_storageAccount_Renamed As CloudStorageAccount

    ''' <summary>
    ''' Gets a CloudStorageAccount
    ''' </summary>
    Public ReadOnly Property Csa_storageAccount() As CloudStorageAccount
        Get
            If csa_storageAccount_Renamed Is Nothing Then
                Dim strAccount As String = "Storage Account"
                Dim strKey As String = "Primary Access Key"

                Dim credential As New StorageCredentials(strAccount, strKey)
                csa_storageAccount_Renamed = New CloudStorageAccount(credential, True)
            End If
            Return csa_storageAccount_Renamed
        End Get
    End Property


    Public Shared strLoacalStorage As String = String.Empty

    ''' <summary>
    ''' Uses DoNetZip to unzip the file to local storage
    ''' </summary>
    ''' <param name="strPath"></param>
    ''' <param name="strContainerName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UnZipFiles(ByVal strPath As String, ByVal strContainerName As String) As Boolean Implements IUnZipWcfService.UnZipFiles

        Dim bln As Boolean = True
        Try
            If (Not String.IsNullOrEmpty(strPath)) AndAlso (Not String.IsNullOrEmpty(strContainerName)) Then
                Dim zipFile As ZipFile = zipFile.Read(strPath)
                zipFile.ExtractAll(strLoacalStorage, ExtractExistingFileAction.OverwriteSilently)
                Dim TheFolder As New DirectoryInfo(strLoacalStorage)
                ListAllFiles(TheFolder, strContainerName)
            End If
        Catch ex As Exception
            bln = False
            Throw ex
        End Try

        Return bln
    End Function

    ''' <summary>
    ''' Lists all files of the specified directory
    ''' </summary>
    ''' <param name="dirInfo"></param>
    ''' <param name="strContainerName"></param>
    ''' <remarks></remarks>
    Private Sub ListAllFiles(ByVal dirInfo As DirectoryInfo, ByVal strContainerName As String)
        Try
            For Each File As FileSystemInfo In dirInfo.GetFileSystemInfos()
                If TypeOf File Is FileInfo Then
                    Dim fileInfo As FileInfo = CType(File, FileInfo)
                    UploadToBlob(CType(File, FileInfo), strContainerName)
                Else
                    Dim newinfo As DirectoryInfo = CType(File, DirectoryInfo)
                    ListAllFiles(newinfo, strContainerName)
                End If
            Next File
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' UpLoads file to blob storage
    ''' </summary>
    ''' <param name="fileInfo"></param>
    ''' <param name="strContainerName"></param>
    ''' <remarks></remarks>
    Private Sub UploadToBlob(ByVal fileInfo As FileInfo, ByVal strContainerName As String)
        Try
            Dim blobClient As CloudBlobClient = Csa_storageAccount.CreateCloudBlobClient()
            Dim blobContainer As CloudBlobContainer = blobClient.GetContainerReference(strContainerName)
            blobContainer.CreateIfNotExists()
            blobContainer.SetPermissions(New BlobContainerPermissions With {.PublicAccess = BlobContainerPublicAccessType.Blob})

            'Generates  a blobName
            Dim strPath As String = fileInfo.FullName
            Dim strNewBlobName As String = String.Empty
            Dim intIndex As Integer = strPath.IndexOf(strLoacalStorage, 0)
            If intIndex >= 0 Then
                Dim intStartIndex As Integer = intIndex + strLoacalStorage.Length
                Dim intLength As Integer = strPath.Length - intStartIndex
                strPath = strPath.Substring(intStartIndex, intLength)
            End If
            Dim strArrPathName() As String = strPath.Split("\"c)
            If strArrPathName.Length > 0 Then
                For i As Integer = 0 To strArrPathName.Length - 1
                    strNewBlobName &= strArrPathName(i) & "_"
                Next i
                strNewBlobName = strNewBlobName.Substring(0, strNewBlobName.Length - 1)
            End If

            If Not String.IsNullOrEmpty(strNewBlobName) Then
                Dim blob As CloudBlockBlob = blobContainer.GetBlockBlobReference(strNewBlobName)
                'upload files
                Using stream As FileStream = fileInfo.OpenRead()
                    blob.UploadFromStream(stream)
                End Using
                fileInfo.Delete()
            End If
        Catch ex As Exception
            If fileInfo.Exists Then
                fileInfo.Delete()
            End If
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Lists all containers of the specified storageAccount 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAllContainer() As List(Of String) Implements IUnZipWcfService.GetAllContainer
        Dim lstContainer As New List(Of String)()
        Try
            'list all Containers and add them to checkboxlist
            Dim blobClient As CloudBlobClient = Csa_storageAccount.CreateCloudBlobClient()
            For Each container As CloudBlobContainer In blobClient.ListContainers()
                If Not lstContainer.Contains(container.Name) Then
                    lstContainer.Add(container.Name)
                End If
            Next container
        Catch ex As Exception
            Throw ex
        End Try

        Return lstContainer
    End Function

    ''' <summary>
    ''' Lists all blobs of the container selected 
    ''' </summary>
    ''' <param name="strContainerName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAllBlob(ByVal strContainerName As String) As List(Of String) Implements IUnZipWcfService.GetAllBlob

        Dim lstBlob As New List(Of String)()
        Try
            If Not String.IsNullOrEmpty(strContainerName) Then
                Dim blobClient As CloudBlobClient = Csa_storageAccount.CreateCloudBlobClient()
                Dim blobContainer As CloudBlobContainer = blobClient.GetContainerReference(strContainerName)
                If blobContainer.Exists() Then
                    'list all blobs 
                    For Each Blob As IListBlobItem In blobContainer.ListBlobs(Nothing, True)

                        'get NewName(NewName is container name connect blob name)
                        Dim strUrl As String = Blob.Uri.ToString()
                        Dim strUrlArr() As String = strUrl.Split("/"c)
                        If strUrlArr.Length > 0 Then
                            Dim intIndex As Integer = 0
                            For i As Integer = 0 To strUrlArr.Length - 1
                                If strUrlArr(i) = strContainerName Then
                                    intIndex = i
                                    Exit For
                                End If
                            Next i
                            'get NewName(NewName is container name connect blob name)
                            Dim strBlobName As String = ""
                            For i As Integer = intIndex + 1 To strUrlArr.Length - 1
                                strBlobName &= strUrlArr(i) & "/"
                            Next i

                            If Not String.IsNullOrEmpty(strBlobName) Then
                                strBlobName = strBlobName.Substring(0, strBlobName.Length - 1)

                                If Not lstBlob.Contains(strBlobName) Then
                                    lstBlob.Add(strBlobName)
                                End If
                            End If
                        End If

                    Next Blob
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try

        Return lstBlob
    End Function



End Class