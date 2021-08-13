using Chat.Common.Models;
using Chat.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Chat.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportApi : ControllerBase
    {
        private readonly ILogger<SupportApi> logger;
        private readonly ISupportService supportService;

        public SupportApi(ILogger<SupportApi> logger,
            ISupportService supportService)
        {
            this.logger = logger;
            this.supportService = supportService;
        }

        [Route("GetSupportRequest")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string id)
        {
            var supportRequest = await supportService.GetSupportRequestAsync(id);
            return Ok(supportRequest);
        }

        [Route("AddSupportRequest")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(SupportRequest supportRequest)
        {
            await supportService.AddSupportRequestAsync(supportRequest);
            return Ok();
        }

        [Route("AddSupportRequests")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(int count)
        {
            await supportService.AddSupportRequestsAsync(count);
            return Ok();
        }

        [Route("DeleteSupportRequests")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            await supportService.DeleteSupportRequestsAsync();
            return Ok();
        }


    }
}
