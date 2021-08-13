using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class Chat
    {
        public Chat(int chatId, string userId, string message)
        {
            Id = Guid.NewGuid().ToString();
            ChatId = chatId;
            UserId = userId;
            Message = message;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int ChatId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsAssign { get; set; }
        public int TeamId { get; set; }
        public int AgentId { get; set; }
    }
}
