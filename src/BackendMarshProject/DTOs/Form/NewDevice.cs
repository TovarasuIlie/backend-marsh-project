using BackendMarshProject.Enums;
using System.ComponentModel.DataAnnotations;

namespace BackendMarshProject.DTOs.Form
{
    public class NewDevice
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The name is required!")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The manufacturer is required!")]
        public string Manufacturer { get; set; }
        public DeviceType Type { get; set; }

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
