using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Configuration;



namespace AzureBlobDemo
{
    // illustration of writing to Azure BLOB storage based on this blog:
    // https://www.simple-talk.com/cloud/cloud-data/an-introduction-to-windows-azure-blob-storage-/
    class Program
    {
        static void Main(string[] args)
        {
            string accountName = ConfigurationManager.AppSettings["AccountName"];
            string accountKey = ConfigurationManager.AppSettings["AccountKey"];
            string uploadPath = @"C:\test\azb\cliff_drop.jpg";
            string blobRef = "APictureFile.jpg";
            string downloadPath = @"C:\test\azb\download\cliff_drop.jpg";

            // (1) upload
            Upload(uploadPath, blobRef, accountName, accountKey);

            // (2) download
            Download(downloadPath, blobRef, accountName, accountKey);

            Console.WriteLine("Done... press a key to end.");
            Console.ReadKey();
        }
        static public void Upload(string filepath, string blobname, string accountName, string accountKey)
        {
            try
            {
                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
                CloudBlobClient client = account.CreateCloudBlobClient();
                CloudBlobContainer sampleContainer = client.GetContainerReference("public-samples");
                sampleContainer.CreateIfNotExists();

                // for public access ////
                BlobContainerPermissions permissions = new BlobContainerPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                sampleContainer.SetPermissions(permissions);
                /////////////////////////

                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(blobname);
                using (Stream file = File.OpenRead(filepath))
                {
                    blob.UploadFromStream(file);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static public void Download(string filepath, string blobname, string accountName, string accountKey)
        {
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer sampleContainer = client.GetContainerReference("public-samples");

            CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(blobname);
            blob.DownloadToFile(filepath, FileMode.Create);
        }
    }
}
