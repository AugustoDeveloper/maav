using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAAV.Application;
using MAAV.DataContracts;
using MAAV.Application.Exceptions;
using Microsoft.AspNetCore.Cors;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1"), Authorize]
    public class OrganisationController : ControllerBase
    {
        [HttpGet("{organisationId}", Name = nameof(GetOrganisationAsync)), Authorize(Roles = "user,admin,team-leader,developer")]
        public async Task<IActionResult> GetOrganisationAsync(
            [FromRoute] string organisationId, 
            [FromServices] IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId))
            {
                return BadRequest(new { reason = $"Invalid organisation id {organisationId}" });
            }

            try
            {
                var organisation = await service.GetByOrganisationNameAsync(organisationId);
                if (organisation == null)
                {
                    return NotFound();
                }

                return Ok(organisation);
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("{organisationId}"), AllowAnonymous]
        public async Task<IActionResult> RegisterOrganisationAsync(
            [FromRoute] string organisationId,
            [FromBody] OrganisationRegistration organisation,
            [FromServices]IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || organisation == null)
            {
                return BadRequest(new { reason = $"Invalid request!", request = organisation, organisationId });
            }

            if (!organisationId.Equals(organisation.Id))
            {
                return BadRequest(new { reason = $"Invalid organisation name!", request = organisation, organisationId });
            }

            try
            {
                organisation.CreatedAt = DateTime.Now;
                var organisationResult = await service.RegisterAsync(organisation);

                return CreatedAtRoute(nameof(GetOrganisationAsync), new { organisationId }, organisationResult);
            }
            catch(NameAlreadyUsedException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{organisationId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateOrganisationAsync(
            [FromRoute] string organisationId, 
            [FromBody]Organisation orgRequest,
            [FromServices]IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || orgRequest == null)
            {
                return BadRequest(new { reason = $"Invalid request!", request = orgRequest, organisationId });
            }

            if (!organisationId.Equals(orgRequest.Name))
            {
                return BadRequest(new { reason = $"Invalid organisation name!", request = orgRequest, organisationId });
            }

            try
            {
                var organisation = await service.UpdateAsync(orgRequest);
                if (organisation == null)
                {
                    return NotFound();
                }

                return Ok(organisation);
            }
            catch(Exception)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{organisationId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteOrganisationAsync(
            [FromRoute]string organisationId,
            [FromServices] IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId))
            {
                return BadRequest(new { reason = $"Invalid organisation id {organisationId}" });
            }

            try
            {
                await service.DeleteByOrganisationNameAsync(organisationId);

                return NoContent();
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
