using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class DeliveriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliveriesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Driver/Deliveries/Pickups
        public async Task<IActionResult> Pickups()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var today = DateTime.Today;
            
            var pickups = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .Where(b => b.DriverId == driverId && 
                       b.Status == BookingStatus.PickupAssigned && 
                       b.PickupDate.HasValue && 
                       b.PickupDate.Value.Date == today)
                .OrderBy(b => b.PickupDate)
                .ToListAsync();

            return View(pickups);
        }

        // GET: Driver/Deliveries/TodayDeliveries
        public async Task<IActionResult> TodayDeliveries()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateTime.Today;
            
            var deliveries = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .Where(b => b.DriverId == driverId && 
                       b.Status == BookingStatus.Completed && 
                       b.PickupDate.HasValue)
                .OrderBy(b => b.PickupDate)
                .ToListAsync();

            return View(deliveries);
        }

        // GET: Driver/Deliveries/Pending
        public async Task<IActionResult> Pending()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var pending = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .Where(b => b.DriverId == driverId && 
                      (b.Status == BookingStatus.PickedUp || 
                       b.Status == BookingStatus.Processing ||
                       b.Status == BookingStatus.StaffAssigned))
                .OrderBy(b => b.PickupDate)
                .ToListAsync();

            return View(pending);
        }

        // GET: Driver/Deliveries/All
        public async Task<IActionResult> All()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var allDeliveries = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .Where(b => b.DriverId == driverId)
                .OrderByDescending(b => b.PickupDate)
                .ToListAsync();

            return View(allDeliveries);
        }
    }
} 