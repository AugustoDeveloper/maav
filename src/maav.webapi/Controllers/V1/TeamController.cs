using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MAAV.Application;
using System;
using MAAV.DataContracts;
using MAAV.Application.Exceptions;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1"), Authorize]
    public class TeamController : ControllerBase
    {
        [HttpGet("{organisationName}/teams/{teamName}", Name = nameof(GetTeamAsync)), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> GetTeamAsync(
            [FromRoute] string organisationName, 
            [FromRoute]string teamName, 
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(teamName))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var team = await service.GetByTeamNameAsync(organisationName, teamName);
                if (team == null)
                {
                    return NotFound();
                }

                return Ok(team);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //TODO: Create unit test for this route
        [HttpGet("{organisationName}/teams"), Authorize(Roles = "Administrator")]
        public async Task<IActionResult> LoadTeamsAsync(
            [FromRoute] string organisationName, 
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var teams = await service.LoadByOrganisationNameAsync(organisationName);

                return Ok(teams);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("{organisationName}/teams"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> AddTeamAsync(
            [FromRoute] string organisationName, 
            [FromBody] Team team, 
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(team?.Name))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                team = await service.AddAsync(organisationName, team);

                return CreatedAtRoute(nameof(GetTeamAsync), new { teamName = team.Name, organisationName }, team);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch(NameAlreadyUsedException)
            {
                return BadRequest(new { reason = "The team name is already used!"});
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{organisationName}/teams/{teamName}"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> UpdateTeamAsync(
            [FromRoute] string organisationName, 
            [FromRoute] string teamName, 
            [FromBody] Team team, 
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(team?.Name) || string.IsNullOrWhiteSpace(teamName) || !string.Equals(teamName, team?.Name))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                
                team = await service.UpdateAsync(organisationName, team);

                if (team == null)
                {
                    return NotFound();
                }

                return Ok(team);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{organisationName}/teams/{teamName}"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> DeleteTeamAsync(
            [FromRoute] string organisationName, 
            [FromRoute] string teamName, 
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) ||  string.IsNullOrWhiteSpace(teamName))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                await service.DeleteAsync(organisationName, teamName);

                return NoContent();
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}