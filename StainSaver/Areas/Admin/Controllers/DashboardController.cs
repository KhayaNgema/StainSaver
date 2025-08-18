using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Admin.Models;
using StainSaver.Data;
using StainSaver.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DashboardController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel();
            
            try
            {
                // Get user counts
                model.TotalCustomers = await _userManager.GetUsersInRoleAsync("Customer").ContinueWith(t => t.Result.Count);
                model.TotalStaff = await _userManager.GetUsersInRoleAsync("Staff").ContinueWith(t => t.Result.Count);
                model.TotalDrivers = await _userManager.GetUsersInRoleAsync("Driver").ContinueWith(t => t.Result.Count);
                
                try
                {
                    // These operations may fail if tables don't exist
                    model.TotalRefundComplains = await _context.Complains
                        .Where(c => c.ComplainType == ComplainType.Refund &&
                                    (c.Status == ComplainStatus.AwaitingCustomer ||
                                     c.Status == ComplainStatus.DriverAssigned ||
                                     c.Status == ComplainStatus.Approved ||
                                     c.Status == ComplainStatus.Review))
                        .CountAsync();

                    model.TotalLostOrFoundComplains = await _context.Complains
                        .Where(c => c.ComplainType == ComplainType.Lost_and_found &&
                                    (c.Status == ComplainStatus.AwaitingCustomer ||
                                     c.Status == ComplainStatus.DriverAssigned ||
                                     c.Status == ComplainStatus.Approved ||
                                     c.Status == ComplainStatus.Review))
                        .CountAsync();


                    model.TotalBookings = await _context.Bookings.CountAsync();
                    model.PendingBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending);
                    model.CompletedBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Completed);
                    model.TotalRevenue = await _context.Bookings
                        .Where(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Delivered)
                        .SumAsync(b => b.TotalAmount);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error retrieving booking metrics");
                    // Set default values if tables don't exist
                    model.TotalBookings = 0;
                    model.PendingBookings = 0;
                    model.CompletedBookings = 0;
                    model.TotalRevenue = 0;
                }
                
                try
                {
                    model.RecentBookings = await _context.Bookings
                        .Include(b => b.Customer)
                        .OrderByDescending(b => b.BookingDate)
                        .Take(5)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error retrieving recent bookings");
                    model.RecentBookings = new List<Booking>();
                }
                
                try
                {
                    model.PendingAssignments = await _context.BookingDetails
                        .Include(bd => bd.Booking)
                            .ThenInclude(b => b.Customer)
                        .Include(bd => bd.LaundryService)
                        .Where(bd => string.IsNullOrEmpty(bd.StaffId) && bd.Booking.Status == BookingStatus.Processing)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error retrieving pending assignments");
                    model.PendingAssignments = new List<BookingDetail>();
                }
                
                try
                {
                    model.UnassignedPickups = await _context.Bookings
                        .Include(b => b.Customer)
                        .Where(b => b.Status == BookingStatus.Confirmed && string.IsNullOrEmpty(b.DriverId))
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error retrieving unassigned pickups");
                    model.UnassignedPickups = new List<Booking>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Admin Dashboard");
            }

            return View(model);
        }
    }
} 