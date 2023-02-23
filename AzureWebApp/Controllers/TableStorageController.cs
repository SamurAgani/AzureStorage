using AzureStorage;
using AzureStorage.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWebApp.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly INoSqlStorage<Product> noSqlStorage;

        public TableStorageController(INoSqlStorage<Product> noSqlStorage)
        {
            this.noSqlStorage = noSqlStorage;
        }
        public IActionResult Index()
        {
            ViewBag.IsUpdate = false;
            ViewBag.products = noSqlStorage.GetAll().ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Random";
            ViewBag.IsUpdate = false;
            await noSqlStorage.Add(product);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(string rowKey, string partitionKey)
        {
            var product = await noSqlStorage.Get(rowKey, partitionKey);
            ViewBag.products = noSqlStorage.GetAll().ToList();
            ViewBag.IsUpdate = true;

            return View("Index", product);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            product.ETag = "*";
            ViewBag.IsUpdate = true;

            await noSqlStorage.Update(product);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string rowKey, string partitionKey)
        {
            await noSqlStorage.Delete(rowKey, partitionKey);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Query(int price)
        {
            ViewBag.IsUpdate = false;
            ViewBag.products = noSqlStorage.Query(x => x.Price > price).ToList();

            return View("Index");
        }
    }
}
