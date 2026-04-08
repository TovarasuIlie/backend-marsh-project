using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendMarshProject.DTOs
{
    public class LoginUser
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The password is required!")]
        public string Password { get; set; }
    }
}
