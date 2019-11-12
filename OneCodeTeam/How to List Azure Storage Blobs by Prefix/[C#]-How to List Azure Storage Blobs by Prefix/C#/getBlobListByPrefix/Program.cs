using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace getBlobListByPrefix
{
    class Program
    {
        static void Main(string[] args)
        {
            // Name of storage account that will do the processing
            const string storageAccountName = "enteryourstorageaccountname";

            // Secondary storage account key
            const string storageAccountKey = "enteryouraccountkeyXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX8vA==";

            // Container name where you want to store the results of the job
            const string storageContainerName = "enteryourcontainername";

            string storageConnectionString =
                $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Next select the container which you are looking for your old blobs.
            CloudBlobContainer container = blobClient.GetContainerReference(storageContainerName);

            //specify a prefix you want to use
            string prefix = "poll--";

            var rsltList = ListBlobsFromContainer(container, prefix);
            foreach (Uri uri in rsltList)
            {
                Console.WriteLine("\tBlob:" + uri);

                //you can delete all blobs here by enable below line
                //blobClient.GetBlobReferenceFromServer(uri).DeleteIfExists();
            }

            Console.WriteLine("Press any key to contine...");
            Console.ReadKey();
        }

        private static List<Uri> ListBlobsFromContainer(CloudBlobContainer container, string prefix)
        {
            var lstBlobUri = new List<Uri>();

            Console.WriteLine("List blobs in the container by prefix.");

            try
            {
                // The prefix is optional when listing blobs in a container.
                // List blobs in a hierarchically listing.
                foreach (var blob in container.ListBlobs(prefix, false, BlobListingDetails.None, null, null))
                {
                    //Console.WriteLine("\tBlob:" + blob.Uri);
                    lstBlobUri.Add(blob.Uri);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
            return lstBlobUri;
        }
    }
}
