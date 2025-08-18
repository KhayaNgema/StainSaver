using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Data;
using StainSaver.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LaundryServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LaundryServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LaundryServices
        public async Task<IActionResult> Index()
        {
            var services = await _context.LaundryServices.ToListAsync();
            return View(services);
        }

        // GET: Admin/LaundryServices/Details/5
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

        // GET: Admin/LaundryServices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LaundryServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Size,Price,IsActive,IsPremium")] LaundryService service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: Admin/LaundryServices/Edit/5
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

        // POST: Admin/LaundryServices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Size,Price,IsActive,IsPremium")] LaundryService service)
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
                    if (!LaundryServiceExists(service.Id))
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

        // GET: Admin/LaundryServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Admin/LaundryServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.LaundryServices.FindAsync(id);
            if (service != null)
            {
                _context.LaundryServices.Remove(service);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/LaundryServices/UpdatePrices
        public async Task<IActionResult> UpdatePrices()
        {
            var services = await _context.LaundryServices.ToListAsync();
            return View(services);
        }

        // POST: Admin/LaundryServices/UpdatePrice/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrice(int id, decimal price)
        {
            var service = await _context.LaundryServices.FindAsync(id);
            
            if (service == null)
            {
                return NotFound();
            }
            
            service.Price = price;
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(UpdatePrices));
        }

        private bool LaundryServiceExists(int id)
        {
            return _context.LaundryServices.Any(e => e.Id == id);
        }
    }
} 