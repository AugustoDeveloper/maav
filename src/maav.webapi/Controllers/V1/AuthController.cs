using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MAAV.Application;
using System;
using MAAV.DataContracts;
using MAAV.Application.Exceptions;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using MAAV.WebAPI.Extensions;

namespace MAAV.WebAPI.Controllers
{
    [ApiController, Route("api/v1")]
    public class AuthController : ControllerBase
    {
        [HttpPost("{organisationId}/authenticate"), AllowAnonymous]
        public async Task<IActionResult> AuthenticateAsync(
            [FromRoute] string organisationId,
            [FromBody] Login login,
            [FromServices]IUserService service)
        {
            if (string.IsNullOrWhiteSpace(login?.Username) || string.IsNullOrWhiteSpace(login?.Password) || string.IsNullOrWhiteSpace(organisationId))
            {
                return BadRequest();
            }

            try
            {
                var auth = await service.AuthenticateAsync(organisationId, login.Username, login.Password);
                auth.GenerateToken();

                return Ok(auth);
            }
            catch(UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}