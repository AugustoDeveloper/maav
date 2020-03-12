using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAAV.Application;
using MAAV.DataContracts;
using MAAV.Application.Exceptions;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1"), Authorize]
    public class OrganisationController : ControllerBase
    {
        [HttpGet("{organisationName}", Name = nameof(GetOrganisationAsync)), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> GetOrganisationAsync(
            [FromRoute] string organisationName, 
            [FromServices] IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest(new { reason = $"Invalid organisation name {organisationName}" });
            }

            try
            {
                var organisation = await service.GetByOrganisationNameAsync(organisationName);
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

        [HttpPost("{organisationName}"), AllowAnonymous]
        public async Task<IActionResult> RegisterOrganisationAsync(
            [FromRoute] string organisationName,
            [FromBody] Organisation organisation,
            [FromServices]IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || organisation == null)
            {
                return BadRequest(new { reason = $"Invalid request!", request = organisation, organisationName });
            }

            if (!organisationName.Equals(organisation.Name))
            {
                return BadRequest(new { reason = $"Invalid organisation name!", request = organisation, organisationName });
            }

            try
            {
                var organisationResult = await service.RegisterAsync(organisation);

                return CreatedAtRoute(nameof(GetOrganisationAsync), new { organisationName }, organisationResult);
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

        [HttpPut("{organisationName}"), Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateOrganisationAsync(
            [FromRoute] string organisationName, 
            [FromBody]Organisation orgRequest,
            [FromServices]IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || orgRequest == null)
            {
                return BadRequest(new { reason = $"Invalid request!", request = orgRequest, organisationName });
            }

            if (!organisationName.Equals(orgRequest.Name))
            {
                return BadRequest(new { reason = $"Invalid organisation name!", request = orgRequest, organisationName });
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

        [HttpDelete("{organisationName}"), Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteOrganisationAsync(
            [FromRoute]string organisationName,
            [FromServices] IOrganisationService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest(new { reason = $"Invalid organisation name {organisationName}" });
            }

            try
            {
                await service.DeleteByOrganisationNameAsync(organisationName);

                return NoContent();
            }
            catch
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
