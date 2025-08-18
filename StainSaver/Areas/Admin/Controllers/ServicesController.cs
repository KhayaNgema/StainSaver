using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Data;
using StainSaver.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Services
        public async Task<IActionResult> Index()
        {
            var services = await _context.LaundryServices.ToListAsync();
            return View(services);
        }

        // GET: Admin/Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.LaundryServices
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Admin/Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.LaundryServices.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            
            return View(service);
        }

        // POST: Admin/Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LaundryService service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: Admin/Services/ToggleStatus/5
        public async Task<IActionResult> ToggleStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.LaundryServices.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            service.IsActive = !service.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _context.LaundryServices.Any(e => e.Id == id);
        }
    }
} 