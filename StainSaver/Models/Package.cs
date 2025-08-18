using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class Package
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackageId { get; set; }

        public string? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public virtual ApplicationUser Driver { get; set; }

        public int? ComplainId { get; set; }

        [ForeignKey("ComplainId")]
        public virtual Complain Complain { get; set; }

        public string ReferenceNumber { get; set; }

        public byte[]? BarcodeImage { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public ApplicationUser Admin { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
