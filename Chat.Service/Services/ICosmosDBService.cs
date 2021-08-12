using Chat.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface ICosmosDBService
    {
        Task<T> GetEntity<T>(string containerId, string partitionKey, string id);
        Task<List<T>> GetEntities<T>(string containerId, string query);
        Task AddEntity<T>(T entity, string containerId, string partitionKey);
        Task UpdateEntity<T>(T entity, string containerId, string partitionKey, string id);
        Task DeleteEntity<T>(string containerId, string partitionKey, string id);
    }
}