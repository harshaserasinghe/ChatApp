using Chat.Common.Models;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface ISupportService
    {
        Task AddSupportRequestAsync(SupportRequest chat);
        Task AddSupportRequestsAsync(int count);
        Task<SupportRequest> DequeueSupportRequestAsync();
        Task EnqueueSupportRequestAsync(SupportRequest chat);
        Task<SupportRequest> GetSupportRequestAsync(string id);
        Task UpdateSupportRequestAsync(SupportRequest supportRequest, int teamId, int agentId);
        Task DeleteSupportRequestsAsync();
        Task<int> GetMessageCountAsync();
    }
}