namespace StainSaver.Models
{
    public class RefundItem
    {
        public int RefundItemId { get; set; }

        public string RefundItemName { get; set; } 

        public int ComplainId { get; set; }
        public virtual Complain Complain { get; set; }

        public string ImageUrl { get; set; }

    }
}
