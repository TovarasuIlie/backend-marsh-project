using BackendMarshProject.DTOs.Form;
using BackendMarshProject.Entities;
using BackendMarshProject.Entities.Paging;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendMarshProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        public DeviceController(DeviceService deviceService) 
        { 
            _deviceService = deviceService;
        }

        [HttpGet("get-devices")]
        [Authorize]
        public async Task<ActionResult<PagedResult<Device>>> GetDevices([FromQuery] PaginationParameters paginationParameters)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList() });
            }

            var devices = await _deviceService.GetAllDevices(paginationParameters);

            return Ok(devices);
        }

        [HttpGet("get-device/{id}")]
        [Authorize]
        public async Task<ActionResult<Device>> GetDevice(int id)
        {
            try
            {
                var device = await _deviceService.GetDeviceById(id);

                return Ok(device);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception) 
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPatch("edit-device/{id}")]
        [Authorize(Roles = "SuperAdmin, InventoryManager")]
        public async Task<ActionResult<Device>> EditDevice(int id, [FromBody] EditDevice editDevice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList() });
            }

            try
            {
                var result = await _deviceService.UpdateDevice(id, editDevice);

                return Ok(result);
            }
            catch (NotFoundException ex) 
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
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

        [HttpPost("add-device")]
        [Authorize(Roles = "SuperAdmin, InventoryManager")]
        public async Task<ActionResult<Device>> AddDevice([FromBody] NewDevice newDevice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList() });
            }

            try
            {
                var device = await _deviceService.AddNewDevice(newDevice);

                return Ok(device);
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

        [HttpDelete("delete-device/{id}")]
        [Authorize(Roles = "SuperAdmin, InventoryManager")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                await _deviceService.DeleteDevice(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("get-devices-overview")]
        [Authorize]
        public async Task<ActionResult<PagedResult<Device>>> GetDevicesOverview([FromQuery] PaginationParameters paginationParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList() });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token." });
            }

            try
            {
                var devices = await _deviceService.GetMyAndUnassignedDevices(paginationParameters, userId);

                return Ok(devices);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("generate-response/{id}")]
        [Authorize]
        public async Task<ActionResult<Object>> GetLLMResponse(int id)
        {
            try
            {
                var device = await _deviceService.GetGeneratedMessage(id);

                return Ok(device);
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

        [HttpPatch("toggle-device-assign-status/{id}")]
        [Authorize]
        public async Task<ActionResult<Object>> ToggleDeviceAssignStatus(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token." });
            }

            try
            {
                await _deviceService.ToggleAssignStatus(id, userId);

                return Ok(new { message = "Device assign status has been change successfully." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
