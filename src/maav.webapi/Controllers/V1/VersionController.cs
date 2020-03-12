using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAAV.Application;
using MAAV.Application.Exceptions;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1"), Authorize]
    public class VersionController : ControllerBase
    {
        [HttpPost("{organisationName}/teams/{teamName}/apps/{appName}/version"), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> GenerateVersionAsync(
            [FromRoute] string organisationName,
            [FromRoute] string teamName,
            [FromRoute] string appName,
            [FromQuery] string source,
            [FromQuery] string target,
            [FromBody] object data,
            [FromServices] IVersionService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(teamName) || string.IsNullOrWhiteSpace(appName))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                return BadRequest("Source branch must have valid value!");
            }
            
            if (string.IsNullOrWhiteSpace(target))
            {
                return Ok(await service.GetVersionFromSourceBranchAsync(organisationName, teamName, appName, source, data));
            }

            return Ok(await service.GetVersionFromSourceAndTagetBranchAsync(organisationName, teamName, appName, source, target, data));
        }
    }
}