using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage.Services
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient blobServiceClient;
        public BlobStorage()
        {
            blobServiceClient = new BlobServiceClient(ConnectionString.AzureStorageConnectionString);
        }
        public string BlobUrl => "https://udemyrealstrorageaccount.blob.core.windows.net";

        public async Task DeleteAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

        public async Task<Stream> DownloadAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            var blobClient = containerClient.GetBlobClient(fileName);
            var file = await blobClient.DownloadAsync();
            return file.Value.Content;
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            List<string> logs = new List<string>();
            var containerClient = blobServiceClient.GetBlobContainerClient(EContainerName.Logs.ToString());
            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);
            await appendBlobClient.CreateIfNotExistsAsync();
            var info = await appendBlobClient.DownloadStreamingAsync();
            using StreamReader sr = new StreamReader(info.Value.Content);
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                logs.Add(line);
            }
            return logs;

        }

        public List<string> GetNames(EContainerName eContainerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(EContainerName.Logs.ToString());
            List<string> names = new List<string>();
            var blobs = containerClient.GetBlobs();
            names.AddRange(blobs.Select(x => x.Name));
            return names;
        }

        public async Task SetLogAsync(string text, string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(EContainerName.Logs.ToString());
            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);
            await appendBlobClient.CreateIfNotExistsAsync();
            using MemoryStream ms = new MemoryStream();
            using StreamWriter sr = new StreamWriter(ms);
            sr.Write($"{DateTime.Now} : {text} /n");
            sr.Flush();
            ms.Position = 0;
            await appendBlobClient.AppendBlockAsync(ms);
        }

        public async Task UploadAsync(Stream fileStream, string fileName, EContainerName eContainerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
        }
    }
}
