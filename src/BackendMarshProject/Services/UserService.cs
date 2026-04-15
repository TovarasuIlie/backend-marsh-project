using BackendMarshProject.DTOs.Form;
using BackendMarshProject.DTOs.User;
using BackendMarshProject.Entities;
using BackendMarshProject.Entities.Extensions;
using BackendMarshProject.Entities.Paging;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendMarshProject.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResult<UserDTO>> GetAllUsers(PaginationParameters paginationParameters)
        {
            return await _userRepository.GetUsers().ToPagedResultAsync(paginationParameters);
        }

        public async Task<UserDTO?> GetUser(int id)
        {
            UserDTO? user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                throw new NotFoundException("The user no longer exists in the system.");
            }

            return user;
        }

        public async Task DeleteUser(int id, int adminId)
        {
            User? user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("The user no longer exists in the system.");
            }

            if (user.Id == adminId)
            {
                throw new BadRequestException("You can't delete account by yourself.");
            }

            await _userRepository.DeleteUserAsync(user);
        }

        public async Task<User> UpdateUser(int id, EditUser editUser, int adminId)
        {
            User? user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("The user no longer exists in the system.");
            }

            if (user.Id == adminId && user.Role != editUser.Role)
            {
                throw new BadRequestException("You can't edit your role by yourself.");
            }

            user.Name = editUser.Name;
            user.Role = editUser.Role;
            user.Location = editUser.Location;

            try
            {
                await _userRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Another user updated this user while you were editing.");
            }

            return user;
        }
    }
}
