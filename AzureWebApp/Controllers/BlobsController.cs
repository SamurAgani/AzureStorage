using AzureStorage;
using AzureWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWebApp.Controllers
{
    public class BlobsController : Controller
    {
        public readonly IBlobStorage blobStorage;

        public BlobsController(IBlobStorage blobStorage)
        {
            this.blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            var names = blobStorage.GetNames(EContainerName.pictures);
            string blobUrl = $"{blobStorage.BlobUrl}/{EContainerName.pictures.ToString()}";
            ViewBag.blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}" }).ToList();
            var a = await blobStorage.GetLogAsync("controller.txt"); 
            ViewBag.logs = await blobStorage.GetLogAsync("controller.txt");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            await blobStorage.SetLogAsync("Upload worked","controller.txt");
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            await blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);
            await blobStorage.SetLogAsync("Out from upload","controller.txt");
            return RedirectToAction("Index");

        }
        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await blobStorage.DownloadAsync(fileName, EContainerName.pictures);

            return File(stream, "application/octet-stream", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string fileName)
        {
            await blobStorage.DeleteAsync(fileName, EContainerName.pictures);
            return RedirectToAction("Index");
        }
    }
}
