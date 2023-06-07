using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

namespace BlogApp.Services
{
    public class AzureStorge
    {
        string connectionString = Environment.GetEnvironmentVariable("StoreAccountConnString");

        public async Task<string> UploadImageAsync(
            Stream imageStream,
            string imageName,
            string containerName = "blog-images"
        )
        {
            imageStream.Seek(0, SeekOrigin.Begin);
            BlobContainerClient containerClient = new BlobContainerClient(
                connectionString,
                containerName
            );
            await containerClient.CreateIfNotExistsAsync();

            string blobName = Guid.NewGuid().ToString() + imageName;

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(imageStream, true);

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeletaImageAsync(
            string imageUrl,
            string containerName = "blog-images"
        )
        {
            BlobContainerClient containerClient = new BlobContainerClient(
                connectionString,
                containerName
            );
            string imageName = Path.GetFileName(imageUrl);
            BlobClient blobClient = containerClient.GetBlobClient(imageName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                return true;
            }
            return false;
        }
    }
}