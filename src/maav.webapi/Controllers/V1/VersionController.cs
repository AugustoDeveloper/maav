using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAAV.Application;
using MAAV.Application.Exceptions;
using MAAV.DataContracts;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1"), Authorize]
    public class VersionController : ControllerBase
    {
        [HttpGet("{organisationId}/teams/{teamId}/apps/{appId}/versions/{keyBranchName}"), Authorize(Roles = "user,admin,developer,team-leader")]
        public async Task<IActionResult> LoadHistoryVersionsAsync(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromRoute] string appId,
            [FromRoute] string keyBranchName,
            [FromServices] IVersionService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId) || string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(keyBranchName))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            var result = await service.LoadHistoryFromKeyBranchName(organisationId, teamId, appId, keyBranchName);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("{organisationId}/teams/{teamId}/apps/{appId}/version"), Authorize(Roles = "user,admin,integration,developer,team-leader")]
        public async Task<IActionResult> GenerateVersionAsync(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromRoute] string appId,
            [FromBody] BranchActionRequest data,
            [FromServices] IVersionService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId) || string.IsNullOrWhiteSpace(appId) || data == null)
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            if (string.IsNullOrWhiteSpace(data.From))
            {
                return BadRequest("Source branch must have valid value!");
            }

            if (string.IsNullOrWhiteSpace(data.To))
            {
                var resultGet = await service.GetVersionFromSourceBranchAsync(organisationId, teamId, appId, data);
                if (resultGet == null)
                {
                    return NotFound();
                }
                return Ok(resultGet);
            }

            var resultUpdate = await service.UpdateVersionOnBranches(organisationId, teamId, appId, data);
            if (resultUpdate == null)
            {
                return NotFound();
            }

            return Ok(resultUpdate);
        }
    }
}