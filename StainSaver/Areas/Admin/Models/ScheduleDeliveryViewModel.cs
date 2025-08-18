using StainSaver.Models;

namespace StainSaver.Areas.Admin.Models
{
    public class ScheduleDeliveryViewModel
    {
        public int ComplainId { get; set; }
        public ComplainType ComplainType { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string DriverId { get; set; }

        public int PackageId { get; set; }

        public string ReferenceNumber { get; set; }

        public byte[]? Barcode { get; set; }
    }
}
