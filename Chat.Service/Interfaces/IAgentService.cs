using Chat.Common.Models;
using System.Threading.Tasks;

namespace Chat.Service.Interfaces
{
    public interface IAgentService
    {
        Task AddTeamsAsync();
        Task<Team> GetAssignedTeamAsync();
        Task ChangeTeamAsync(string name);
        Task<Team> GetTeamAsync(string id);
        Task<Team> GetTeamByNameAsync(string name);
        Task AssignSupportRequestToAgentAsync(SupportRequest supportRequest, Team team);
        Task<int> GetCapacity();
        bool IsAllAgentsBusy(Team team);
        Task DeleteAllTeamsAsync();
        Task RestAllAgents();
    }
}