using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface IAzureServiceBusService
    {
        Task EnqueueAsync<T>(T chatModel);
        Task<T> DequeueAsync<T>();
        Task DequeueSupportRequestsAsync();
        Task<int> GetMessageCountAsync();
    }
}