using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;

namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FileUploadService _fileUploadService;
        public DashboardsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            FileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboards()
        {
            return View();
        }

        public async Task<IActionResult> LostAndFoundReports()
        {
            var complains = await _context.Complains
                .ToListAsync();

            return View(complains ?? new List<Complain>());
        }



        [HttpGet]
        public async Task<IActionResult> BookingReports(DateTime? from, DateTime? to)
        {
            var bookings = await _context.Bookings.ToListAsync();

            if (from.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate.Date >= from.Value.Date).ToList();
            }
            if (to.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate.Date <= to.Value.Date).ToList();
            }

            return View(bookings);
        }


        public async Task<IActionResult> RefundReports()
        {
            return View();
        }
    }
}
