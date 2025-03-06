using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacifyFunctions.Helpers
{
    public class StorageHelper
    {

        private readonly String connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly String containerName;
        private BlobContainerClient blobContainerClient;

        public StorageHelper(String containerName)
        {
            this.containerName = containerName;
            InitStorageAccount().GetAwaiter().GetResult();
        }

        public async Task InitStorageAccount()
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();
        }

        public async Task<string> UploadToBlobAsync(byte[] imageBytes, string fileName)
        {
            try
            {
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                using var stream = new MemoryStream(imageBytes);

                await blobClient.UploadAsync(stream, overwrite: true);
                return blobClient.Uri.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
