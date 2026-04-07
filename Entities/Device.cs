using backend_marsh_project.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_marsh_project.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public User? AssignedToUser { get; set; }
        public DeviceType Type { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string Processor {  get; set; }
        public int RAMAmount { get; set; }
        public string Description { get; set; }
    }
}
