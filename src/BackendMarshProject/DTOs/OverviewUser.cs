using BackendMarshProject.Entities;
using BackendMarshProject.Enums;

namespace BackendMarshProject.DTOs
{
    public class OverviewUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Device> Devices { get; set; }
        public UserRole Role { get; set; }
        public string Location { get; set; }
    }
}
