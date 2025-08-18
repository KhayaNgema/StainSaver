using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class CustomerNotification
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public int? BookingId { get; set; }
        
        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        [Required]
        public bool IsRead { get; set; } = false;
    }
} 