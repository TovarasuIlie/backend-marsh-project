using BackendMarshProject.DTOs.Form;
using BackendMarshProject.DTOs.User;
using BackendMarshProject.Entities;
using BackendMarshProject.Entities.Extensions;
using BackendMarshProject.Entities.Paging;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Repository;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteUser(int id)
        {
            User? user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("The user no longer exists in the system.");
            }

            await _userRepository.DeleteUserAsync(user);
        }

        public async Task<User> UpdateUser(int id, EditUser editUser)
        {
            User? user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("The user no longer exists in the system.");
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
