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
    public class ApplicationController : ControllerBase
    {
        [HttpGet("{organisationName}/teams/{teamName}/apps/{appName}", Name = nameof(GetApplicationAsync)), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> GetApplicationAsync(
            [FromRoute] string organisationName, 
            [FromRoute] string teamName, 
            [FromRoute] string appName, 
            [FromServices] IApplicationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(teamName) || string.IsNullOrWhiteSpace(appName))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            try
            {
                var app = await service.GetByNameAsync(organisationName, teamName, appName);
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

        [HttpGet("{organisationName}/teams/{teamName}/apps"), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> LoadAllApplicationFromTeam(
            [FromRoute] string organisationName, 
            [FromRoute] string teamName, 
            [FromServices] IApplicationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(teamName))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            try
            {
                var apps = await service.LoadAllFromTeamAsync(organisationName, teamName);

                return Ok(apps);
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("{organisationName}/teams/{teamName}/apps"), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> AddApplicationAsync(
            [FromRoute] string organisationName,
            [FromRoute] string teamName,
            [FromBody] DataContracts.Application app,
            [FromServices]IApplicationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(teamName) || string.IsNullOrWhiteSpace(app?.Name))
            {
                return BadRequest(new { reason = $"Invalid parameters" });
            }

            try
            {
                var appResult = await service.AddAsync(organisationName, teamName, app);

                return CreatedAtRoute(nameof(GetApplicationAsync), new { organisationName, teamName, appName = app.Name }, appResult);
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

        [HttpPut("{organisationName}/teams/{teamName}/apps/{appName}"), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> UpdateApplicationAsync(
            [FromRoute] string organisationName,
            [FromRoute] string teamName,
            [FromRoute] string appName,
            [FromBody]DataContracts.Application application,
            [FromServices]IApplicationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(teamName) || string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(application?.Name))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            if (!string.Equals(application?.Name, appName))
            {
                return BadRequest(new { reason = $"Invalid application name!", request = application, appName });
            }

            try
            {
                var appResult = await service.UpdateAsync(organisationName, teamName, application);
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

        [HttpDelete("{organisationName}/teams/{teamName}/apps/{appName}"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> DeleteOrganisationAsync(
            [FromRoute] string organisationName,
            [FromRoute] string teamName,
            [FromRoute] string appName,
            [FromServices] IApplicationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(teamName) || string.IsNullOrWhiteSpace(appName))
            {
                return BadRequest(new { reason = $"Invalid parameters!" });
            }

            try
            {
                await service.DeleteByNameAsync(organisationName, teamName, appName);

                return NoContent();
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}