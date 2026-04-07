using backend_marsh_project.Data;
using backend_marsh_project.DTOs;
using backend_marsh_project.Entities;
using backend_marsh_project.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend_marsh_project.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IJWTService _jwtService;


        public AuthService(AppDbContext context, IJWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoggedUser> RegisterUser(RegisterUser registerUser)
        {
            var duplicateEmail = await _context.Users.AnyAsync(u => u.Email == registerUser.Email);

            if(duplicateEmail)
            {
                throw new BadRequestException("This email address is took by another user!");
            }

            var hasher = new PasswordHasher<User>();

            User user = new User
            {
                Name = registerUser.Name,
                Email = registerUser.Email,
                Password = hasher.HashPassword(new User(), registerUser.Password),
                Role = Enums.UserRole.Employee,
                Location = registerUser.Location
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new LoggedUser
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Location = user.Location,
                Token = _jwtService.GenerateToken(user)
            };
        }

        public async Task<LoggedUser> LoginUser(LoginUser loginUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);

            if (user == null)
            {
                throw new BadRequestException("Email or password is incorrect!");
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, loginUser.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Email or password is incorrect!");
            }

            return new LoggedUser
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Location = user.Location,
                Token = _jwtService.GenerateToken(user)
            };
        }
    }
}
