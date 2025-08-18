using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Data;
using StainSaver.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Reviews
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Booking)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return View(reviews);
        }

        // GET: Admin/Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Booking)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(bd => bd.LaundryService)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }
    }
} 