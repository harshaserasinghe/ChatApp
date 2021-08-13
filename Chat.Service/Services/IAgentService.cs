using Chat.Common.Models;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public interface IAgentService
    {
        Task AddTeamsAsync();
        Task<Team> GetAssignedTeamAsync();
        Task ChangeTeamAsync(string name);
        Task<Team> GetTeamAsync(string id);
        Task AssignSupportRequestToTeamAsync(SupportRequest supportRequest, Team team);
        Task<int> GetCapacity();
        Task DeleteTeamsAsync();
    }
}