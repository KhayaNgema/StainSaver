using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public enum AdminNotificationType
    {
        BookingCreated,
        BookingUpdate,
        StaffAssigned,
        PaymentReceived,
        SystemAlert,
        Review,
        Other
    }
    
    public class AdminNotification
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public int? BookingId { get; set; }
        
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public bool IsRead { get; set; }
        
        public AdminNotificationType NotificationType { get; set; }
    }
} 