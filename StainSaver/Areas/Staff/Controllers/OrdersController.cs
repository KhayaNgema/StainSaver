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

namespace StainSaver.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Staff/Orders/Assigned
        public async Task<IActionResult> Assigned()
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var assignedOrders = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .Where(bd => bd.StaffId == staffId && bd.Status == "Assigned")
                .OrderByDescending(bd => bd.Booking.PickupDate)
                .ToListAsync();

            return View(assignedOrders);
        }

        // GET: Staff/Orders/Processing
        public async Task<IActionResult> Processing()
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var processingOrders = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .Where(bd => bd.StaffId == staffId && bd.Status == "Processing")
                .OrderByDescending(bd => bd.Booking.PickupDate)
                .ToListAsync();

            return View(processingOrders);
        }

        // GET: Staff/Orders/Completed
        public async Task<IActionResult> Completed()
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var completedOrders = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .Where(bd => bd.StaffId == staffId && bd.Status == "Completed")
                .OrderByDescending(bd => bd.CompletedOn)
                .ToListAsync();

            return View(completedOrders);
        }

        // GET: Staff/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .FirstOrDefaultAsync(bd => bd.Id == id && bd.StaffId == staffId);

            if (bookingDetail == null)
            {
                return NotFound();
            }

            return View(bookingDetail);
        }

        // GET: Staff/Orders/StartProcessing/5
        public async Task<IActionResult> StartProcessing(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.Booking)
                .Include(bd => bd.LaundryService)
                .Include(bd => bd.Booking.Customer)
                .FirstOrDefaultAsync(bd => bd.Id == id && bd.StaffId == staffId && bd.Status == "Assigned");

            if (bookingDetail == null)
            {
                return NotFound();
            }

            // Update the booking detail status
            bookingDetail.Status = "Processing";
            _context.Update(bookingDetail);
            
            // Always update the parent booking status to Processing
            if (bookingDetail.Booking != null)
            {
                var booking = await _context.Bookings.FindAsync(bookingDetail.BookingId);
                if (booking != null)
                {
                    booking.Status = BookingStatus.Processing;
                    _context.Update(booking);
                    
                    // Create notification for customer
                    try
                    {
                        var notification = new CustomerNotification
                        {
                            CustomerId = bookingDetail.Booking.CustomerId,
                            Title = "Service Processing Started",
                            Message = $"Your {bookingDetail.LaundryService.Name} service for booking #{bookingDetail.BookingId} is now being processed.",
                            BookingId = bookingDetail.BookingId,
                            CreatedDate = DateTime.Now,
                            IsRead = false
                        };
                        
                        _context.CustomerNotifications.Add(notification);
                    }
                    catch (Exception ex)
                    {
                        // Log the notification creation error but don't fail the whole operation
                        System.Diagnostics.Debug.WriteLine($"Error creating notification: {ex.Message}");
                    }
                }
            }
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Processing));
        }

        // GET: Staff/Orders/MarkComplete/5
        public async Task<IActionResult> MarkComplete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.Booking)
                .Include(bd => bd.LaundryService)
                .FirstOrDefaultAsync(bd => bd.Id == id && bd.StaffId == staffId && bd.Status == "Processing");

            if (bookingDetail == null)
            {
                return NotFound();
            }

            // Update booking detail
            bookingDetail.Status = "Completed";
            bookingDetail.CompletedOn = DateTime.Now;
            
            _context.Update(bookingDetail);
            
            // Create notification for customer
            try
            {
                var notification = new CustomerNotification
                {
                    CustomerId = bookingDetail.Booking.CustomerId,
                    Title = "Service Completed",
                    Message = $"Your {bookingDetail.LaundryService.Name} service for booking #{bookingDetail.BookingId} has been completed.",
                    BookingId = bookingDetail.BookingId,
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };
                
                _context.CustomerNotifications.Add(notification);
            }
            catch (Exception ex)
            {
                // Log the notification creation error but don't fail the whole operation
                System.Diagnostics.Debug.WriteLine($"Error creating notification: {ex.Message}");
            }
            
            await _context.SaveChangesAsync();

            // Check if all booking details are completed
            var allDetailsCompleted = await _context.BookingDetails
                .Where(bd => bd.BookingId == bookingDetail.BookingId)
                .AllAsync(bd => bd.Status == "Completed");

            if (allDetailsCompleted)
            {
                // Load the booking directly to ensure proper tracking
                var booking = await _context.Bookings
                    .FindAsync(bookingDetail.BookingId);
                
                if (booking != null)
                {
                    booking.Status = BookingStatus.Completed;
                    _context.Update(booking);
                    
                    // Create notification about the entire booking being completed
                    try
                    {
                        var bookingNotification = new CustomerNotification
                        {
                            CustomerId = booking.CustomerId,
                            Title = "Booking Completed",
                            Message = $"All services for your booking #{booking.Id} have been completed and are ready for delivery.",
                            BookingId = booking.Id,
                            CreatedDate = DateTime.Now,
                            IsRead = false
                        };
                        
                        _context.CustomerNotifications.Add(bookingNotification);
                    }
                    catch (Exception ex)
                    {
                        // Log the notification creation error but don't fail the whole operation
                        System.Diagnostics.Debug.WriteLine($"Error creating booking completion notification: {ex.Message}");
                    }
                    
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Completed));
        }
        
        // GET: Staff/Orders/CancelTask/5
        public async Task<IActionResult> CancelTask(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.Booking)
                .FirstOrDefaultAsync(bd => bd.Id == id && bd.StaffId == staffId && 
                                    (bd.Status == "Assigned" || bd.Status == "Processing"));

            if (bookingDetail == null)
            {
                return NotFound();
            }

            // Store booking ID before updating
            var bookingId = bookingDetail.BookingId;

            // Reset staff assignment
            bookingDetail.Status = "Pending";
            bookingDetail.StaffId = null;
            
            _context.Update(bookingDetail);
            await _context.SaveChangesAsync();

            // Check if this was the last assigned task for this booking
            var anyStaffAssigned = await _context.BookingDetails
                .AnyAsync(bd => bd.BookingId == bookingId && bd.StaffId != null);

            if (!anyStaffAssigned)
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == bookingId);
                
                if (booking != null && booking.Status == BookingStatus.StaffAssigned)
                {
                    booking.Status = BookingStatus.Processing;
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Task has been successfully removed from your queue.";
            return RedirectToAction(nameof(Assigned));
        }

        // GET: Staff/Orders/New
        public async Task<IActionResult> New()
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var newOrders = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .Where(bd => bd.Status == "Pending" && bd.StaffId == null)
                .OrderByDescending(bd => bd.Booking.PickupDate)
                .ToListAsync();

            return View(newOrders);
        }

        // GET: Staff/Orders/AssignToMe/5
        public async Task<IActionResult> AssignToMe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.Booking)
                .Include(bd => bd.LaundryService)
                .FirstOrDefaultAsync(bd => bd.Id == id && bd.Status == "Pending" && bd.StaffId == null);

            if (bookingDetail == null)
            {
                return NotFound();
            }

            // Assign to this staff member
            bookingDetail.Status = "Assigned";
            bookingDetail.StaffId = staffId;
            
            _context.Update(bookingDetail);
            
            // Update booking status if this is the first assignment
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.Id == bookingDetail.BookingId);
                
            if (booking != null)
            {
                if (booking.Status != BookingStatus.StaffAssigned)
                {
                    booking.Status = BookingStatus.StaffAssigned;
                    _context.Update(booking);
                }
                
                // Get the staff member details
                var staffMember = await _userManager.FindByIdAsync(staffId);
                
                // Create notification for customer
                try
                {
                    var notification = new CustomerNotification
                    {
                        CustomerId = booking.CustomerId,
                        Title = "Staff Assigned",
                        Message = $"A staff member has been assigned to process your {bookingDetail.LaundryService.Name} service for booking #{booking.Id}.",
                        BookingId = booking.Id,
                        CreatedDate = DateTime.Now,
                        IsRead = false
                    };
                    
                    _context.CustomerNotifications.Add(notification);
                }
                catch (Exception ex)
                {
                    // Log the notification creation error but don't fail the whole operation
                    System.Diagnostics.Debug.WriteLine($"Error creating notification: {ex.Message}");
                }
            }
            
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order has been successfully assigned to you.";
            return RedirectToAction(nameof(Assigned));
        }

        // GET: Staff/Orders/ReadyForDelivery
        public async Task<IActionResult> ReadyForDelivery()
        {
            var completedOrders = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .Where(bd => bd.Status == "Completed" && bd.Booking.Status == BookingStatus.Completed)
                .OrderByDescending(bd => bd.CompletedOn)
                .ToListAsync();

            return View(completedOrders);
        }
    }
} 