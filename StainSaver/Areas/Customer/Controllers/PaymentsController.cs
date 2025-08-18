using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Data;
using StainSaver.Models;
using System.Security.Claims;

namespace StainSaver.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Payments/Pending
        public IActionResult Pending()
        {
            // Redirect to the Booking Index where all bookings including ones with pending payments are shown
            return RedirectToAction("Index", "Booking", new { area = "Customer" });
        }

        // GET: Customer/Payments/Methods
        public IActionResult Methods()
        {
            // Redirect to the Booking Index 
            return RedirectToAction("Index", "Booking", new { area = "Customer" });
        }
    }
} 