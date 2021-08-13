using Chat.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Chat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentApi : ControllerBase
    {
        private readonly ILogger<AgentApi> logger;
        private readonly IAgentService agentService;

        public AgentApi(ILogger<AgentApi> logger,
            IAgentService teamService)
        {
            this.logger = logger;
            this.agentService = teamService;
        }

        [Route("GetTeam")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string id)
        {
            var team = await agentService.GetTeamAsync(id);
            return Ok(team);
        }

        [Route("AddTeams")]
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            await agentService.AddTeamsAsync();
            return Ok();
        }

        [Route("ChangeTeam")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(string name)
        {
            await agentService.ChangeTeamAsync(name);
            return Ok();
        }

        [Route("DeleteTeams")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            await agentService.DeleteTeamsAsync();
            return Ok();
        }
    }
}
