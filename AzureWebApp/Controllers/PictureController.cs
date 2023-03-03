using AzureStorage;
using AzureStorage.Models;
using AzureStorage.Services;
using AzureWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AzureWebApp.Controllers
{
    public class PictureController : Controller
    {

        private readonly INoSqlStorage<UserPicture> noSqlStorage;
        private readonly IBlobStorage blobStorage;
        public string UserId { get; set; } = "1234";
        public string City { get; set; } = "Baku";

        public PictureController(INoSqlStorage<UserPicture> noSqlStorage, IBlobStorage blobStorage)
        {
           this.noSqlStorage = noSqlStorage;
           this.blobStorage = blobStorage;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.UserId = UserId;
            ViewBag.City = City;
            List<FileBlob> blobs = new List<FileBlob>();
            var user = await noSqlStorage.Get(UserId,City);

            if (user != null)
            {
                user.Paths.ForEach(p =>
                {
                    blobs.Add(new FileBlob { Name = p, Url = blobStorage.BlobUrl + "/" + EContainerName.pictures + "/" +  p});
                });
                ViewBag.fileBlobs = blobs;
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)
        {
            List<string> picturesList = new List<string>();
            foreach (var item in pictures)
            {
                var newPictureName = $"{Guid.NewGuid()}{Path.GetExtension(item.FileName)}";

                await blobStorage.UploadAsync(item.OpenReadStream(), newPictureName, EContainerName.pictures);

                picturesList.Add(newPictureName);
            }

            var isUser = await noSqlStorage.Get(UserId, City);

            if (isUser != null)
            {
                picturesList.AddRange(isUser.Paths);
                isUser.Paths = picturesList;
            }
            else
            {
                isUser = new UserPicture();

                isUser.RowKey = UserId;
                isUser.PartitionKey = City;
                isUser.Paths = picturesList;
            }

            await noSqlStorage.Add(isUser);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddWatermark(PictureWatermarkQueue pictureWatermarkQueue)
        {
            var jsonString = JsonConvert.SerializeObject(pictureWatermarkQueue);
            string jsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
            AzureQueue azureQueue = new AzureQueue("watermarkqueue");
            await azureQueue.SendMessage(jsonBase64);
            return Ok();
        }
    }
}
