using backend_marsh_project.Data;
using backend_marsh_project.DTOs;
using backend_marsh_project.Entities;
using backend_marsh_project.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace backend_marsh_project.Services
{
    public class DeviceService
    {
        private readonly AppDbContext _context;

        public DeviceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Device>> GetAllDevices()
        {
            return await _context.Devices.ToListAsync();
        }

        public async Task<Device> GetDeviceById(int id)
        {
            Device? device = await _context.Devices.FindAsync(id);

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

        public async Task<Device> UpdateDevice(Device updatedDevice)
        {
            Device? device = await _context.Devices.FindAsync(updatedDevice.Id);

            if (device == null)
            {
                throw new NotFoundException("The device no longer exists in the system.");
            }

            device.OSVersion    = updatedDevice.OSVersion;
            device.Processor    = updatedDevice.Processor;
            device.RAMAmount    = updatedDevice.RAMAmount;
            device.Description  = updatedDevice.Description;

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
    }
}
