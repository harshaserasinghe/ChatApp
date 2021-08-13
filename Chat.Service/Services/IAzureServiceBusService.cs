using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface IAzureServiceBusService
    {
        Task EnqueueAsync(Common.Models.SupportRequest chatModel);
        Task<Common.Models.SupportRequest> DequeueAsync();
        Task DequeueSupportRequestsAsync();
        Task<int> GetMessageCountAsync();
    }
}