using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public enum DetergentType
    {
        Liquid,
        Powder,
        Bleach,
        Anionic
    }

    public class BookingPreferences
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        [Required]
        [Display(Name = "Detergent Type")]
        public DetergentType DetergentType { get; set; } = DetergentType.Liquid;

        [Required]
        [Display(Name = "Laundry Bag Required")]
        public bool LaundryBagRequired { get; set; } = false;

        [Display(Name = "T-Shirts")]
        public int TShirtsCount { get; set; } = 0;

        [Display(Name = "Dresses")]
        public int DressesCount { get; set; } = 0;

        [Display(Name = "Trousers")]
        public int TrousersCount { get; set; } = 0;

        [Display(Name = "Blankets")]
        public int BlanketsCount { get; set; } = 0;

        [Display(Name = "Terms & Conditions Accepted")]
        public bool TermsAccepted { get; set; } = false;
    }
} 