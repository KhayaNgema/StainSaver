namespace StainSaver.Areas.Customer.Models
{
    public class CustomerDashboardViewModel
    {
        public string Title { get; set; }
        public int ActiveOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingPayments { get; set; }

        public int ActiveComplains { get; set; }
    }
} 