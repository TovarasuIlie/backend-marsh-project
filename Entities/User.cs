using backend_marsh_project.Enums;
using System.Text.Json.Serialization;

namespace backend_marsh_project.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string Location { get; set; }
    }
}
