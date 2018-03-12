using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISW_Web.Models;
using AISW_Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Options;
using AISW_Web.Interfaces;

namespace AISW_Web.Controllers
{
    public class LinksController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public LinksController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        // GET: Link
        public async Task<ActionResult> Index()
        {
            var links = await _storeRepository.GetLinks();

            return View(links);
        }

        // GET: Link/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Link/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var item = CastFormCollection(collection);

                await _storeRepository.StoreLink(item);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // GET: Link/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var link = await _storeRepository.GetLink(id);

            return View(link);
        }

        // POST: Link/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                await _storeRepository.DeleteLink(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private Link CastFormCollection(IFormCollection collection)
        {
            var link = new Link()
            {
                Url = collection["Url"],
                Label = collection["Label"],
                Description = collection["Description"]
            };

            return link;
        }

        private async Task<CloudTable> GetCloudTable(string tableConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(tableConnectionString);

            var blobClient = storageAccount.CreateCloudTableClient();

            var table = blobClient.GetTableReference(containerName);

            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}