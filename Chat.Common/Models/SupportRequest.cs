using Newtonsoft.Json;
using System;

namespace Chat.Common.Models
{
    public class SupportRequest
    {
        public SupportRequest(int supportRequestId, string userId, string message)
        {
            Id = supportRequestId.ToString();//Guid.NewGuid().ToString();
            SupportRequestId = supportRequestId;
            UserId = userId;
            Message = message;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int SupportRequestId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsAssign { get; set; }
        public int TeamId { get; set; }
        public int AgentId { get; set; }
    }
}
