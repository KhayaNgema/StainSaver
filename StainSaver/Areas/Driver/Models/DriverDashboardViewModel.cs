namespace StainSaver.Areas.Driver.Models
{
    public class DriverDashboardViewModel
    {
        public string Title { get; set; }
        public int PickupsToday { get; set; }
        public int DeliveriesToday { get; set; }
        public int TotalAssignedDeliveries { get; set; }
        public int PendingDeliveries { get; set; }
    }
} 