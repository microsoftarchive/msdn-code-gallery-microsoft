using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure;
using System.IO;
using Microsoft.WindowsAzure;

namespace tstcopy2
{
    class Program
    {
        static void Main(string[] args)
        {

            CloudStorageAccount sourceStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("sourceStorageConnectionString"));
            CloudStorageAccount targetStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("targetStorageConnectionString"));

            CloudBlobClient sourceCloudBlobClient = sourceStorageAccount.CreateCloudBlobClient();
            CloudBlobClient targetCloudBlobClient = targetStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer sourceContainer = sourceCloudBlobClient.GetContainerReference(CloudConfigurationManager.GetSetting("sourceContainer"));
            CloudBlobContainer targetContainer = targetCloudBlobClient.GetContainerReference(CloudConfigurationManager.GetSetting("targetContainer"));
            targetContainer.CreateIfNotExists();

            // Copy each blob
            foreach (IListBlobItem blob in sourceContainer.ListBlobs(useFlatBlobListing: true))
            {

                Uri thisBlobUri = blob.Uri;

                var blobName = Path.GetFileName(thisBlobUri.ToString());
                Console.WriteLine("Copying blob: " + blobName);

                CloudBlockBlob sourceBlob = sourceContainer.GetBlockBlobReference(blobName);
                CloudBlockBlob targetBlob = targetContainer.GetBlockBlobReference(blobName);

                Task task = TransferManager.CopyAsync(sourceBlob, targetBlob, true /* isServiceCopy */);

            }
            Console.WriteLine("Press any key to continue:");
            Console.Read();

        }
    }
}
