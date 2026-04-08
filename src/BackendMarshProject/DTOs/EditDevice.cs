using System.ComponentModel.DataAnnotations;

namespace backend_marsh_project.DTOs
{
    public class EditDevice
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The manufacturer is required!")]
        public string OperatingSystem { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The manufacturer is required!")]
        public string OSVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The manufacturer is required!")]
        public string Processor { get; set; }

        [Required(ErrorMessage = "Then RAM amount is requred!")]
        public int RAMAmount { get; set; }
        public string Description { get; set; }
    }
}
