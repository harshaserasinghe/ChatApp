using Chat.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface ICosmosDBService
    {
        Task<T> GetEntityAsync<T>(string containerId, string partitionKey, string id);
        Task<List<T>> GetEntitiesAsync<T>(string containerId, string query);
        Task AddEntityAsync<T>(T entity, string containerId, string partitionKey);
        Task UpdateEntityAsync<T>(T entity, string containerId, string partitionKey, string id);
        Task DeleteEntityAsync<T>(string containerId, string partitionKey, string id);
    }
}