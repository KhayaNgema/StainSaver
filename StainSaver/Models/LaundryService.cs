using System.ComponentModel.DataAnnotations;

namespace StainSaver.Models
{
    public enum BasketSize
    {
        Small,
        Medium,
        Large
    }

    public class LaundryService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Service Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Service Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Basket Size")]
        public BasketSize Size { get; set; }

        [Required]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // To determine if this is a premium service (like blankets)
        public bool IsPremium { get; set; } = false;
    }
} 