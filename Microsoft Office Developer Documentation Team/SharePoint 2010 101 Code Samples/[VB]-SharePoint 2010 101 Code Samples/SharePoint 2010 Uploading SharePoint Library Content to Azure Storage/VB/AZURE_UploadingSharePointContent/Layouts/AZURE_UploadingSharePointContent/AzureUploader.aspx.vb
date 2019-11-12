Imports System
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports System.IO
'These are the Windows Azure storage namespaces
'Note: to upload these assemblies to SharePoint, go to Package/Advanced. Then add an
'Additional Assembly - select the Microsoft.WindowsAzure.StorageClient dll from the Azure SDK
Imports Microsoft.WindowsAzure.StorageClient
Imports Microsoft.WindowsAzure

Namespace Layouts.AZURE_UploadingSharePointContent

    'This application page uploads a file from a SharePoint library to Windows Azure Blob Storage
    Partial Public Class AzureUploader
        Inherits LayoutsPageBase

        'These properties are used to set up the Azure Storage Client
        Private accountName As String
        Private accountSharedKey As String
        Private defaultContainerName As String
        Private blobUri As Uri
        Private queueUri As Uri
        Private tableUri As Uri
        'This is the Windows Azure Blob Storage client
        Private cloudBlobClient As CloudBlobClient
        'This is the container in which documents get stored in Azure
        Private blobContainer As CloudBlobContainer
        'This is the blob itself, i.e. the document uploaded to Azure
        Private cloudBlob As CloudBlob

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            InitializeCloudStorage()
        End Sub

        Protected Sub btnUpload_Click(ByVal sender As Object, ByVal args As EventArgs)
            'Find out about the file we want to archive
            Dim webSite As SPWeb = SPContext.Current.Web
            Dim filePath As String = webSite.Url.ToString() + Request.QueryString("ItemUrl").ToString()
            'This is work around for the webSite.GetFile() method.
            'I found this didn't let me get the binary bite array
            Dim item As SPListItem = webSite.GetListItem(filePath)
            Dim itemfolder As SPFolder = webSite.Folders(item.ParentList.Title).SubFolders(item.Name)
            Dim fileToArchive As SPFile = itemfolder.Files(0)
            'Dim fileToArchive As SPFile = webSite.GetFile(filePath)
            'Save the binary version to upload to Azure
            Dim fileBinaryArray As Byte() = fileToArchive.OpenBinary()
            'Make sure the archived file name is unique
            Dim fileNameGuid As String = Guid.NewGuid().ToString()
            Dim newBlobName As String = String.Format(fileNameGuid + "_" + fileToArchive.Name.ToString())
            'Get or create the container
            blobContainer = cloudBlobClient.GetContainerReference(defaultContainerName)
            blobContainer.CreateIfNotExist()
            'Make sure the container is public
            Dim perms As BlobContainerPermissions = blobContainer.GetPermissions()
            perms.PublicAccess = BlobContainerPublicAccessType.Container
            blobContainer.SetPermissions(perms)
            'Create the blob
            cloudBlob = blobContainer.GetBlockBlobReference(newBlobName)
            Try
                'Upload the binary
                cloudBlob.UploadByteArray(fileBinaryArray)
                'Display the path to the blob
                resultsLabel.Text = "Blob Location: " + cloudBlob.Uri.ToString()
                resultsLabel.Text += "<br />Try pasting the above URL into the IE address bar to download the file from Azure"
            Catch ex As Exception
                resultsLabel.Text = "There was an exception uploading the file: " + ex.Message
            End Try
        End Sub

        Private Sub InitializeCloudStorage()
            'This method sets up the Windows Azure blob storage client
            'During development you can use the Storage Emulator from the SQL Azure SDK
            'When you deploy the application, you must have a Azure Storage Account set up
            'and edit the values below
            'Ideally, take these values from a web.config file rather than hardcoding them
            Try
                'Edit these values to match your Azure storage account
                accountName = "myaccountname"
                accountSharedKey = "insert the primary shared key from the Azure management portal here"
                blobUri = New Uri("http://myaccountname.blob.core.windows.net/")
                queueUri = New Uri("http://myaccountname.queue.core.windows.net/")
                tableUri = New Uri("http://myaccountname.table.core.windows.net/")
                'Use any unique string for the container name. However it must be all lower case
                defaultContainerName = "sharepointdocs"

                'display the name of the file we want to upload
                Me.docFileName.Text = Request.QueryString("ItemUrl").ToString()

                'For the Local storage emulator, we actually don't need the above values:
                Dim azAccount As CloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount
                'For deployment, uncomment this code:
                'Dim azAccount As CloudStorageAccount = New CloudStorageAccount( _
                '     New StorageCredentialsAccountAndKey(accountName, accountSharedKey), _
                '    blobUri, _
                '    queueUri, _
                '    tableUri)

                'Finally, create the client
                cloudBlobClient = azAccount.CreateCloudBlobClient()
            Catch ex As Exception
                resultsLabel.Text = "Error Initializing Cloud Storage: " + ex.Message
            End Try
        End Sub

    End Class

End Namespace
