using StainSaver.Models;

namespace StainSaver.Areas.Customer.Models
{
    public class BookingViewModel
    {
        public Booking Booking { get; set; }
        public BookingPreferences BookingPreferences { get; set; }
        public Payment Payment { get; set; }
        public IEnumerable<LaundryService> LaundryServices { get; set; }
    }

    public class BookingDetailsViewModel
    {
        public Booking Booking { get; set; }
        public BookingPreferences BookingPreferences { get; set; }
        public Payment Payment { get; set; }
    }
} 