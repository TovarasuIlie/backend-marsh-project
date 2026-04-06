using backend_marsh_project.Data;
using backend_marsh_project.DTOs;
using backend_marsh_project.Entities;
using backend_marsh_project.Exceptions;
using backend_marsh_project.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<string>> RegisterUser(RegisterUser registerUser)
        {
            try
            {
                var token = await _authService.RegisterUser(registerUser);

                return Ok(token);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login-user")]
        public async Task<ActionResult<string>> LoginUser(LoginUser loginUser)
        {
            try
            {
                var token = await _authService.LoginUser(loginUser);

                return Ok(token);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
