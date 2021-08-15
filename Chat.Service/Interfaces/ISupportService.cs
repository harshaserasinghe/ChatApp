using Chat.Common.Models;
using System.Threading.Tasks;

namespace Chat.Service.Interfaces
{
    public interface ISupportService
    {
        Task AddSupportRequestAsync(SupportRequest chat);
        Task AddSupportRequestsAsync(int count);
        Task<SupportRequest> DequeueSupportRequestAsync();
        Task EnqueueSupportRequestAsync(SupportRequest chat);
        Task<SupportRequest> GetSupportRequestAsync(string id);
        Task UpdateSupportRequestAsync(SupportRequest supportRequest);
        Task DeleteSupportAllRequestsAsync();
        Task<bool> IsSupportRequestsAvailableAsync();
    }
}