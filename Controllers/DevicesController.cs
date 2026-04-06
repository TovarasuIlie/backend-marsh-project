using Microsoft.AspNetCore.Mvc;
using backend_marsh_project.Entities;
using backend_marsh_project.Services;
using backend_marsh_project.Exceptions;
using backend_marsh_project.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace backend_marsh_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        public DevicesController(DeviceService deviceService) 
        { 
            _deviceService = deviceService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            var devices = await _deviceService.GetAllDevices();

            return Ok(devices);
        }

        [HttpGet("{id}")]
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutDevice(Device device)
        {
            try
            {
                var result = await _deviceService.UpdateDevice(device);

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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Device>> PostDevice(NewDevice newDevice)
        {
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

        [HttpDelete("{id}")]
        [Authorize]
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
    }
}
