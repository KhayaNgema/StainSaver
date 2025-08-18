using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StainSaver.Areas.Driver.Models
{
    public class PickUpConfirmViewModel
    {
        public int PickUpId { get; set; }

        [Display(Name = "Item description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Item Image")]
        public string ItemImage { get; set; }

        [Display(Name = "Items in Complain")]
        public IEnumerable<PickUpItemViewModel> Items { get; set; }

        [Display(Name = "Comments on item condition")]
        public string Comments { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP Code must be a 6 digit number")]
        [Display(Name = "OTP Code")]
        public string OtpCode { get; set; }
    }

    public class PickUpItemViewModel
    {
        public string ItemName { get; set; }
        public string ItemImage { get; set; }
    }
}
