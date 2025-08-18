using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class Refund
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RefundId { get; set; }

        public int ComplainId { get; set; }

        [ForeignKey("ComplainId")]
        public virtual Complain Complain { get; set; }

        public decimal CouponBonus { get; set; }

        public decimal RefundedAmount { get; set; }       

        public RefundStatus Status { get; set; }          

        public ICollection<RefundValidationEntry> RefundValidationEntries { get; set; } = new List<RefundValidationEntry>();

        public ICollection<RefundPolicyEntry> RefundPolicyEntries { get; set; } = new List<RefundPolicyEntry>();
    }

    public enum RefundStatus
    {
        Processed,
        Rejected,
        Cancelled,
        Refunded,
        Escalated
    }


    public class RefundValidationEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RefundValidationEntryId { get; set; }

        public int RefundId { get; set; }
        [ForeignKey("RefundId")]
        public Refund Refund { get; set; }

        public RefundValidation RefundValidation { get; set; }
    }


    public class RefundPolicyEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RefundPolicyEntryId { get; set; }

        public int RefundId { get; set; }
        [ForeignKey("RefundId")]
        public Refund Refund { get; set; }

        public RefundPolicy RefundPolicy { get; set; }
    }

    public enum RefundValidation
    {
        [Display(Name = "Valid Complaint Missing Items")]
        ValidComplaintMissingItems,

        [Display(Name = "Evidence Of Service Failure")]
        EvidenceOfServiceFailure,

        [Display(Name = "Client Followed Return Policy")]
        ClientFollowedReturnPolicy
    }

    public enum RefundPolicy
    {
        [Display(Name = "Late Claim Deduction")]
        LateClaimDeduction,

        [Display(Name = "Policy Deduction")]
        PolicyDeduction
    }

    public static class RefundPolicyData
    {
        public static readonly Dictionary<RefundPolicy, decimal> RefundPolicyPercentages = new()
        {
            { RefundPolicy.LateClaimDeduction, 0.15m },
            { RefundPolicy.PolicyDeduction, 0.10m }
        };
    }
}
