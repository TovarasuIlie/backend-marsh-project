using BackendMarshProject.Entities;
using BackendMarshProject.Enums;

namespace BackendMarshProject.DTOs.Device
{
    public class DeviceUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Location { get; set; }
    }
}
