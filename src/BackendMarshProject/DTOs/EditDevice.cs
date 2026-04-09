using System.ComponentModel.DataAnnotations;

namespace BackendMarshProject.DTOs
{
    public class EditDevice
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The Operating System is required!")]
        public string OperatingSystem { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The OS Version is required!")]
        public string OSVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Processor is required!")]
        public string Processor { get; set; }

        [Required(ErrorMessage = "Then RAM amount is requred!")]
        [Range(1, 1024, ErrorMessage = "The RAM amount must be between 1 and 1024!")]
        public int RAMAmount { get; set; }
        public string Description { get; set; }
    }
}
