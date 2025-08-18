using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Admin.Models;
using StainSaver.Data;
using StainSaver.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Driver)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Admin/Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Driver)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.LaundryService)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Admin/Bookings/AssignDriver/5
        public async Task<IActionResult> AssignDriver(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Get all drivers
            var drivers = await _userManager.GetUsersInRoleAsync("Driver");

            var model = new AssignDriverViewModel
            {
                BookingId = booking.Id,
                Booking = booking,
                Drivers = new SelectList(drivers, "Id", "FullName")
            };

            return View(model);
        }

        // POST: Admin/Bookings/AssignDriver
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDriver(AssignDriverViewModel model)
        {
            if (string.IsNullOrEmpty(model.DriverId))
            {
                ModelState.AddModelError("DriverId", "Please select a driver");
                
                // Reload the form data
                var booking = await _context.Bookings
                    .Include(b => b.Customer)
                    .FirstOrDefaultAsync(b => b.Id == model.BookingId);
                
                if (booking == null)
                {
                    return NotFound();
                }
                
                var availableDrivers = await _userManager.GetUsersInRoleAsync("Driver");
                model.Booking = booking;
                model.Drivers = new SelectList(availableDrivers, "Id", "FullName");
                
                return View(model);
            }
            
            if (ModelState.IsValid)
            {
                var booking = await _context.Bookings
                    .Include(b => b.Customer)
                    .FirstOrDefaultAsync(b => b.Id == model.BookingId);
                    
                if (booking == null)
                {
                    return NotFound();
                }

                // Get driver details to include in notification
                var driver = await _userManager.FindByIdAsync(model.DriverId);
                
                if (driver == null)
                {
                    ModelState.AddModelError("DriverId", "Selected driver not found");
                    
                    // Reload drivers for dropdown
                    var driversList = await _userManager.GetUsersInRoleAsync("Driver");
                    model.Booking = booking;
                    model.Drivers = new SelectList(driversList, "Id", "FullName");
                    
                    return View(model);
                }
                
                booking.DriverId = model.DriverId;
                booking.Status = BookingStatus.PickupAssigned;

                _context.Update(booking);
                
                // No need to create notification if there's a database issue
                bool notificationCreated = false;
                
                try
                {
                    // First try updating just the booking without the notification
                    await _context.SaveChangesAsync();
                    
                    // If that succeeds, try to add the notification separately
                    try
                    {
                        // Try accessing the DbSet to see if the table exists
                        bool tableExists = true;
                        try
                        {
                            // This will throw an exception if the table doesn't exist
                            var testQuery = _context.CustomerNotifications.FirstOrDefault();
                        }
                        catch (Exception)
                        {
                            tableExists = false;
                            System.Diagnostics.Debug.WriteLine("CustomerNotifications table doesn't exist - skipping notification creation");
                        }

                        if (tableExists)
                        {
                            var notification = new CustomerNotification
                            {
                                CustomerId = booking.CustomerId,
                                Title = "Driver Assigned",
                                Message = $"A driver has been assigned to your booking #{booking.Id}. Your laundry will be picked up on {booking.PickupDate?.ToString("d MMMM yyyy") ?? "the scheduled date"}.",
                                BookingId = booking.Id,
                                CreatedDate = DateTime.Now,
                                IsRead = false
                            };
                            
                            _context.CustomerNotifications.Add(notification);
                            await _context.SaveChangesAsync();
                            notificationCreated = true;
                        }
                    }
                    catch (Exception notificationEx)
                    {
                        // Log the notification creation error but don't fail the whole operation
                        System.Diagnostics.Debug.WriteLine($"Error creating notification: {notificationEx.Message}");
                        // Continue without the notification
                    }
                    
                    TempData["SuccessMessage"] = notificationCreated 
                        ? "Driver assigned successfully and customer notified." 
                        : "Driver assigned successfully but notification could not be created.";
                        
                    return RedirectToAction(nameof(Details), new { id = model.BookingId });
                }
                catch (DbUpdateException dbEx)
                {
                    // Get detailed error information
                    var innerException = dbEx.InnerException;
                    var errorMessage = "Database error: " + dbEx.Message;
                    
                    if (innerException != null)
                    {
                        errorMessage += " Inner exception: " + innerException.Message;
                        
                        // Log the full exception details
                        System.Diagnostics.Debug.WriteLine("=== Database Error Details ===");
                        System.Diagnostics.Debug.WriteLine($"Error Message: {dbEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {innerException.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack Trace: {innerException.StackTrace}");
                    }
                    
                    // Try a different approach - just update the status
                    try
                    {
                        // Clear the context to avoid tracking conflicts
                        _context.Entry(booking).State = EntityState.Detached;
                        
                        // Load a fresh instance
                        var freshBooking = await _context.Bookings.FindAsync(model.BookingId);
                        if (freshBooking != null)
                        {
                            freshBooking.Status = BookingStatus.PickupAssigned;
                            freshBooking.DriverId = model.DriverId;
                            await _context.SaveChangesAsync();
                            
                            TempData["SuccessMessage"] = "Driver assigned successfully (minimal update).";
                            return RedirectToAction(nameof(Details), new { id = model.BookingId });
                        }
                    }
                    catch (Exception fallbackEx)
                    {
                        errorMessage += " Fallback also failed: " + fallbackEx.Message;
                        System.Diagnostics.Debug.WriteLine($"Fallback Error: {fallbackEx.Message}");
                    }
                    
                    // Show the detailed error to the user
                    ModelState.AddModelError("", errorMessage);
                    
                    // Reload form data
                    var driversForReload = await _userManager.GetUsersInRoleAsync("Driver");
                    model.Booking = booking;
                    model.Drivers = new SelectList(driversForReload, "Id", "FullName");
                    
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving changes: {ex.Message}");
                    
                    // Reload form data
                    var driversForReload = await _userManager.GetUsersInRoleAsync("Driver");
                    model.Booking = booking;
                    model.Drivers = new SelectList(driversForReload, "Id", "FullName");
                    
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            var booking2 = await _context.Bookings.Include(b => b.Customer).FirstOrDefaultAsync(b => b.Id == model.BookingId);
            var allDrivers = await _userManager.GetUsersInRoleAsync("Driver");
            model.Booking = booking2;
            model.Drivers = new SelectList(allDrivers, "Id", "FullName");

            return View(model);
        }

        // GET: Admin/Bookings/BookingDetailsForStaffAssignment
        public async Task<IActionResult> BookingDetailsForStaffAssignment()
        {
            var bookingDetails = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .Where(bd => bd.StaffId == null && bd.Status == "Pending" && 
                      (bd.Booking.Status == BookingStatus.PickedUp || bd.Booking.Status == BookingStatus.Processing))
                .OrderBy(bd => bd.Booking.PickupDate)
                .ToListAsync();

            return View(bookingDetails);
        }

        // GET: Admin/Bookings/AssignStaff/5
        public async Task<IActionResult> AssignStaff(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bookingDetail == null)
            {
                return NotFound();
            }

            // Get all staff members
            var staffMembers = await _userManager.GetUsersInRoleAsync("Staff");

            var model = new AssignStaffViewModel
            {
                BookingDetailId = bookingDetail.Id,
                BookingDetail = bookingDetail,
                StaffMembers = new SelectList(staffMembers, "Id", "FullName")
            };

            return View(model);
        }

        // POST: Admin/Bookings/AssignStaff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStaff(AssignStaffViewModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.StaffId))
            {
                var bookingDetail = await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                    .Include(bd => bd.LaundryService)
                    .FirstOrDefaultAsync(bd => bd.Id == model.BookingDetailId);

                if (bookingDetail == null)
                {
                    return NotFound();
                }

                var staffMember = await _userManager.FindByIdAsync(model.StaffId);
                if (staffMember == null)
                {
                    ModelState.AddModelError("StaffId", "Selected staff member not found");
                    
                    // Reload staff for dropdown
                    var staffList = await _userManager.GetUsersInRoleAsync("Staff");
                    model.BookingDetail = bookingDetail;
                    model.StaffMembers = new SelectList(staffList, "Id", "FullName");
                    
                    return View(model);
                }

                bookingDetail.StaffId = model.StaffId;
                bookingDetail.Status = "Assigned";

                _context.Update(bookingDetail);
                await _context.SaveChangesAsync();

                // Update booking status to StaffAssigned
                if (bookingDetail.Booking != null)
                {
                    bookingDetail.Booking.Status = BookingStatus.StaffAssigned;
                    _context.Update(bookingDetail.Booking);
                    await _context.SaveChangesAsync();
                    
                    // Create notification for customer
                    try
                    {
                        var notification = new CustomerNotification
                        {
                            CustomerId = bookingDetail.Booking.CustomerId,
                            Title = "Staff Assigned",
                            Message = $"A staff member has been assigned to process your {bookingDetail.LaundryService.Name} service for booking #{bookingDetail.Booking.Id}.",
                            BookingId = bookingDetail.Booking.Id,
                            CreatedDate = DateTime.Now,
                            IsRead = false
                        };
                        
                        _context.CustomerNotifications.Add(notification);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Staff assigned successfully and customer notified.";
                    }
                    catch (Exception ex)
                    {
                        // Log the notification creation error but don't fail the whole operation
                        System.Diagnostics.Debug.WriteLine($"Error creating notification: {ex.Message}");
                        TempData["SuccessMessage"] = "Staff assigned successfully but notification could not be created.";
                    }
                }

                return RedirectToAction(nameof(Details), new { id = bookingDetail.BookingId });
            }

            // If we got this far, something failed, redisplay form
            var bookingDetail2 = await _context.BookingDetails
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bd => bd.LaundryService)
                .FirstOrDefaultAsync(bd => bd.Id == model.BookingDetailId);

            var staffMembers = await _userManager.GetUsersInRoleAsync("Staff");
            model.BookingDetail = bookingDetail2;
            model.StaffMembers = new SelectList(staffMembers, "Id", "FullName");

            return View(model);
        }

        // GET: Admin/Bookings/UpdateStatus/5
        public async Task<IActionResult> UpdateStatus(int? id, BookingStatus status)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = booking.Id });
        }

        // GET: Admin/Bookings/Cancel/5
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Update booking status
            booking.Status = BookingStatus.Cancelled;
            _context.Update(booking);

            // Also update any pending booking details
            foreach (var detail in booking.BookingDetails)
            {
                if (detail.Status != "Completed")
                {
                    detail.Status = "Cancelled";
                    _context.Update(detail);
                }
            }

            // Create notification for customer
            var notification = new CustomerNotification
            {
                CustomerId = booking.CustomerId,
                Title = "Booking Cancelled",
                Message = $"Your booking #{booking.Id} has been cancelled by the administrator.",
                BookingId = booking.Id,
                CreatedDate = DateTime.Now,
                IsRead = false
            };
            
            _context.CustomerNotifications.Add(notification);
            
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking has been successfully cancelled.";
            return RedirectToAction(nameof(Details), new { id = booking.Id });
        }
    }
} 