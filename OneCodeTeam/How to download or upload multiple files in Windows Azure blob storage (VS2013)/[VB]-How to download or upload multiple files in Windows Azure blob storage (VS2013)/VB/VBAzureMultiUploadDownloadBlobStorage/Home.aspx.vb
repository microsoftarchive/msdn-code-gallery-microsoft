'***************************** Module Header ******************************\
' Module Name: Home.aspx.vb
' Project:     VBAzureMultiUploadDownloadBlobStorage
' Copyright (c) Microsoft Corporation.
' 
' For users with large amounts of unstructured data to store in the cloud, Blob storage offers a cost-effective and 
'scalable solution ,users can store documents ,social data ,images and text etc.

'This project  demonstrates How to download or upload multiple files in windows azure blob storage.
'Users can choose multiple files of different kinds to upload to Blob storage.
'Users can choose multiple files of different kinds to download from Blob storage.

' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Auth
Imports Microsoft.WindowsAzure.Storage.Blob
Imports System.IO

Public Class Home
    Inherits System.Web.UI.Page

    Dim dicBBlob As New Dictionary(Of String, CloudBlockBlob)()
    Dim dicPBlob As New Dictionary(Of String, CloudPageBlob)()
    Dim dicSelectedBlob As New Dictionary(Of String, List(Of String))()
    Dim lstContainer As New List(Of String)()

    Private m_csa_storageAccount As CloudStorageAccount

    ''' <summary>
    ''' Gets a CloudStorageAccount
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Csa_storageAccount() As CloudStorageAccount
        Get
            If m_csa_storageAccount Is Nothing Then
                Dim strAccount As String = "Storage Account"
                Dim strKey As String = "Primary Access Key"

                Dim credential As New StorageCredentials(strAccount, strKey)
                m_csa_storageAccount = New CloudStorageAccount(credential, True)
            End If
            Return m_csa_storageAccount
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'Lists all containers of the specified storageAccount
            GetContainerListByStorageAccount(Csa_storageAccount)
        Else
            'Clears the data that is added previously 
            dicBBlob.Clear()
            dicPBlob.Clear()

            If ViewState("selectContainer") IsNot Nothing Then
                Dim lst As List(Of String) = CType(ViewState("selectContainer"), List(Of String))

                For Each item As String In lst
                    Dim strContainer As String = item
                    If Not String.IsNullOrEmpty(strContainer) Then
                        'Displays blobs of the container selected 
                        GetBlobListByContainer(strContainer, Csa_storageAccount)
                    End If
                Next item
            End If
        End If
    End Sub

    ''' <summary>
    ''' Lists all containers of the specified storageAccount 
    ''' </summary>
    ''' <param name="storageAcount"></param>
    ''' <remarks></remarks>
    Private Sub GetContainerListByStorageAccount(storageAcount As CloudStorageAccount)

        'clear all items
        cbxl_container.Items.Clear()

        'Gets Container of ViewState
        Dim lstSelectContainer As New List(Of String)()
        If ViewState("selectContainer") IsNot Nothing Then
            lstSelectContainer = CType(ViewState("selectContainer"), List(Of String))
        End If
        'Lists all Containers and add them to CheckBoxList    
        Dim blobClient As CloudBlobClient = storageAcount.CreateCloudBlobClient()
        For Each container As CloudBlobContainer In blobClient.ListContainers()
            Dim item As New ListItem()
            item.Value = container.Uri.ToString()
            item.Text = container.Name
            If lstSelectContainer.Contains(container.Name) Then
                item.Selected = True
            End If
            cbxl_container.Items.Add(item)
        Next container

    End Sub

    ''' <summary>
    ''' Displays blobs of the container selected  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub cbxl_container_SelectedIndexChanged(sender As Object, e As EventArgs)

        'Clears the data that is added previously 
        lstContainer.Clear()
        dicBBlob.Clear()
        dicPBlob.Clear()

        'Gets Container of ViewState
        Dim lstSelectContainer As New List(Of String)()
        If ViewState("selectContainer") IsNot Nothing Then
            lstSelectContainer = CType(ViewState("selectContainer"), List(Of String))
        End If

        For Each item As ListItem In cbxl_container.Items
            If item.Selected Then
                Dim strContainer As String = item.Text
                If Not String.IsNullOrEmpty(strContainer) Then
                    If Not lstContainer.Contains(strContainer) Then
                        lstContainer.Add(strContainer)
                    End If
                End If
            End If
        Next item

        'Clears blobs
        ClearBlobList(lstSelectContainer)

        For Each key As String In lstContainer
            GetBlobListByContainer(key, Csa_storageAccount)
        Next key

       

        ' Saves the names of the selected containers   
        If ViewState("selectContainer") IsNot Nothing Then
            ViewState("selectContainer") = lstContainer
        Else
            ViewState.Add("selectContainer", lstContainer)
        End If

    End Sub

    ''' <summary>
    '''  Displays blobs of the container selected  
    ''' </summary>
    ''' <param name="strContainerName"></param>
    ''' <param name="storageAcount"></param>
    ''' <remarks></remarks>
    Private Sub GetBlobListByContainer(strContainerName As String, storageAcount As CloudStorageAccount)
        'Adds the container name as table title 
        Dim celltitle As New TableCell()
        celltitle.Text = strContainerName
        tbl_blobList.Rows(0).Cells.Add(celltitle)

        Dim blobClient As CloudBlobClient = storageAcount.CreateCloudBlobClient()
        Dim blobContainer As CloudBlobContainer = blobClient.GetContainerReference(strContainerName)
        If blobContainer.Exists() Then
            Dim cell As New TableCell()
            Dim chkl_blobList As New CheckBoxList()
            chkl_blobList.ID = strContainerName
            chkl_blobList.AutoPostBack = False
            chkl_blobList.EnableViewState = True

            'Lists all blobs and add them to CheckBoxList
            For Each Blob As IListBlobItem In blobContainer.ListBlobs(Nothing, True)
                Dim item As New ListItem()
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
                    'Generates a new name in the format of ContainerName+BlobName
                    Dim strBlobName As String = ""
                    For i As Integer = intIndex + 1 To strUrlArr.Length - 1
                        strBlobName &= strUrlArr(i) & "/"
                    Next i
                    If Not String.IsNullOrEmpty(strBlobName) Then
                        strBlobName = strBlobName.Substring(0, strBlobName.Length - 1)
                        item.Text = strBlobName
                        item.Value = Blob.Uri.ToString()
                        chkl_blobList.Items.Add(item)
                        If TypeOf Blob Is CloudBlockBlob AndAlso (Not dicBBlob.ContainsKey(strBlobName)) Then
                            dicBBlob.Add(strBlobName, CType(Blob, CloudBlockBlob))
                        ElseIf TypeOf Blob Is CloudPageBlob AndAlso (Not dicPBlob.ContainsKey(strBlobName)) Then
                            dicPBlob.Add(strBlobName, CType(Blob, CloudPageBlob))
                        End If
                    End If
                End If
            Next Blob

            'Adds CheckBoxList to Table 
            cell.Controls.Add(chkl_blobList)
            tbl_blobList.Rows(1).Cells.Add(cell)
        End If
    End Sub

    ''' <summary>
    ''' Clears  blobs added in pageload event which have not been selected this time
    ''' </summary>
    ''' <param name="lstSelectContainer"></param>
    ''' <remarks></remarks>
    Private Sub ClearBlobList(ByVal lstSelectContainer As List(Of String))
        tbl_blobList.Rows(0).Cells.Clear()
        tbl_blobList.Rows(1).Cells.Clear()
    End Sub

    ''' <summary>
    '''  Gets blobs that are selected 
    ''' </summary>
    ''' <param name="lstSelectContainer"></param>
    ''' <remarks></remarks>
    Private Sub GetSelectedBlob(ByVal lstSelectContainer As List(Of String))
        Dim lstCell As New List(Of Integer)()
        If lstSelectContainer.Count > 0 Then
            For i As Integer = 0 To lstSelectContainer.Count - 1
                For j As Integer = 0 To tbl_blobList.Rows(0).Cells.Count - 1
                    Dim cell As TableCell = tbl_blobList.Rows(0).Cells(j)
                    If cell.Text = lstSelectContainer(i) Then
                        lstCell.Add(j)
                    End If
                Next j
            Next i
        End If
        If lstCell IsNot Nothing AndAlso lstCell.Count > 0 Then
            For i As Integer = 0 To lstCell.Count - 1
                For Each con As Control In tbl_blobList.Rows(1).Cells(i).Controls
                    If TypeOf con Is CheckBoxList Then
                        Dim cbx As CheckBoxList = CType(con, CheckBoxList)
                        For Each item As ListItem In cbx.Items
                            If item.Selected Then
                                Dim strContainer As String = item.Text
                                If (Not String.IsNullOrEmpty(strContainer)) AndAlso (Not dicSelectedBlob.ContainsKey(cbx.ID)) Then
                                    Dim lst As New List(Of String)()
                                    lst.Add(strContainer)
                                    dicSelectedBlob.Add(cbx.ID, lst)
                                Else
                                    dicSelectedBlob(cbx.ID).Add(strContainer)
                                End If
                            End If
                        Next item
                    End If
                Next con
            Next i
        End If
    End Sub

    ''' <summary>
    ''' Download files
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btn_Downlaod_Click(sender As Object, e As EventArgs)
        If ViewState("selectContainer") IsNot Nothing Then
            Dim lst As List(Of String) = CType(ViewState("selectContainer"), List(Of String))

            'Gets blobs that are selected 
            GetSelectedBlob(lst)

            If dicSelectedBlob.Count > 0 Then
                For Each strContainer As String In lst
                    If dicSelectedBlob.ContainsKey(strContainer) Then
                        Dim lstBlob As New List(Of String)()
                        lstBlob = dicSelectedBlob(strContainer)
                        For Each KeyName As String In lstBlob
                            DownLoadBlobByBlobName(KeyName, strContainer)
                        Next KeyName
                    End If
                Next strContainer
                Response.Write("<script>alert('Files Successfully downloaded！');</script>")
            Else
                Response.Write("<script>alert('Select the blobs you want to download！');</script>")
            End If
        Else
            Response.Write("<script>alert('Select the container which contains the blobs you want to download！');</script>")
        End If
    End Sub

    ''' <summary>
    ''' Gets blob based on the given Blob name and downloads to local disk 
    ''' </summary>
    ''' <param name="strBlobName"></param>
    ''' <param name="strContainer"></param>
    ''' <remarks></remarks>
    Private Sub DownLoadBlobByBlobName(strBlobName As String, strContainer As String)
        Dim filePath As String = Server.MapPath("DownLoad/")
        If Directory.Exists(filePath) = False Then
            Directory.CreateDirectory(filePath)
        End If
        'Generates  a new name
        Dim strName() As String = strBlobName.Split("/"c)
        Dim strNewName As String = ""
        If strName.Length > 0 Then
            For i As Integer = 0 To strName.Length - 1
                strNewName &= strName(i) & "_"
            Next i
        End If
        strNewName = strNewName.Substring(0, strNewName.Length - 1)
        strNewName = strContainer & "_" & strNewName

        Try
            'Download blob
            If dicBBlob.ContainsKey(strBlobName) Then
                Dim bblob As CloudBlockBlob = dicBBlob(strBlobName)
                bblob.DownloadToFile(filePath & strNewName, FileMode.Create)
            End If
            If dicPBlob.ContainsKey(strBlobName) Then
                Dim bblob As CloudPageBlob = dicPBlob(strBlobName)
                bblob.DownloadToFile(filePath & strNewName, FileMode.Create)
            End If
        Catch
        End Try
    End Sub



    ''' <summary>
    ''' Upload files
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btn_Upload_Click(sender As Object, e As EventArgs)
        'Generates  container name
        Dim strContainerName As String = txt_container.Text
        Try
            Dim blobClient As CloudBlobClient = Csa_storageAccount.CreateCloudBlobClient()
            Dim blobContainer As CloudBlobContainer = blobClient.GetContainerReference(strContainerName)

            blobContainer.CreateIfNotExists()
            blobContainer.SetPermissions(New BlobContainerPermissions With {.PublicAccess = BlobContainerPublicAccessType.Blob})

            'uplaod files
            Dim httpFiles As HttpFileCollection = Request.Files
            If httpFiles IsNot Nothing Then
                For i As Integer = 0 To httpFiles.Count - 1
                    Dim file As HttpPostedFile = httpFiles(i)

                    'Generates  a blobName
                    Dim blockName As String = file.FileName
                    Dim strName() As String = file.FileName.Split("\"c)
                    If strName.Length > 0 Then
                        blockName = strName(strName.Length - 1)
                    End If
                    If Not String.IsNullOrEmpty(blockName) Then
                        Dim blob As CloudBlockBlob = blobContainer.GetBlockBlobReference(blockName)
                        'upload files
                        blob.UploadFromStream(file.InputStream)
                    End If

                Next i
            End If


        Catch

        End Try
        'Rereads the containers and blobs  
        GetContainerListByStorageAccount(Csa_storageAccount)
        cbxl_container_SelectedIndexChanged(cbxl_container, Nothing)

    End Sub


End Class