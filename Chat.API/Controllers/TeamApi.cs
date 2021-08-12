using Chat.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Chat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamApi : ControllerBase
    {
        private readonly ILogger<TeamApi> logger;
        private readonly ITeamService teamService;

        public TeamApi(ILogger<TeamApi> logger,
            ITeamService teamService)
        {
            this.logger = logger;
            this.teamService = teamService;
        }

        [Route("GetTeam")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string id)
        {
            var team = await teamService.GetTeamAsync(id);
            return Ok(team);
        }

        [Route("AddTeams")]
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            await teamService.AddTeamsAsync();
            return Ok();
        }

        [Route("ChangeShift")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(string id)
        {
            await teamService.ChangeShiftAsync(id);
            return Ok();
        }
    }
}
