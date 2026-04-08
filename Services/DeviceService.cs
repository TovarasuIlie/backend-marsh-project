using backend_marsh_project.Data;
using backend_marsh_project.DTOs;
using backend_marsh_project.Entities;
using backend_marsh_project.Entities.Extensions;
using backend_marsh_project.Entities.Paging;
using backend_marsh_project.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace backend_marsh_project.Services
{
    public class DeviceService
    {
        private readonly AppDbContext _context;
        private readonly LLMService _LLMService;

        public DeviceService(AppDbContext context, LLMService lLMService)
        {
            _context = context;
            _LLMService = lLMService;
        }

        public async Task<PagedResult<Device>> GetAllDevices(PaginationParameters paginationParameters)
        {
            return await _context.Devices
                .Include(d => d.AssignedToUser)
                .Select(d => new Device
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manufacturer = d.Manufacturer,
                    Processor = d.Processor,
                    OperatingSystem = d.OperatingSystem,
                    OSVersion = d.OSVersion,
                    RAMAmount = d.RAMAmount,
                    Description = d.Description,
                    AssignedToUser = d.AssignedToUser == null ? null : new User
                    {
                        Id = d.AssignedToUser.Id,
                        Name = d.AssignedToUser.Name,
                        Email = d.AssignedToUser.Email,
                        Role = d.AssignedToUser.Role,
                        Location = d.AssignedToUser.Location
                    }
                })
                .ToPagedResultAsync(paginationParameters);
        }

        public async Task<Device> GetDeviceById(int id)
        {
            Device? device = await _context.Devices
                .Include(d => d.AssignedToUser)
                .Select(d => new Device
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manufacturer = d.Manufacturer,
                    Processor = d.Processor,
                    OperatingSystem = d.OperatingSystem,
                    OSVersion = d.OSVersion,
                    RAMAmount = d.RAMAmount,
                    Description = d.Description,
                    AssignedToUser = d.AssignedToUser == null ? null : new User
                    {
                        Id = d.AssignedToUser.Id,
                        Name = d.AssignedToUser.Name,
                        Email = d.AssignedToUser.Email,
                        Role = d.AssignedToUser.Role,
                        Location = d.AssignedToUser.Location
                    }
                })
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null) 
            {
                throw new NotFoundException($"The device with ID {id} was not found.");
            }

            return device;
        }

        public async Task<Device> AddNewDevice(NewDevice newDevice)
        {
            var duplicateDevice = await _context.Devices.AnyAsync(d => d.Name == newDevice.Name && d.Manufacturer == newDevice.Manufacturer && d.Type == newDevice.Type && d.RAMAmount == newDevice.RAMAmount);

            if (duplicateDevice)
            {
                throw new BadRequestException("This device already exists.");
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

            _context.Devices.Add(device);

            try
            {
                await _context.SaveChangesAsync();
                return device;
            }
            catch (DbUpdateException)
            {
                throw new BadRequestException("Invalid type. Must be Laptop, Phone, or Tablet.");
            }
        }

        public async Task<Device> UpdateDevice(int deviceId, EditDevice updatedDevice)
        {
            Device? device = await _context.Devices.FindAsync(deviceId);

            if (device == null)
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            device.OperatingSystem = updatedDevice.OperatingSystem;
            device.OSVersion       = updatedDevice.OSVersion;
            device.Processor       = updatedDevice.Processor;
            device.RAMAmount       = updatedDevice.RAMAmount;
            device.Description     = updatedDevice.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Another user updated this device while you were editing.");
            }

            return device;
        }

        public async Task DeleteDevice(int id)
        {
            Device? device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }

        public async Task<Object> GetGeneratedMessage(int id)
        {
            Device? device = await _context.Devices.FindAsync(id);

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
    }
}
