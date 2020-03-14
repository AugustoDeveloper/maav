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
        [HttpGet("{organisationName}/users/{username}", Name = nameof(GetUserAsync)), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> GetUserAsync(
            [FromRoute] string organisationName, 
            [FromRoute]string username, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var user = await service.GetByUsernameAsync(organisationName, username);
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

        [HttpPut("{organisationName}/users/{username}/roles"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> SetRolesAsync(
            [FromRoute] string organisationName,
            [FromRoute] string username,
            [FromBody] string[] roles,
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || 
                string.IsNullOrWhiteSpace(username) || 
                roles.Length < 1 || 
                roles.Any(r => string.IsNullOrWhiteSpace(r)))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                await service.SetRolesAsync(organisationName, username, roles);

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

        [HttpGet("{organisationName}/users/{username}/roles"), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> LoadRolesAsync(
            [FromRoute] string organisationName,
            [FromRoute] string username,
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var roles = await service.LoadRolesAsync(organisationName, username);

                return Ok(roles);
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

        [HttpGet("{organisationName}/users"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> LoadAllUsers(
            [FromRoute] string organisationName,
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                var users = await service.LoadAllUsers(organisationName);

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

        //TODO: Create unit test for this route
        [HttpPut("{organisationName}/teams/{teamName}/users"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> AddUserOnTeamAsync(
            [FromRoute] string organisationName, 
            [FromRoute] string teamName, 
            [FromBody] User user, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(user?.Username) || string.IsNullOrWhiteSpace(teamName))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                await service.AddUserToTeamAsync(organisationName, teamName, user);

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

        [HttpDelete("{organisationName}/teams/{teamName}/users/{username}"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> RemoveUserFromTeamAsync(
            [FromRoute] string organisationName, 
            [FromRoute] string teamName, 
            [FromRoute] string username, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(teamName))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {   
                await service.RemoveUserToTeamAsync(organisationName, teamName, username);

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

        [HttpPost("{organisationName}/users"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> AddUserAsync(
            [FromRoute] string organisationName, 
            [FromBody] User user, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(user?.Username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                user = await service.AddAsync(organisationName, user);

                return CreatedAtRoute(nameof(GetUserAsync), new { username = user.Username, organisationName }, user);
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

        [HttpPut("{organisationName}/users/{username}"), Authorize(Roles = "User,Administrator,Leader")]
        public async Task<IActionResult> UpdateUserAsync(
            [FromRoute] string organisationName, 
            [FromRoute] string username, 
            [FromBody] User user, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(user?.Username) || string.IsNullOrWhiteSpace(username) || !string.Equals(username, user?.Username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                
                user = await service.UpdateAsync(organisationName, user);

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

        [HttpDelete("{organisationName}/users/{username}"), Authorize(Roles = "Administrator,Leader")]
        public async Task<IActionResult> DeleteUserAsync(
            [FromRoute] string organisationName, 
            [FromRoute] string username, 
            [FromServices] IUserService service)
        {
            if (string.IsNullOrWhiteSpace(organisationName) ||  string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { reason = "Something is wrong with your parameters"});
            }

            try
            {
                await service.DeleteAsync(organisationName, username);

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
