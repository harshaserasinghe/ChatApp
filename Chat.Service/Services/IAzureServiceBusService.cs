using Chat.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface IAzureServiceBusService
    {
        Task EnqueueAsync(Common.Models.Chat chatModel);
        Task<Common.Models.Chat> DequeueAsync();
        int GetActiveMessageCount();
        //Task DequeueAllAsync();
    }
}