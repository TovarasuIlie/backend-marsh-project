using BackendMarshProject.Data;
using BackendMarshProject.DTOs;
using BackendMarshProject.DTOs.Form;
using BackendMarshProject.DTOs.User;
using BackendMarshProject.Entities;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendMarshProject.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;


        public AuthService(IUserRepository userRepository, IJWTService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoggedUser> RegisterUser(RegisterUser registerUser)
        {
            if(await _userRepository.IsDuplicateEmail(registerUser.Email))
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

            await _userRepository.AddUserAsync(user);

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
            var user = await _userRepository.GetUserByEmail(loginUser.Email);

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

        public async Task<UserDTO> GetUserData(int userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found!");
            }

            return user;
        }
    }
}
