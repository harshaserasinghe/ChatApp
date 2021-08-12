using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface IChatService
    {
        Task AddChatAsync(Common.Models.Chat chat);
        Task AddChatsAsync();
        Task<Common.Models.Chat> DequeueAsync();
        Task EnqueueAsync(Common.Models.Chat chat);
        Task<Common.Models.Chat> GetChatAsync(string id);
        Task AssignChatAsync(string id, int teamId, int agentId);
        Task DeleteChatsAsync();
    }
}