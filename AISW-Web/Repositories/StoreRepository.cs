using AISW_Web.Interfaces;
using AISW_Web.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISW_Web.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppSecretSettings _appSecretSettings;

        public StoreRepository(IOptions<AppSecretSettings> appSecretSettings)
        {
            _appSecretSettings = appSecretSettings.Value;
        }

        public async Task StoreLink(Link item)
        {
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSecretSettings.TableStorageContainerName);

            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            // Create a TableEntityAdapter based on the item
            TableEntityAdapter<Link> entity = new TableEntityAdapter<Link>(item, _appSecretSettings.TableStoragePartitionKey, GetRowKey(item.Label));
            batchOperation.InsertOrReplace(entity);

            // Execute the batch operation.
            await table.ExecuteBatchAsync(batchOperation);
        }

        public async Task DeleteLink(string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSecretSettings.TableStorageContainerName);

            // Create a retrieve operation that expects a the right entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Link>>(_appSecretSettings.TableStoragePartitionKey, GetRowKey(id));

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
        }


        public async Task<List<Link>> GetLinks()
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

            return links;
        }

        public async Task<Link> GetLink(string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSecretSettings.TableStorageContainerName);

            // Create a retrieve operation that takes an entity.GetRowKey
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Link>>(_appSecretSettings.TableStoragePartitionKey, GetRowKey(id));

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                // get the result and create a new object from the data
                var deviceResult = (TableEntityAdapter<Link>)retrievedResult.Result;

                var link = deviceResult.OriginalEntity;

                return link;
            }
            else
            {
                return null;
            }
        }

        private string GetRowKey(string id) => id.ToLower();


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
