using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
        public class LostOrFoundItem
        {
            [Key]
            public int LostOrFoundItemId { get; set; }

            [ForeignKey("Complain")]
            public int ComplainId { get; set; }

            public Complain Complain { get; set; }

            [Display(Name = "Item Description")]
            public string? ItemDescription { get; set; }

            [Display(Name = "Image of Lost or Found Item")]
            public string ImageUrl { get; set; }
        } 
    }
