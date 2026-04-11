using BackendMarshProject.Enums;
using System.ComponentModel.DataAnnotations;

namespace BackendMarshProject.DTOs.Form
{
    public class EditUser
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The name is required!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The role is required!")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid User Role!")]
        public UserRole Role { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The location is required!")]
        public string Location { get; set; }
    }
}
