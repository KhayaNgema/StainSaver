using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Admin.Models;
using StainSaver.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> Customers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Customer");
            return View(users.ToList());
        }

        public async Task<IActionResult> Staff()
        {
            var users = await _userManager.GetUsersInRoleAsync("Staff");
            return View(users.ToList());
        }

        public async Task<IActionResult> Drivers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Driver");
            return View(users.ToList());
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View(new CreateUserViewModel { Roles = roles });
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FullName = model.FullName,
                    IdNumber = model.IdNumber,
                    StreetAddress = model.StreetAddress,
                    Suburb = model.Suburb,
                    City = model.City,
                    Province = model.Province,
                    PostalCode = model.PostalCode,
                    PhoneNumber = model.PhoneNumber,
                    AlternativeContactNumber = model.AlternativeContactNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            // If we got this far, something failed, redisplay form
            model.Roles = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View(model);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IdNumber = user.IdNumber,
                StreetAddress = user.StreetAddress,
                Suburb = user.Suburb,
                City = user.City,
                Province = user.Province,
                PostalCode = user.PostalCode,
                PhoneNumber = user.PhoneNumber,
                AlternativeContactNumber = user.AlternativeContactNumber,
                Role = userRoles.FirstOrDefault(),
                Roles = roles
            };

            return View(model);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.FullName = model.FullName;
                user.IdNumber = model.IdNumber;
                user.StreetAddress = model.StreetAddress;
                user.Suburb = model.Suburb;
                user.City = model.City;
                user.Province = model.Province;
                user.PostalCode = model.PostalCode;
                user.PhoneNumber = model.PhoneNumber;
                user.AlternativeContactNumber = model.AlternativeContactNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Update role if changed
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var currentRole = userRoles.FirstOrDefault();
                    
                    if (currentRole != model.Role)
                    {
                        if (currentRole != null)
                        {
                            await _userManager.RemoveFromRoleAsync(user, currentRole);
                        }
                        
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    // Update password if provided
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(user);
                        var passwordResult = await _userManager.AddPasswordAsync(user, model.Password);
                        if (!passwordResult.Succeeded)
                        {
                            foreach (var error in passwordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            
                            // If we got this far, something failed with the password, redisplay form
                            model.Roles = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
                            return View(model);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            // If we got this far, something failed, redisplay form
            model.Roles = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View(model);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            return View(user);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.Role = userRoles.FirstOrDefault();
            return View(user);
        }
    }
} 