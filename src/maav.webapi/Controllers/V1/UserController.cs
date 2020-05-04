using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MAAV.Application;
using System;
using MAAV.DataContracts;
using MAAV.Application.Exceptions;
using System.Net;
using System.Linq;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1"), Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet("{organisationId}/users/{username}", Name = nameof(GetUserAsync)), Authorize(Roles = "user,admin,team-leader")]
        public async Task<IActionResult> GetUserAsync(
            [FromRoute] string organisationId, 
            [FromRoute]string username, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var user = await service.GetByUsernameAsync(organisationId, username);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
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

        [HttpPut("{organisationId}/team/{teamId}/users/{username}"), Authorize(Roles = "admin,team-leader, developer, user")]
        public async Task<IActionResult> SetTeamPermission(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromRoute] string username,
            [FromBody] TeamPermission permission,
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) ||
                string.IsNullOrWhiteSpace(username) ||
                permission == null ||
                (!permission.IsWriter && !permission.IsReader && !permission.IsOwner))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters" });
            }

            try
            {
                var hasPermission = await service.IsOwner(organisationId, teamId, User.Identity.Name);
                if (!hasPermission)
                {
                    return Forbid();
                }

                await service.SetRolesAsync(organisationId, teamId, username, permission);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message });
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{organisationId}/team/{teamId}/users"), Authorize(Roles = "admin,team-leader, developer, user")]
        public async Task<IActionResult> SetTeamPermission(
            [FromRoute] string organisationId,
            [FromRoute] string teamId,
            [FromBody] User[] users,
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) ||
                string.IsNullOrWhiteSpace(teamId) ||
                users.Any(u => string.IsNullOrWhiteSpace(u.Username)) ||
                users.Any(u => u.TeamsPermissions == null) ||
                !users.Any(u => u.TeamsPermissions.Any(t => t.TeamId == teamId)) ||
                (users.Any(u => u.TeamsPermissions.Where(t => t.TeamId == teamId).Any(permission => !permission.IsWriter && !permission.IsReader && !permission.IsOwner))))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters" });
            }

            try
            {
                var hasPermission = await service.IsOwner(organisationId, teamId, User.Identity.Name);
                if (!hasPermission)
                {
                    return Forbid();
                }

                var permissions = users.GroupBy(k => k.Username, u => u.TeamsPermissions.FirstOrDefault(t => t.TeamId == teamId)).ToList();
                foreach (var permissionGroup in permissions)
                {
                    await service.SetRolesAsync(organisationId, teamId, permissionGroup.Key, permissionGroup.FirstOrDefault());
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message });
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{organisationId}/users"), Authorize(Roles = "admin")]
        public async Task<IActionResult> LoadAllUsers(
            [FromRoute] string organisationId,
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var users = await service.LoadAllUsers(organisationId);

                return Ok(users);
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

        [HttpDelete("{organisationId}/teams/{teamId}/users/{username}"), Authorize(Roles = "admin,team-leader, developer, user")]
        public async Task<IActionResult> RemoveUserFromTeamAsync(
            [FromRoute] string organisationId, 
            [FromRoute] string teamId, 
            [FromRoute] string username, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(teamId))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var hasPermission = await service.IsOwner(organisationId, teamId, User.Identity.Name);
                if (!hasPermission)
                {
                    return Forbid();
                }

                await service.RemoveUserToTeamAsync(organisationId, teamId, username);

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

        [HttpPost("{organisationId}/users"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUserAsync(
            [FromRoute] string organisationId, 
            [FromBody] User user, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(user?.Username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                user.CreatedAt = DateTime.Now;
                user = await service.AddAsync(organisationId, user);

                return CreatedAtRoute(nameof(GetUserAsync), new { username = user.Username, organisationId }, user);
            }
            catch(NameAlreadyUsedException)
            {
                return BadRequest(new { reason = "Username is already used for another user" });
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

        [HttpPut("{organisationId}/users/{username}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUserAsync(
            [FromRoute] string organisationId, 
            [FromRoute] string username, 
            [FromBody] User user, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(user?.Username) || string.IsNullOrWhiteSpace(username) || !string.Equals(username, user?.Username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                
                user = await service.UpdateAsync(organisationId, user, user.Username.Equals(User.Identity.Name));

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
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

        [HttpDelete("{organisationId}/users/{username}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUserAsync(
            [FromRoute] string organisationId, 
            [FromRoute] string username, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) ||  string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                await service.DeleteAsync(organisationId, username);

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

        [HttpPatch("{organisationId}/users/{username}"), Authorize(Roles = "admin,team-leader")]
        public async Task<IActionResult> ResetPasswordAsync(
            [FromRoute] string organisationId,
            [FromRoute] string username,
            [FromBody] User user,
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationId) || string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters" });
            }

            try
            {
                await service.ResetPassword(organisationId, username, user.Password);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { reason = ex.Message });
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
