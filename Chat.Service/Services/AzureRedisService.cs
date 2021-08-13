using Chat.Common.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class AzureRedisService : IAzureRedisService
    {
        private readonly AzureRedisConfig azureRedisConfig;
        private readonly Lazy<ConnectionMultiplexer> lazyConnection;
        public AzureRedisService(IOptions<AzureRedisConfig> azureRedisConfig)
        {
            this.azureRedisConfig = azureRedisConfig.Value;
            lazyConnection = CreateConnection();
        }

        private Lazy<ConnectionMultiplexer> CreateConnection()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = azureRedisConfig.ConnectionString;
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public void SetEntity<T>(string key, T entity)
        {
            var cache = lazyConnection.Value.GetDatabase();
            var value = JsonSerializer.Serialize(entity);
            cache.StringSet(key, value);
        }

        public T GetEntity<T>(string key)
        {
            var cache = lazyConnection.Value.GetDatabase();
            var value = cache.StringGet(key);
            var entity = JsonSerializer.Deserialize<T>(value);
            return entity;
        }

        public bool IsEntityExists(string key)
        {
            var cache = lazyConnection.Value.GetDatabase();
            return cache.KeyExists(key);
        }
    }
}
