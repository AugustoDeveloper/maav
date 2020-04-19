using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAAV.Application;
using MAAV.Application.Exceptions;
using System.Text.Json;
using MAAV.WebAPI.Serializers;
using MAAV.DataContracts.GitHub;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1"), Authorize]
    public class ApplicationController : ControllerBase
    {
        private JsonSerializerOptions optionSerialization = new JsonSerializerOptions
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
        };

        [HttpGet("{organisationId}/teams/{teamId}/apps/{appId}", Name = nameof(GetApplicationAsync)), Authorize(Roles = "user,developer,admin,team-leader")]
        public async Task<IActionResult> GetApplicationAsync(
            [FromRoute] string organisationId, 
            [FromRoute] string teamId, 
            [FromRoute] string appId, 
            [FromServices] IApplicationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId) || string.IsNullOrWhiteSpace(appId))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            try
            {
                var app = await service.GetByIdAsync(organisationId, teamId, appId);
                if (app == null)
                {
                    return NotFound();
                }

                return Ok(app);
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{organisationId}/teams/{teamId}/apps"), Authorize(Roles = "user,developer,admin,team-leader")]
        public async Task<IActionResult> LoadAllApplicationFromTeam(
            [FromRoute] string organisationId, 
            [FromRoute] string teamId, 
            [FromServices] IApplicationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            try
            {
                var apps = await service.LoadAllFromTeamAsync(organisationId, teamId);

                return Ok(apps);
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("{organisationId}/teams/{teamId}/apps"), Authorize(Roles = "user,developer,admin,team-leader")]
        public async Task<IActionResult> AddApplicationAsync(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromBody] DataContracts.Application app,
            [FromServices]IApplicationService service,
            [FromServices]IUserService userService)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId) || string.IsNullOrWhiteSpace(app?.Name))
            {
                return BadRequest(new { reason = $"Invalid parameters" });
            }

            try
            {
                var hasPermission = await userService.IsOwner(organisationId, teamId, User.Identity.Name);
                if (!hasPermission)
                {
                    return Forbid();
                }

                app.CreatedAt = DateTime.Now;
                app.Id = null;
                var appResult = await service.AddAsync(organisationId, teamId, app);

                return CreatedAtRoute(nameof(GetApplicationAsync), new { organisationId, teamId, appId = app.Name }, appResult);
            }
            catch(NameAlreadyUsedException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch(Exception)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{organisationId}/teams/{teamId}/apps/{appId}"), Authorize(Roles = "user,developer,admin,team-leader")]
        public async Task<IActionResult> UpdateApplicationAsync(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromRoute] string appId,
            [FromBody]DataContracts.Application application,
            [FromServices]IApplicationService service,
            [FromServices]IUserService userService)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId) || string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(application?.Name))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            if (!string.Equals(application?.Id, appId))
            {
                return BadRequest(new { reason = $"Invalid application name!", request = application, appId });
            }

            try
            {
                var hasPermission = await userService.IsOwner(organisationId, teamId, User.Identity.Name);
                if (!hasPermission)
                {
                    return Forbid();
                }

                var appResult = await service.UpdateAsync(organisationId, teamId, application);
                if (appResult == null)
                {
                    return NotFound();
                }

                return Ok(appResult);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message } );
            }
            catch(Exception)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{organisationId}/teams/{teamId}/apps/{appId}"), Authorize(Roles = "user,developer,admin,team-leader")]
        public async Task<IActionResult> DeleteApplicationAsync(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromRoute] string appId,
            [FromServices] IApplicationService service,
            [FromServices] IUserService userService)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId) || string.IsNullOrWhiteSpace(appId))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            try
            {
                var hasPermission = await userService.IsOwner(organisationId, teamId, User.Identity.Name);
                if (!hasPermission)
                {
                    return Forbid();
                }

                await service.DeleteByIdAsync(organisationId, teamId, appId);

                return NoContent();
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{orgId}/teams/{teamId}/apps/{appId}/webhook/{commit}"), AllowAnonymous]
        public async Task<IActionResult> GetLastActionResultAsync(
            [FromRoute] string orgId,
            [FromRoute] string teamId,
            [FromRoute] string appId,
            [FromRoute] string commit,
            [FromServices]IGithubEventResultService service)
        {
            var result = await service.GetLastEventAsync(commit, teamId, appId, orgId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("{organisationId}/teams/{teamId}/apps/{appId}/webhook"), AllowAnonymous]
        public async Task<IActionResult> PostWebHookIntegrationAsync(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromRoute] string appId,
            [FromBody]object payload, 
            [FromServices]IApplicationService service, 
            [FromServices]IGitHubWebHookService githubService)
        {
            var headerAuth = Request.Headers["X-Hub-Signature"];
            var headerEvent = Request.Headers["X-GitHub-Event"];

            if(!string.IsNullOrWhiteSpace(headerEvent) && !string.IsNullOrWhiteSpace(headerAuth) && await service.IsValidSha1Async(organisationId, teamId, appId, headerAuth, payload.ToString()))
            {
                if (headerEvent.Equals("push") || headerEvent.Equals("pull_request"))
                {

                    IGithubEvent @event = null;
                    if (headerEvent.Equals("push"))
                    {
                        @event = JsonSerializer.Deserialize<PushEvent>(payload.ToString(), optionSerialization);
                    }
                    else
                    {
                        @event = JsonSerializer.Deserialize<PullRequestEvent>(payload.ToString(), optionSerialization);
                    }

                    @event.ApplicationId = appId;
                    @event.OrganisationId = organisationId;
                    @event.TeamId = teamId;
                    await githubService.EnqueueAsync(@event);
                }
            }
            return Ok();
        }
    }
}