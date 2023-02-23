using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage
{
    public enum EContainerName
    {
        Picture,
        Pdf
    }
    public interface IBlobStorage
    {
        public string BlobUrl { get; }
        Task UploadAsync(Stream fileStream, string fileName, EContainerName eContainerName);
        Task<Stream> DownloadAsync(string fileName, EContainerName eContainerName);
        Task DeleteAsync(string fileName, EContainerName eContainerName);
        Task SetLogAsync(string text, string fileName);
        Task<List<string>> GetLogAsync(string fileName);
        List<string> GetNames(EContainerName eContainerName);
    }
}
