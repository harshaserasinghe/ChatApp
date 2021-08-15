using System.Threading.Tasks;

namespace Chat.Service.Interfaces
{
    public interface IAzureServiceBusService
    {
        Task EnqueueAsync<T>(T entity);
        Task<T> DequeueAsync<T>();
        Task DequeueSupportRequestsAsync();
        Task<int> GetMessageCountAsync();
    }
}