namespace StainSaver.Areas.Staff.Models
{
    public class StaffDashboardViewModel
    {
        public string Title { get; set; }
        public int NewOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ReadyForDelivery { get; set; }
        public int TotalAssignedOrders { get; set; }
    }
} 