using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class BookingDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        [Required]
        public int LaundryServiceId { get; set; }
        
        [ForeignKey("LaundryServiceId")]
        public LaundryService LaundryService { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // Staff member assigned to this specific task
        public string? StaffId { get; set; }
        
        [ForeignKey("StaffId")]
        public ApplicationUser? Staff { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Completed On")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedOn { get; set; }
    }
} 