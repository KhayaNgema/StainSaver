using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Driver.Models;
using StainSaver.Data;
using StainSaver.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StainSaver.Areas.Driver.Controllers
{
    [Area("Driver")]
    [Authorize(Roles = "Driver")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateTime.Today;
            
            // Get counts for dashboard
            var pickupsToday = await _context.Bookings
                .CountAsync(b => b.DriverId == driverId && 
                            b.Status == BookingStatus.PickupAssigned && 
                            b.PickupDate.HasValue && 
                            b.PickupDate.Value.Date == today);
                            
            var deliveriesToday = await _context.Bookings
                .CountAsync(b => b.DriverId == driverId && 
                            b.Status == BookingStatus.Completed);
                            
            var pendingDeliveries = await _context.Bookings
                .CountAsync(b => b.DriverId == driverId && 
                           (b.Status == BookingStatus.PickedUp || 
                            b.Status == BookingStatus.Processing ||
                            b.Status == BookingStatus.StaffAssigned));
                            
            var totalAssigned = await _context.Bookings
                .CountAsync(b => b.DriverId == driverId);

            var totalItemDeliveries = await _context.Deliveries
                .Where(it => it.DriverId == driverId &&
                it.Status == DeliveryStatus.DriverAssigned ||
                it.Status == DeliveryStatus.Delivering)
                .CountAsync();

            var totalPickUps = await _context.PickUps
                .Where(tp => tp.DriverId == driverId &&
                tp.Status == PickUpStatus.DriverAssigned ||
                tp.Status == PickUpStatus.PickingUp)
                .CountAsync();
            
            var model = new DriverDashboardViewModel
            {
                Title = "Driver Dashboard",
                PickupsToday = pickupsToday,
                DeliveriesToday = deliveriesToday,
                PendingDeliveries = pendingDeliveries,
                TotalAssignedDeliveries = totalAssigned,
                ItemDeliveries = totalItemDeliveries,
                ItemsPickUps = totalPickUps
            };
            
            return View(model);
        }
    }
} 