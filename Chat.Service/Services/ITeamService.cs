using Chat.Common.Models;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface ITeamService
    {
        Task AddTeamsAsync();
        Task<Team> GetAssignedTeamAsync();
        Task ChangeShiftAsync(string id);
        Task<Team> GetTeamAsync(string id);
        Task AssignChatToTeamAsync(Common.Models.Chat chat, Team team);
        int GetTeamCapacity(Team teamModel);
        Task ShowTeam();
    }
}