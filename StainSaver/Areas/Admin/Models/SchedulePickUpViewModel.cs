using StainSaver.Models;

namespace StainSaver.Areas.Admin.Models
{
    public class SchedulePickUpViewModel
    {
        public int ComplainId { get; set; }
        public ComplainType ComplainType { get; set; }

        public DateTime PickUpDate { get; set; }

        public string DriverId { get; set; }
    }
}
