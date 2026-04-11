using BackendMarshProject.DTOs;
using BackendMarshProject.DTOs.Form;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendMarshProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-user")]
        public async Task<ActionResult<LoggedUser>> RegisterUser([FromBody] RegisterUser registerUser)
        {
            try
            {
                var logged = await _authService.RegisterUser(registerUser);

                return Ok(logged);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPost("login-user")]
        public async Task<ActionResult<LoggedUser>> LoginUser([FromBody] LoginUser loginUser)
        {
            try
            {
                var logged = await _authService.LoginUser(loginUser);

                return Ok(logged);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
