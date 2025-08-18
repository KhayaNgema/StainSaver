using StainSaver.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Areas.Admin.Models
{
    public class ProcessRefundViewModel
    {
        public int ComplainId { get; set; }
        public List<RefundValidation> RefundValidations { get; set; }
        public decimal CouponBonus { get; set; }
        public List<RefundPolicy> RefundPolicies { get; set; }

        public List<RefundDisplayItemViewModel> RefundItems { get; set; } = new List<RefundDisplayItemViewModel>();
    }

    public class RefundDisplayItemViewModel
    {
        [Display(Name = "Refund Item Name")]
        public string RefundItemName { get; set; }

        [Display(Name = "Image")]
        public string ImageFile { get; set; }
    }

    public class LostOrFoundDisplayItemViewModel
    {
        [Display(Name = "Item Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Image")]
        public string? ImageFile { get; set; }

        public int LostOrFoundItemId { get; set; }

        public bool IsPackaged { get; set; }
    }
}
