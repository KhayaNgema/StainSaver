using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        
        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [Required]
        [Display(Name = "Rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [Display(Name = "Comments")]
        [MaxLength(500)]
        public string Comments { get; set; }

        [Required]
        [Display(Name = "Review Date")]
        [DataType(DataType.DateTime)]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        // The customer who submitted the review
        [Required]
        public string CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }
    }
} 