using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Staff.Models;
using StainSaver.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StainSaver.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Count orders assigned to this staff member by status
            var newOrders = await _context.BookingDetails
                .CountAsync(bd => bd.StaffId == staffId && bd.Status == "Assigned");
                
            var processingOrders = await _context.BookingDetails
                .CountAsync(bd => bd.StaffId == staffId && bd.Status == "Processing");
                
            var completedOrders = await _context.BookingDetails
                .CountAsync(bd => bd.StaffId == staffId && bd.Status == "Completed");
                
            var totalAssigned = newOrders + processingOrders + completedOrders;
            
            var model = new StaffDashboardViewModel
            {
                Title = "Staff Dashboard",
                NewOrders = newOrders,
                ProcessingOrders = processingOrders,
                ReadyForDelivery = completedOrders,
                TotalAssignedOrders = totalAssigned
            };
            
            return View(model);
        }
    }
} 