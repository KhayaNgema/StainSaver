using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Admin.Models;
using StainSaver.Models;
using System.Threading.Tasks;

namespace StainSaver.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new UsersViewModel();
            
            var roles = await _roleManager.Roles.ToListAsync();
            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                model.Roles.Add(new RoleViewModel 
                { 
                    RoleName = role.Name,
                    Users = usersInRole.ToList()
                });
            }
            
            return View(model);
        }

        // GET: Users/GetUsersInRole
        [HttpGet]
        public async Task<IActionResult> GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return BadRequest("Role name cannot be empty");

            // Get all users in the role
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            // Project to ViewModel
            var users = usersInRole.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.FullName,
                u.PhoneNumber
            });

            return Json(users);
        }
        
        // GET: Customer/Orders/Create - Redirect to Booking/Create
        [Route("Customer/Orders/Create")]
        public IActionResult RedirectToBookingCreate()
        {
            return RedirectToAction("Create", "Booking", new { area = "Customer" });
        }
    }
} 