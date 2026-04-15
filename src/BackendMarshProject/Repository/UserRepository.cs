using BackendMarshProject.Data;
using BackendMarshProject.DTOs.User;
using BackendMarshProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendMarshProject.Repository
{
    public interface IUserRepository
    {
        IQueryable<UserDTO> GetUsers();
        Task<UserDTO?> GetUserById(int id);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmail(string email);
        Task<bool> IsDuplicateEmail(string email);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(User user);

        Task SaveChangesAsync();
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext appDbContext) 
        {
            _context = appDbContext;
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserDTO?> GetUserById(int id) 
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    Email = u.Email,
                    Location = u.Location,
                    Devices = u.Devices.Select(d => new UserDeviceDTO
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Manufacturer = d.Manufacturer,
                        OperatingSystem = d.OperatingSystem,
                        OSVersion = d.OSVersion,
                        RAMAmount = d.RAMAmount,
                        Processor = d.Processor,
                        Type = d.Type,
                        Description = d.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public IQueryable<UserDTO> GetUsers()
        {
            return _context.Users
                .Include(u => u.Devices)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    Email = u.Email,
                    Location = u.Location,
                    Devices = u.Devices.Select(d => new UserDeviceDTO
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Manufacturer = d.Manufacturer,
                        OperatingSystem = d.OperatingSystem,
                        OSVersion = d.OSVersion,
                        RAMAmount = d.RAMAmount,
                        Processor = d.Processor,
                        Type = d.Type,
                        Description = d.Description
                    }).ToList()
                });
        }

        public async Task<bool> IsDuplicateEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
