using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class DeliveryItem
    {
        [Key]
        public int DeliveryItemId { get; set; }

        public int LostOrFoundItemId { get; set; }

        [ForeignKey(nameof(LostOrFoundItemId))]
        public LostOrFoundItem LostOrFoundItem { get; set; }

        public int ComplainId { get; set; }
        public virtual Complain Complain { get; set; }

        public bool IsPackaged { get; set; } = false;

        public bool IsCollected { get; set; } = false;

        public bool IsMissing { get; set; }

        public DateTime? PackagedAt { get; set; }

        public DateTime? CollectionAt { get; set; }
    }
}
