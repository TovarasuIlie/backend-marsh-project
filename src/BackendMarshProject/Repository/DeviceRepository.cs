using BackendMarshProject.Data;
using BackendMarshProject.DTOs.Device;
using BackendMarshProject.DTOs.Form;
using BackendMarshProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendMarshProject.Repository
{
    public interface IDeviceReposity
    {
        IQueryable<DeviceDTO> GetAllDevices();
        IQueryable<DeviceDTO> GetAllUnassignedDevices(int id);
        IQueryable<DeviceDTO> GetDeviceById(int id);
        Task<Device?> GetDeviceByIdAsync(int id);
        Task CreateDeviceAsync(Device device);
        Task<bool> IsDuplicate(Device device);
        Task<bool> IsDuplicate(NewDevice device);
        Task SaveChangesAsync();
        Task DeleteDeviceAsync(Device device);
    }

    public class DeviceRepository : IDeviceReposity
    {
        private readonly AppDbContext _context;

        public DeviceRepository(AppDbContext context) 
        { 
            _context = context;
        }

        public async Task CreateDeviceAsync(Device device)
        {
            _context.Devices.Add(device);
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeviceAsync(Device device)
        {
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }

        public IQueryable<DeviceDTO> GetAllDevices()
        {
            return _context.Devices
                .Include(d => d.AssignedToUser)
                .Select(d => new DeviceDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manufacturer = d.Manufacturer,
                    Processor = d.Processor,
                    OperatingSystem = d.OperatingSystem,
                    OSVersion = d.OSVersion,
                    RAMAmount = d.RAMAmount,
                    Description = d.Description,
                    AssignedToUser = d.AssignedToUser == null ? null : new UserDTO
                    {
                        Id = d.AssignedToUser.Id,
                        Name = d.AssignedToUser.Name,
                        Email = d.AssignedToUser.Email,
                        Role = d.AssignedToUser.Role,
                        Location = d.AssignedToUser.Location
                    }
                })
                .AsNoTracking();
        }

        public IQueryable<DeviceDTO> GetAllUnassignedDevices(int id)
        {
            return _context.Devices
                .Include(d => d.AssignedToUser)
                .Where(d => d.AssignedToUser == null || d.AssignedToUser.Id == id)
                .Select(d => new DeviceDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manufacturer = d.Manufacturer,
                    Processor = d.Processor,
                    OperatingSystem = d.OperatingSystem,
                    OSVersion = d.OSVersion,
                    RAMAmount = d.RAMAmount,
                    Description = d.Description,
                    AssignedToUser = d.AssignedToUser == null ? null : new UserDTO
                    {
                        Id = d.AssignedToUser.Id,
                        Name = d.AssignedToUser.Name,
                        Email = d.AssignedToUser.Email,
                        Role = d.AssignedToUser.Role,
                        Location = d.AssignedToUser.Location
                    }
                })
                .AsNoTracking();
        }

        public IQueryable<DeviceDTO> GetDeviceById(int id)
        {
            return _context.Devices
                .Include(d => d.AssignedToUser)
                .Where(d => d.Id == id)
                .Select(d => new DeviceDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manufacturer = d.Manufacturer,
                    Processor = d.Processor,
                    OperatingSystem = d.OperatingSystem,
                    OSVersion = d.OSVersion,
                    RAMAmount = d.RAMAmount,
                    Description = d.Description,
                    Type = d.Type,
                    AssignedToUser = d.AssignedToUser == null ? null : new UserDTO
                    {
                        Id = d.AssignedToUser.Id,
                        Name = d.AssignedToUser.Name,
                        Email = d.AssignedToUser.Email,
                        Role = d.AssignedToUser.Role,
                        Location = d.AssignedToUser.Location
                    }
                })
                .AsNoTracking();
        }

        public async Task<Device?> GetDeviceByIdAsync(int id)
        {
            return await _context.Devices.FindAsync(id);
        }

        public async Task<bool> IsDuplicate(Device device)
        {
            return await _context.Devices.AnyAsync(d =>
                d.Name == device.Name &&
                d.Manufacturer == device.Manufacturer &&
                d.Processor == device.Processor &&
                d.OperatingSystem == device.OperatingSystem &&
                d.OSVersion == device.OSVersion &&
                d.Type == device.Type &&
                d.RAMAmount == device.RAMAmount
            );
        }

        public async Task<bool> IsDuplicate(NewDevice device)
        {
            return await _context.Devices.AnyAsync(d =>
                d.Name == device.Name &&
                d.Manufacturer == device.Manufacturer &&
                d.Processor == device.Processor &&
                d.OperatingSystem == device.OperatingSystem &&
                d.OSVersion == device.OSVersion &&
                d.Type == device.Type &&
                d.RAMAmount == device.RAMAmount
            );
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
