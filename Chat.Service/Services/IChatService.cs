using Chat.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface IChatService
    {
        Task<Common.Models.Chat> GetChatAsync();
        Task SetChatAsync(Common.Models.Chat chatModel);
    }
}