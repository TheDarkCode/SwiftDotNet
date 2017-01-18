using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;

namespace SwiftDotNet.Services
{
    public class ImageService : IImageService
    {
        private static class ImageServiceConnectionString
        {
            static string account = "account";
            static string key = "key";
            public static CloudStorageAccount GetConnectionString()
            {
                string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", account, key);
                return CloudStorageAccount.Parse(connectionString);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile imageToUpload)
        {
            string imageFullPath = null;
            if (imageToUpload == null || imageToUpload.Length == 0)
            {
                return null;
            }
            try
            {
                CloudStorageAccount cloudStorageAccount = ImageServiceConnectionString.GetConnectionString();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                        );
                }
                string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;
                await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.OpenReadStream());

                imageFullPath = cloudBlockBlob.Uri?.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
            return imageFullPath;
        }
    }
}
