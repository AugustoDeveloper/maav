using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("healthcheck")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetHealthAsync()
        {
            return await Task.FromResult(Ok("For now, is ok!"));
        }
    }
}