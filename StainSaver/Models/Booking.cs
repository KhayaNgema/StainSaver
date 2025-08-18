using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        PickupAssigned,
        PickedUp,
        Processing,
        StaffAssigned,
        Completed,
        Delivered,
        Cancelled
    }

    public enum DeliveryMethod
    {
        DriverPickupAndDelivery,
        ClientDropoffAndPickup
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;
        
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Display(Name = "Pickup Date")]
        [DataType(DataType.DateTime)]
        public DateTime? PickupDate { get; set; }

        [Display(Name = "Delivery Date")]
        [DataType(DataType.DateTime)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "Special Instructions")]
        public string? SpecialInstructions { get; set; }

        [Required]
        [Display(Name = "Status")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Required]
        [Display(Name = "Delivery Method")]
        public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.DriverPickupAndDelivery;

        // Driver assigned for pickup
        public string? DriverId { get; set; }
        
        [ForeignKey("DriverId")]
        public ApplicationUser? Driver { get; set; }

        // Total amount for the booking
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        // Navigation property for booking details (services selected)
        public ICollection<BookingDetail>? BookingDetails { get; set; }
    }
} 