using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface ISupportService
    {
        Task AddSupportRequestAsync(Common.Models.SupportRequest chat);
        Task AddSupportRequestsAsync(int count);
        Task<Common.Models.SupportRequest> DequeueSupportRequestAsync();
        Task EnqueueSupportRequestAsync(Common.Models.SupportRequest chat);
        Task<Common.Models.SupportRequest> GetSupportRequestAsync(string id);
        Task AssignSupportRequestAsync(string id, int teamId, int agentId);
        Task DeleteSupportRequestsAsync();
    }
}