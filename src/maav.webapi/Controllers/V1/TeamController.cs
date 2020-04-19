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
        [HttpGet("{organisationId}/teams/{teamId}", Name = nameof(GetTeamAsync)), Authorize(Roles = "user,admin,team-leader,developer")]
        public async Task<IActionResult> GetTeamAsync(
            [FromRoute] string organisationId,
            [FromRoute]string teamId,
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(teamId))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters" });
            }

            try
            {
                var team = await service.GetByTeamNameAsync(organisationId, teamId);
                if (team == null)
                {
                    return NotFound();
                }

                return Ok(team);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //TODO: Create unit test for this route
        [HttpGet("{organisationId}/teams"), Authorize(Roles = "admin")]
        public async Task<IActionResult> LoadTeamsAsync(
            [FromRoute] string organisationId, 
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var teams = await service.LoadByOrganisationIdAsync(organisationId);

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

        [HttpPost("{organisationId}/teams"), Authorize(Roles = "admin,team-leader")]
        public async Task<IActionResult> AddTeamAsync(
            [FromRoute] string organisationId, 
            [FromBody] Team team, 
            [FromServices] ITeamService service,
            [FromServices] IUserService userService)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(team?.Name))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                team.CreatedAt = DateTime.Now;
                team = await service.AddAsync(organisationId, team);
                await userService.SetRolesAsync(organisationId, team.Id, this.User.Identity.Name, new TeamPermission { TeamId = team.Id, IsOwner = true });

                return CreatedAtRoute(nameof(GetTeamAsync), new { teamId = team.Name, organisationId }, team);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message});
            }
            catch(NameAlreadyUsedException)
            {
                return BadRequest(new { reason = "The team id is already used!"});
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{organisationId}/teams/{teamId}"), Authorize(Roles = "user,admin,team-leader,developer")]
        public async Task<IActionResult> UpdateTeamAsync(
            [FromRoute] string organisationId, 
            [FromRoute] string teamId, 
            [FromBody] Team team, 
            [FromServices] IUserService userService,
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(team?.Id) || string.IsNullOrWhiteSpace(teamId) || !string.Equals(teamId, team?.Id))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var hasPermission = await userService.IsOwner(organisationId, teamId, User.Identity.Name);
                if (!hasPermission)
                {
                    return Forbid();
                }

                team = await service.UpdateAsync(organisationId, team);

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

        [HttpDelete("{organisationId}/teams/{teamId}"), Authorize(Roles = "admin,team-leader")]
        public async Task<IActionResult> DeleteTeamAsync(
            [FromRoute] string organisationId, 
            [FromRoute] string teamId, 
            [FromServices] IUserService userService,
            [FromServices] ITeamService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) ||  string.IsNullOrWhiteSpace(teamId))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                await service.DeleteAsync(organisationId, teamId);

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