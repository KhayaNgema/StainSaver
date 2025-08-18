using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class Delivery
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryId { get; set; }

        public int ComplainId { get; set; }
        [ForeignKey("ComplainId")]
        public virtual Complain Complain { get; set; }

        public string DriverId { get; set; }
        [ForeignKey("DriverId")]
        public virtual ApplicationUser Driver { get; set; }

        public DateTime DeliveryDate { get; set; }

        public bool IsDelivered { get; set; }

        public DeliveryStatus Status { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }
    }

    public enum DeliveryStatus
    {
        DriverAssigned,
        Delivering,
        Delivered
    }
}
