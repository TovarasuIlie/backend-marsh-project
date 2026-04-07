using backend_marsh_project.Data;
using backend_marsh_project.DTOs;
using backend_marsh_project.Entities;
using backend_marsh_project.Exceptions;
using backend_marsh_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend_marsh_project.Controllers
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

        [HttpGet("user-overview")]
        [Authorize]
        public async Task<ActionResult<OverviewUser>> UserOverview()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var user = await _authService.GetUserData(int.Parse(userIdString));

                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
