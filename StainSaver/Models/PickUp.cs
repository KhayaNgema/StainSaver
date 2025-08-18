using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class PickUp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PickUpId { get; set; }

        public int ComplainId { get; set; }
        [ForeignKey("ComplainId")]
        public virtual Complain Complain { get; set; }

        public string DriverId { get; set; }
        [ForeignKey("DriverId")]
        public virtual ApplicationUser Driver { get; set; }

        public DateTime PickUpDate { get; set; }

        public bool IsPickedUp { get; set; }

        public PickUpStatus Status { get; set; }

        public string ReferenceNumber { get; set; }

        public string? Comments { get; set; }

        public string? OTP {  get; set; }
    }

    public enum PickUpStatus
    { 
      DriverAssigned,
      PickingUp,
      Completed
    }
}
