using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
//These are the Windows Azure storage namespaces
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;


namespace AZURE_UploadingSharePointContent.Layouts.AZURE_UploadingSharePointContent
{
    //This application page uploads a file from a SharePoint library to Windows Azure Blob Storage
    public partial class AzureUploader : LayoutsPageBase
    {

        //These properties are used to set up the Azure Storage Client
        private string accountName;
        private string accountSharedKey;
        private string defaultContainerName;
        private Uri blobUri;
        private Uri queueUri;
        private Uri tableUri;
        //This is the Windows Azure Blob Storage client
        private CloudBlobClient cloudBlobClient;
        //This is the container in which documents get stored in Azure
        private CloudBlobContainer blobContainer;
        //This is the blob itself, i.e. the document uploaded to Azure
        private CloudBlob cloudBlob;

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeCloudStorage();
        }

        protected void btnUpload_Click(object sender, EventArgs args)
        {
            //Find out about the file we want to archive
            SPWeb webSite = SPContext.Current.Web;
            string filePath = webSite.Url.ToString() + Request.QueryString["ItemUrl"].ToString();
            //This is work around for the webSite.GetFile() method.
            //I found this didn't let me get the binary bite array
            SPListItem item = webSite.GetListItem(filePath);
            SPFolder itemfolder = webSite.Folders[item.ParentList.Title].SubFolders[item.Name];
            SPFile fileToArchive = itemfolder.Files[0];
            //SPFile fileToArchive = webSite.GetFile(filePath);
            //Save the binary version to upload to Azure
            byte[] fileBinaryArray = fileToArchive.OpenBinary();
            //Make sure the archived file name is unique
            string fileNameGuid = Guid.NewGuid().ToString();
            string newBlobName = string.Format(fileNameGuid + "_" + fileToArchive.Name.ToString());
            //Get or create the container
            blobContainer = cloudBlobClient.GetContainerReference(defaultContainerName);
            blobContainer.CreateIfNotExist();
            //Make sure the container is public
            var perms = blobContainer.GetPermissions();
            perms.PublicAccess = BlobContainerPublicAccessType.Container;
            blobContainer.SetPermissions(perms);
            //Create the blob
            cloudBlob = blobContainer.GetBlockBlobReference(newBlobName);
            try
            {
                //Upload the binary
                cloudBlob.UploadByteArray(fileBinaryArray);
                //Display the path to the blob
                resultsLabel.Text = "Blob Location: " + cloudBlob.Uri.ToString();
                resultsLabel.Text += "<br />Try pasting the above URL into the IE address bar to download the file from Azure";
             }
             catch (Exception ex)
             {
                 resultsLabel.Text = "There was an exception uploading the file: " + ex.Message;
             }            
        }

        private void InitializeCloudStorage()
        {
            //This method sets up the Windows Azure blob storage client
            //During development you can use the Storage Emulator from the SQL Azure SDK
            //When you deploy the application, you must have a Azure Storage Account set up
            //and edit the values below
            //Ideally, take these values from a web.config file rather than hardcoding them
            try
            {
                //Edit these values to match your Azure storage account
                accountName = "myaccountname";
                accountSharedKey = "insert the primary shared key from the Azure management portal here";
                blobUri = new Uri("http://myaccountname.blob.core.windows.net/");
                queueUri = new Uri("http://myaccountname.queue.core.windows.net/");
                tableUri = new Uri("http://myaccountname.table.core.windows.net/");
                //Use any unique string for the container name. However it must be all lower case
                defaultContainerName = "sharepointdocs";

                //display the name of the file we want to upload
                this.docFileName.Text = Request.QueryString["ItemUrl"].ToString();

                //For the Local storage emulator, we actually don't need the above values:
                CloudStorageAccount azAccount = CloudStorageAccount.DevelopmentStorageAccount; 
                //For deployment, uncomment this code:
                //CloudStorageAccount azAccount = new CloudStorageAccount
                //(
                //    new StorageCredentialsAccountAndKey(accountName, accountSharedKey),
                //    blobUri,
                //    queueUri,
                //    tableUri
                //);

                //Finally, create the client
                cloudBlobClient = azAccount.CreateCloudBlobClient();
            }
            catch (Exception ex)
            {
                resultsLabel.Text = "Error Initializing Cloud Storage: " + ex.Message;
            }
        }
    }
}
