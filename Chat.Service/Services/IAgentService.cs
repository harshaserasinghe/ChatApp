using Chat.Common.Models;

namespace Chat.Service.Services
{
    public interface IAgentService
    {
        void AssignTeam(Team teamModel);
        void ShowTeam();
        void AssignChat(Common.Models.Chat chatModel);
        int GetTeamCapacity(Team teamModel);
    }
}