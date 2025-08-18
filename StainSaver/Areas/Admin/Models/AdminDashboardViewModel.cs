using StainSaver.Models;
using System.Collections.Generic;

namespace StainSaver.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalStaff { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CompletedBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Booking> RecentBookings { get; set; } = new List<Booking>();
        public List<BookingDetail> PendingAssignments { get; set; } = new List<BookingDetail>();
        public List<Booking> UnassignedPickups { get; set; } = new List<Booking>();
    }
} 