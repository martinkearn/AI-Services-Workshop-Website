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

namespace AISW_Web.Controllers
{
    public class LinkController : Controller
    {
        private readonly AppSecretSettings _appSecretSettings;

        public LinkController(IOptions<AppSecretSettings> appSecretSettings)
        {
            _appSecretSettings = appSecretSettings.Value;
        }

        // GET: Link
        public async Task<ActionResult> Index()
        {
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSecretSettings.TableStorageContainerName);

            TableContinuationToken token = null;

            var entities = new List<TableEntityAdapter<Link>>();
            TableQuery<TableEntityAdapter<Link>> query = new TableQuery<TableEntityAdapter<Link>>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            // create list of objects from the storage entities
            var links = new List<Link>();
            foreach (var entity in entities)
            {
                links.Add(entity.OriginalEntity);
            }

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

                var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSecretSettings.TableStorageContainerName);

                // Create the batch operation.
                TableBatchOperation batchOperation = new TableBatchOperation();

                // Creat a TableEntityAdapter based on the item
                TableEntityAdapter<Link> entity = new TableEntityAdapter<Link>(item, _appSecretSettings.TableStoragePartitionKey, item.Label);
                batchOperation.InsertOrReplace(entity);

                // Execute the batch operation.
                await table.ExecuteBatchAsync(batchOperation);

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
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSecretSettings.TableStorageContainerName);

            // Create a retrieve operation that takes an entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Link>>(_appSecretSettings.TableStoragePartitionKey, id);

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                // get the result and create a new object from the data
                var deviceResult = (TableEntityAdapter<Link>)retrievedResult.Result;

                var link = deviceResult.OriginalEntity;

                return View(link);
            }
            else
            {
                return View();
            }


        }

        // POST: Link/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                //get cloudtable
                var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSecretSettings.TableStorageContainerName);

                // Create a retrieve operation that expects a the right entity.
                TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Link>>(_appSecretSettings.TableStoragePartitionKey, id);

                // Execute the operation.
                TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

                // Assign the result to an Entity.
                var deleteEntity = (TableEntityAdapter<Link>)retrievedResult.Result;

                // Create the Delete TableOperation.
                if (deleteEntity != null)
                {
                    // Create the Delete TableOperation.
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                    // Execute the operation.
                    await table.ExecuteAsync(deleteOperation);
                }

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