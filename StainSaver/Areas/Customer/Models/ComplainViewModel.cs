using StainSaver.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace StainSaver.Areas.Customer.Models
{
    public class ComplainViewModel
    {
        public int BookingId { get; set; }

        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; }

        [Display(Name = "Complain Type")]
        public ComplainType ComplainType { get; set; }

        [Display(Name = "Bank Account Type")]
        public BankAccountType? BankAccountType { get; set; }

        [Display(Name = "Bank Account Number")]
        public string? BankAccountNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Proof of Damage")]
        public IFormFile? ProofOfDamage { get; set; }

        [Display(Name = "Proof of Payment")]
        public IFormFile? ProofOfPayment { get; set; }

        [Display(Name = "Is Lost")]
        public bool IsLost { get; set; }

        [Display(Name = "Is Found")]
        public bool IsFound { get; set; }

        [Display(Name = "Item Lost or Found")]
        public string? ItemLostOrFound { get; set; }

        [Display(Name = "Lost or Found Date")]
        public DateTime? LostOrFoundDate { get; set; }

        [Display(Name = "Image of Lost or Found Item")]
        public IFormFile? ImageOfLostOrFoundItem { get; set; }

        public Bank Bank { get; set; }

        public string LostFoundDescription { get; set; }

        public RefundTo ReasonForRefund { get; set; }

        // Collection of multiple refund items for Refund Complains
        public List<RefundItemViewModel>? RefundItems { get; set; } = new List<RefundItemViewModel>();

        // Collection of multiple lost or found items for Lost and Found Complains
        public List<LostOrFoundItemViewModel>? LostOrFoundItems { get; set; } = new List<LostOrFoundItemViewModel>();
    }

    public class RefundItemViewModel
    {
        [Display(Name = "Refund Item Name")]
        public string RefundItemName { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }
    }

    public class LostOrFoundItemViewModel
    {
        [Display(Name = "Item Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }
    }
}
