using BackendMarshProject.Entities;
using BackendMarshProject.Enums;

namespace BackendMarshProject.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<UserDeviceDTO> Devices { get; set; } = new List<UserDeviceDTO>();
        public UserRole Role { get; set; }
        public string Location { get; set; }
    }
}
