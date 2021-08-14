using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Chat.Common.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Chat.Service.Services
{
    public class CosmosDBService : ICosmosDBService
    {
        private readonly CosmosClient cosmosClient;
        private readonly CosmoDBConfig cosomoDBConfig;

        public CosmosDBService(IOptions<CosmoDBConfig> cosomoDBConfig)
        {
            this.cosomoDBConfig = cosomoDBConfig.Value;
            cosmosClient = new CosmosClient(this.cosomoDBConfig.ConnectionString);
        }
        private Container GetContainer(string containerId) =>
            cosmosClient.GetContainer(cosomoDBConfig.DataBaseId, containerId);

        public async Task<T> GetEntityAsync<T>(string containerId, string partitionKey, string id)
        {
            var container = GetContainer(containerId);
            return await container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        public async Task AddEntityAsync<T>(T entity, string containerId, string partitionKey)
        {
            var container = GetContainer(containerId);
            await container.CreateItemAsync<T>(entity);
        }

        public async Task UpdateEntityAsync<T>(T entity, string containerId, string partitionKey, string id)
        {
            var container = GetContainer(containerId);
            await container.ReplaceItemAsync<T>(entity, id, new PartitionKey(partitionKey));
        }


        public async Task<List<T>> GetEntitiesAsync<T>(string containerId, string query)
        {

            var queryDefinition = new QueryDefinition(query);
            var container = GetContainer(containerId);
            List<T> entities = new List<T>();
            var queryResultSetIterator = container.GetItemQueryIterator<T>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (T entity in currentResultSet)
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task DeleteEntityAsync<T>(string containerId, string partitionKey, string id)
        {
            var container = GetContainer(containerId);
            await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }
    }
}
