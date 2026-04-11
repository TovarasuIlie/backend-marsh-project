using BackendMarshProject.Entities;
using BackendMarshProject.Enums;

namespace BackendMarshProject.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<DeviceDTO> Devices { get; set; } = new List<DeviceDTO>();
        public UserRole Role { get; set; }
        public string Location { get; set; }
    }
}
