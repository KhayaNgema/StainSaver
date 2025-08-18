using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Driver.Models;
using StainSaver.Data;
using StainSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StainSaver.Areas.Driver.Controllers
{
    [Area("Driver")]
    [Authorize(Roles = "Driver")]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Driver/Tasks
        public async Task<IActionResult> Index()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var assignedBookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .Where(b => b.DriverId == driverId && 
                       (b.Status == BookingStatus.PickupAssigned || 
                        b.Status == BookingStatus.PickedUp ||
                        b.Status == BookingStatus.Completed ||
                        b.Status == BookingStatus.Processing ||
                        b.Status == BookingStatus.StaffAssigned))
                .OrderByDescending(b => b.PickupDate)
                .ToListAsync();

            return View(assignedBookings);
        }

        // GET: Driver/Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .FirstOrDefaultAsync(b => b.Id == id && b.DriverId == driverId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Driver/Tasks/MarkAsPickedUp/5
        public async Task<IActionResult> MarkAsPickedUp(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.Id == id && b.DriverId == driverId && b.Status == BookingStatus.PickupAssigned);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found or you are not authorized to update it.";
                return RedirectToAction(nameof(Index));
            }

            booking.Status = BookingStatus.PickedUp;
            booking.PickupDate = DateTime.Now; // Update the actual pickup time
            _context.Update(booking);
            
            // Store notification for customer
            var notification = new CustomerNotification
            {
                CustomerId = booking.CustomerId,
                Title = "Laundry Picked Up",
                Message = $"Your laundry for booking #{booking.Id} has been picked up by our driver.",
                BookingId = booking.Id,
                CreatedDate = DateTime.Now,
                IsRead = false
            };
            
            _context.CustomerNotifications.Add(notification);
            
            // Store notification for admin
            var adminNotification = new AdminNotification
            {
                Title = "Laundry Picked Up",
                Message = $"Booking #{booking.Id} has been picked up by the driver and is now at the facility.",
                BookingId = booking.Id,
                CreatedDate = DateTime.Now,
                IsRead = false,
                NotificationType = AdminNotificationType.BookingUpdate
            };
            
            _context.AdminNotifications.Add(adminNotification);
            
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Booking successfully marked as picked up.";
            return RedirectToAction(nameof(Details), new { id = booking.Id });
        }

        // GET: Driver/Tasks/MarkAsDelivered/5
        public async Task<IActionResult> MarkAsDelivered(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.Id == id && b.DriverId == driverId && b.Status == BookingStatus.Completed);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found or you are not authorized to update it.";
                return RedirectToAction(nameof(Index));
            }

            booking.Status = BookingStatus.Delivered;
            booking.DeliveryDate = DateTime.Now;
            _context.Update(booking);
            
            // Store notification for customer
            var notification = new CustomerNotification
            {
                CustomerId = booking.CustomerId,
                Title = "Laundry Delivered",
                Message = $"Your laundry for booking #{booking.Id} has been delivered. Thank you for using our service!",
                BookingId = booking.Id,
                CreatedDate = DateTime.Now,
                IsRead = false
            };
            
            _context.CustomerNotifications.Add(notification);
            
            // Store notification for admin
            var adminNotification = new AdminNotification
            {
                Title = "Laundry Delivered",
                Message = $"Booking #{booking.Id} has been successfully delivered to the customer.",
                BookingId = booking.Id,
                CreatedDate = DateTime.Now,
                IsRead = false,
                NotificationType = AdminNotificationType.BookingUpdate
            };
            
            _context.AdminNotifications.Add(adminNotification);
            
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Booking successfully marked as delivered.";
            return RedirectToAction(nameof(Details), new { id = booking.Id });
        }
    }
} 