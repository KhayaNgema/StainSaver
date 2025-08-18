using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Customer.Models;
using StainSaver.Data;
using StainSaver.Models;
using System.Security.Claims;

namespace StainSaver.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get active, completed, and pending payment bookings for this customer
            var activeBookings = await _context.Bookings
                .Where(b => b.CustomerId == userId && 
                       (b.Status == BookingStatus.Pending || 
                        b.Status == BookingStatus.Confirmed || 
                        b.Status == BookingStatus.PickupAssigned || 
                        b.Status == BookingStatus.PickedUp || 
                        b.Status == BookingStatus.Processing))
                .CountAsync();

            var completedBookings = await _context.Bookings
                .Where(b => b.CustomerId == userId && 
                       (b.Status == BookingStatus.Completed || 
                        b.Status == BookingStatus.Delivered))
                .CountAsync();


            var activeComplains = await _context.Complains
                .Where(b => b.CustomerId == userId &&
                b.Status == ComplainStatus.Review ||
                b.Status == ComplainStatus.DriverAssigned ||
                b.Status == ComplainStatus.AwaitingCustomer ||
                b.Status == ComplainStatus.Approved)
                .CountAsync();

            // Get bookings where payment is pending
            var pendingPayments = await _context.Bookings
                .Where(b => b.CustomerId == userId && 
                      !_context.Payments.Any(p => p.BookingId == b.Id && p.Status == PaymentStatus.Completed))
                .CountAsync();

            var model = new CustomerDashboardViewModel
            {
                Title = "Customer Dashboard",
                ActiveOrders = activeBookings,
                CompletedOrders = completedBookings,
                PendingPayments = pendingPayments,
                ActiveComplains = activeComplains
            };
            
            return View(model);
        }
    }
} 