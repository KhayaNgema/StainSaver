using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StainSaver.Models;

namespace StainSaver.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Customer"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Staff"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Staff" });
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Driver"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Driver" });
                    }
                }
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
