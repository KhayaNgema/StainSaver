using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StainSaver.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        
        [ForeignKey("BookingId")]
        [ValidateNever]
        public Booking? Booking { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Payment Status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Required]
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // For simulated card payment
        [Required]
        [Display(Name = "Card Number")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}-\d{4}$", ErrorMessage = "Card number format should be XXXX-XXXX-XXXX-XXXX")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiry date format should be MM/YY")]
        public string ExpiryDate { get; set; }

        [Required]
        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV should be 3 digits")]
        public string CVV { get; set; }

        [Required]
        [Display(Name = "Card Holder Name")]
        public string CardHolderName { get; set; }

        [Display(Name = "Transaction Reference")]
        public string? TransactionReference { get; set; }
    }
} 