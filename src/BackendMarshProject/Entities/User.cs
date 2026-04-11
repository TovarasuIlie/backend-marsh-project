using BackendMarshProject.DTOs.User;
using BackendMarshProject.Enums;
using System.Text.Json.Serialization;

namespace BackendMarshProject.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
        public UserRole Role { get; set; }
        public string Location { get; set; }
    }
}
