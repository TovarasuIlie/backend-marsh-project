using BackendMarshProject.Data;
using BackendMarshProject.DTOs.Device;
using BackendMarshProject.DTOs.Form;
using BackendMarshProject.Entities;
using BackendMarshProject.Entities.Extensions;
using BackendMarshProject.Entities.Paging;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Repository;
using Microsoft.EntityFrameworkCore;

namespace BackendMarshProject.Services
{
    public class DeviceService
    {
        private readonly IDeviceReposity _deviceReposity;
        private readonly LLMService _LLMService;

        public DeviceService(IDeviceReposity deviceReposity, LLMService lLMService)
        {
            _deviceReposity = deviceReposity;
            _LLMService = lLMService;
        }

        public async Task<PagedResult<DeviceDTO>> GetAllDevices(PaginationParameters paginationParameters)
        {
            return await _deviceReposity.GetAllDevices().ToPagedResultAsync(paginationParameters);
        }

        public async Task<PagedResult<DeviceDTO>> GetMyAndUnassignedDevices(PaginationParameters paginationParameters, int userId)
        {
            return await _deviceReposity.GetAllUnassignedDevices(userId)
                .OrderByDescending(d => d.AssignedToUser.Id)
                .ThenBy(d => d.Id)
                .ToPagedResultAsync(paginationParameters);
        }

        public async Task<DeviceDTO?> GetDeviceById(int id)
        {
            DeviceDTO? device = await _deviceReposity.GetDeviceById(id).FirstOrDefaultAsync();

            if (device == null) 
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            return device;
        }

        public async Task<Device> AddNewDevice(NewDevice newDevice)
        {
            
            if (await _deviceReposity.IsDuplicate(newDevice))
            {
                throw new BadRequestException("This device already registred in the system.");
            }

            Device device = new Device
            {
                Name = newDevice.Name,
                Manufacturer = newDevice.Manufacturer,
                Type = newDevice.Type,
                OperatingSystem = newDevice.OperatingSystem,
                OSVersion = newDevice.OSVersion,
                Processor = newDevice.Processor,
                RAMAmount = newDevice.RAMAmount,
                Description = newDevice.Description
            };

            try
            {
                await _deviceReposity.CreateDeviceAsync(device);

                return device;
            }
            catch (DbUpdateException)
            {
                throw new BadRequestException("Invalid type. Must be Laptop, Phone, or Tablet.");
            }
        }

        public async Task<Device> UpdateDevice(int deviceId, EditDevice updatedDevice)
        {
            Device? device = await _deviceReposity.GetDeviceByIdAsync(deviceId);

            if (device == null)
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            device.OperatingSystem = updatedDevice.OperatingSystem;
            device.OSVersion       = updatedDevice.OSVersion;
            device.Processor       = updatedDevice.Processor;
            device.RAMAmount       = updatedDevice.RAMAmount;
            device.Description     = updatedDevice.Description;

            if (await _deviceReposity.IsDuplicate(device))
            {
                throw new BadRequestException("This device already registred in the system.");
            }

            try
            {
                await _deviceReposity.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Another user updated this device while you were editing.");
            }

            return device;
        }

        public async Task DeleteDevice(int id)
        {
            Device? device = await _deviceReposity.GetDeviceByIdAsync(id);

            if (device == null)
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            await _deviceReposity.DeleteDeviceAsync(device);
        }

        public async Task<Object> GetGeneratedMessage(int id)
        {
            Device? device = await _deviceReposity.GetDeviceByIdAsync(id);

            if (device == null)
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            string inputMessage = $"Input: Name – {device.Name}, " +
                $"Manufacturer – {device.Manufacturer}, " +
                $"OS – {device.OperatingSystem} {device.OSVersion}, " +
                $"Type – {device.Type.ToString()}, " +
                $"RAM – {device.RAMAmount}GB, " +
                $"Processor – {device.Processor}";

            return new { message = await _LLMService.GeneratedResponse(inputMessage) };
        }

        public async Task ToggleAssignStatus(int deviceId, int userId)
        {
            Device? device = await _deviceReposity.GetDeviceByIdAsync(deviceId);

            if (device == null)
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            if (device.AssignedToUserId == null)
            {
                device.AssignedToUserId = userId;
            }
            else if (device.AssignedToUserId == userId)
            {
                device.AssignedToUserId = null;
            }
            else
            {
                throw new BadRequestException("You can't unassign this device.");
            }

            try
            {
                await _deviceReposity.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Another user updated this device while you were editing.");
            }
        }
    }
}
