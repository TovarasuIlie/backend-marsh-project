using BackendMarshProject.DTOs.Form;
using BackendMarshProject.DTOs.User;
using BackendMarshProject.Entities.Paging;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackendMarshProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<UserDTO>>> GetUsers([FromQuery] PaginationParameters paginationParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList() });
            }

            var devices = await _userService.GetAllUsers(paginationParameters);

            return Ok(devices);
        }
        [HttpGet("get-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUser(id);

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

        [HttpPatch("edit-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> EditUser(int id, [FromBody] EditUser editUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList() });
            }

            var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(adminIdString, out int adminId))
            {
                return StatusCode(403, new { message = "Invalid user token." });
            }

            try
            {
                var result = await _userService.UpdateUser(id, editUser, adminId);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(adminIdString, out int adminId))
            {
                return StatusCode(403, new { message = "Invalid user token." });
            }

            try
            {
                await _userService.DeleteUser(id, adminId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
        public async Task<ActionResult<UserDTO>> UserOverview()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token." });
            }

            try
            {
                var user = await _userService.GetUser(int.Parse(userIdString));

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
