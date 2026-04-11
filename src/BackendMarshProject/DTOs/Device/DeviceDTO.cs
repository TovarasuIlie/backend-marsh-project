using BackendMarshProject.Enums;

namespace BackendMarshProject.DTOs.Device
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public DeviceType Type { get; set; }
        public DeviceUserDTO? AssignedToUser { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string Processor { get; set; }
        public int RAMAmount { get; set; }
        public string Description { get; set; }
    }
}
