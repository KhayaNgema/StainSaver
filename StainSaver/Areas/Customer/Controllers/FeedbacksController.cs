using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StainSaver.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FileUploadService _fileUploadService;

        public FeedbacksController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            FileUploadService fileUploadService)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> ThankYou()
        {
            return View();
        }
    }
}
