using backend_marsh_project.Enums;

namespace backend_marsh_project.DTOs
{
    public class LoggedUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Location { get; set; }

        public string Token { get; set; }
    }
}
