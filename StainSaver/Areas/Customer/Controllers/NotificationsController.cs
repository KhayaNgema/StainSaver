using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Customer.Models;
using StainSaver.Data;
using StainSaver.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StainSaver.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Notifications
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var notifications = await _context.CustomerNotifications
                .Include(n => n.Booking)
                .Where(n => n.CustomerId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
            
            var viewModel = new NotificationViewModel
            {
                Notifications = notifications,
                UnreadCount = notifications.Count(n => !n.IsRead)
            };
            
            return View(viewModel);
        }
        
        // GET: Customer/Notifications/MarkAsRead/5
        public async Task<IActionResult> MarkAsRead(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var notification = await _context.CustomerNotifications
                .FirstOrDefaultAsync(n => n.Id == id && n.CustomerId == userId);
                
            if (notification == null)
            {
                return NotFound();
            }
            
            notification.IsRead = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Customer/Notifications/MarkAllAsRead
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var unreadNotifications = await _context.CustomerNotifications
                .Where(n => n.CustomerId == userId && !n.IsRead)
                .ToListAsync();
                
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                _context.Update(notification);
            }
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Customer/Notifications/GetUnreadCount
        [HttpGet]
        public async Task<JsonResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var unreadCount = await _context.CustomerNotifications
                .CountAsync(n => n.CustomerId == userId && !n.IsRead);
                
            return Json(new { unreadCount });
        }
    }
} 