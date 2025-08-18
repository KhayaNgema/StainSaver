using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class Feedback
    {
           [Key ,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int FeedbackId { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]    
        public virtual ApplicationUser Customer { get; set; }

            public Rating ServiceRating { get; set; }

            public string? Comments { get; set; }

            public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
        }

    }

