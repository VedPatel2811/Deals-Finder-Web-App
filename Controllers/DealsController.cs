using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Lab5.Models.ViewModels;
using Lab5.Models;
using Lab5.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Lab5.Controllers
{
    public class DealsController : Controller
    {
        private readonly DealsFinderDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "deals";

        public DealsController(DealsFinderDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Deals/Index/{id}
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            await EnsureContainerExistsAsync();

            var service = await _context.FoodDeliveryServices.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var prefix = $"{id}/";

            var deals = new List<Deal>();

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);

                deals.Add(new Deal
                {
                    Id = blobItem.Name.GetHashCode(), // Temporary ID for view purposes
                    FileName = blobItem.Name.Replace(prefix, ""),
                    FoodDeliveryServiceId = id,
                    Url = blobClient.Uri.ToString()
                });
            }

            var model = new DealsPostsViewModel
            {
                FoodDeliveryService = service,
                Deals = deals
            };

            return View(model);
        }

        // GET: Deals/Create/{id}
        [HttpGet]
        public IActionResult Create(string id)
        {
            var service = _context.FoodDeliveryServices.Find(id);
            if (service == null)
            {
                return NotFound();
            }

            var model = new FileInputViewModel
            {
                FoodDeliveryServiceId = id,
                FoodDeliveryServiceTitle = service.Title
            };

            return View(model);
        }

        // POST: Deals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileInputViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid file.");
                return View(model);
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobName = $"{model.FoodDeliveryServiceId}/{Guid.NewGuid()}_{model.File.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = model.File.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = model.File.ContentType
                });
            }

            // Save to database
            var deal = new Deal
            {
                FoodDeliveryServiceId = model.FoodDeliveryServiceId,
                FileName = model.File.FileName,
                Url = blobClient.Uri.ToString()
            };

            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = model.FoodDeliveryServiceId });
        }

        [HttpGet]
        [Route("Deals/Delete/{id}/{fileName}")]
        public IActionResult Delete(string id, string fileName)
        {
            fileName = WebUtility.UrlDecode(fileName);
            var service = _context.FoodDeliveryServices.Find(id);
            if (service == null) return NotFound();

            var model = new FileInputViewModel
            {
                FoodDeliveryServiceId = id,
                FoodDeliveryServiceTitle = service.Title
            };

            ViewBag.FileName = fileName;
            return View(model);
        }

        [HttpPost]
        [Route("Deals/Delete/{id}/{fileName}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, string fileName)
        {
            fileName = WebUtility.UrlDecode(fileName);
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid parameters");

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient($"{id}/{fileName}");

            await blobClient.DeleteIfExistsAsync();
            var deal = await _context.Deals
                .FirstOrDefaultAsync(d => d.FoodDeliveryServiceId == id && d.FileName == fileName);
            if (deal != null)
            {
                _context.Deals.Remove(deal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { id });
        }

        private async Task EnsureContainerExistsAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }
    }
}