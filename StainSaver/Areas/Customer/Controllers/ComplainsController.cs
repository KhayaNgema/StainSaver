using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using StainSaver.Areas.Customer.Models;
using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
namespace StainSaver.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class ComplainsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FileUploadService _fileUploadService;
        public ComplainsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            FileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult SuccessfullyCollectedItems()
        {
            return View();
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var complains = await _context.Complains
                .Include(c => c.RefundItems)
                .Include(c => c.LostOrFoundItems)
                .Where(c => c.CustomerId == user.Id &&
                c.Status == ComplainStatus.Review ||
                c.Status == ComplainStatus.DriverAssigned ||
                c.Status == ComplainStatus.AwaitingCustomer ||
                c.Status == ComplainStatus.Approved)
                .ToListAsync();
            return View(complains);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var complain = await _context.Complains
                .Include(c => c.Customer)
                .Include(c => c.RefundItems)
                .Include(c => c.LostOrFoundItems)
                .FirstOrDefaultAsync(c => c.ComplainId == id);
            if (complain == null)
            {
                return NotFound();
            }
            return View(complain);
        }

       

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CollectDelivery(int packageId)
        {
            var package = await _context.Packages
                .Where(p => p.PackageId == packageId)
                .Include(p => p.Complain)
                .Include(p => p.Complain)
                .FirstOrDefaultAsync();

            var complain = package.Complain;

            var delivery = await _context.Deliveries
                .Where(p => p.ComplainId == complain.ComplainId)
                .FirstOrDefaultAsync();

            complain.Status = ComplainStatus.Completed;
            delivery.Status = DeliveryStatus.Delivered;

            _context.Update(complain);
            _context.Update(delivery);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SuccessfullyCollectedItems));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReceiveItems(int packageId)
        {
            var package = await _context.Packages
                .Where(p => p.PackageId == packageId)
                .Include(p => p.Complain)
                .FirstOrDefaultAsync();

            var complain = package.Complain;

            var packageItems = await _context.DeliveryItems
                .Where(p => p.ComplainId == package.ComplainId)
                .Include(p => p.LostOrFoundItem)
                .ToListAsync();

            ViewBag.PackageId = packageId;
            ViewBag.ComaplainReference = complain.ReferenceNumber;
            ViewBag.PackageReference = package.ReferenceNumber;

            return View(packageItems);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ScanPackage(int packageId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var package = await _context.Packages
                .Include(p => p.Complain)
                .ThenInclude(p => p.Customer)
                .Where(p => p.PackageId == packageId && p.Complain.Customer.Id == user.Id)
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return RedirectToAction("Home", "Error");
            }

            var viewModel = new ScanPackageViewModel
            {
                PackageId = package.PackageId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ItemFound(int itemId)
        {
            var item = await _context.DeliveryItems
                .Where(i => i.DeliveryItemId == itemId)
                .FirstOrDefaultAsync();

            item.IsCollected = true;

            _context.Update(item);
            await _context.SaveChangesAsync();

            return Ok(new { success = true});
        }

        [HttpPost]
        public async Task<IActionResult> ItemMissing(int itemId)
        {
            var item = await _context.DeliveryItems
                .Where(i => i.DeliveryItemId == itemId)
                .FirstOrDefaultAsync();

            item.IsMissing = true;

            _context.Update(item);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VerifyScannedPackage(int packageId, string scannedText)
        {
            if (string.IsNullOrEmpty(scannedText))
                return Json(new { success = false, message = "Invalid scanned text." });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not authenticated." });

            var package = await _context.Packages
                    .Include(d => d.Complain)
                        .ThenInclude(c => c.Customer)
                .FirstOrDefaultAsync(p => p.ReferenceNumber == scannedText);

            if (package == null)
                return Json(new { success = false, message = "Package not found." });

            if (package.PackageId != packageId)
                return Json(new { success = false, message = "The scanned code does not match the expected package." });

            if (package.Complain?.Customer == null)
                return Json(new { success = false, message = "Associated customer information not found." });

            if (package.Complain.Customer.Id != user.Id)
                return Json(new { success = false, message = "You do not have permission to receive this package." });

            if (package.Complain.Status == ComplainStatus.Completed)
                return Json(new { success = false, message = "This package was already delivered." });


            var redirectUrl = Url.Action(nameof(ReceiveItems), new { packageId = packageId });

            return Json(new { success = true, redirectUrl });
        }



        [HttpGet]
        public async Task<IActionResult> Complain(int bookingId)
        {
            var viewModel = new ComplainViewModel { BookingId = bookingId };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Complain(ComplainViewModel viewModel)
        {
            try
            {
                Console.WriteLine("Received complain POST:");
                Console.WriteLine($"BookingId: {viewModel.BookingId}");
                Console.WriteLine($"ComplainType: {viewModel.ComplainType}");
                Console.WriteLine($"RefundItems count: {(viewModel.RefundItems?.Count ?? 0)}");
                Console.WriteLine($"LostOrFoundItems count: {(viewModel.LostOrFoundItems?.Count ?? 0)}");

                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == viewModel.BookingId);
                if (booking == null)
                {
                    ModelState.AddModelError("", "Invalid booking selected for complain.");
                    Console.WriteLine("Booking not found.");
                    return View(viewModel);
                }

                var user = await _userManager.GetUserAsync(User);
                string referenceNumber = (viewModel.ComplainType == ComplainType.Refund)
                    ? GenerateRefundComplainReferenceNumber()
                    : GenerateLostOrFoundComplainReferenceNumber();

                var complain = new Complain
                {
                    ReferenceNumber = referenceNumber,
                    CustomerId = user.Id,
                    ComplainType = viewModel.ComplainType,
                    BankAccountNumber = viewModel.BankAccountNumber,
                    BankAccountType = viewModel.BankAccountType,
                    Bank = viewModel.Bank,
                    Status = ComplainStatus.Review,
                    ReasonForRefund = viewModel.ReasonForRefund,
                    BookingId = booking.Id,
                    Comments = new List<string>(),
                    Description = (viewModel.ComplainType == ComplainType.Refund) ? viewModel.Description : viewModel.LostFoundDescription,
                    IsFound = viewModel.IsFound,
                    IsLost = viewModel.IsLost,
                };

                if (viewModel.ProofOfPayment != null && viewModel.ProofOfPayment.Length > 0)
                {
                    complain.ProofOfPayment = await _fileUploadService.UploadFileAsync(viewModel.ProofOfPayment);
                    Console.WriteLine($"Uploaded ProofOfPayment: {complain.ProofOfPayment}");
                }

                _context.Complains.Add(complain);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Saved complain with ID: {complain.ComplainId}");

                if (viewModel.ComplainType == ComplainType.Refund && viewModel.RefundItems != null && viewModel.RefundItems.Any())
                {
                    foreach (var itemVm in viewModel.RefundItems)
                    {
                        string imageUrl = string.Empty;
                        if (itemVm.ImageFile != null && itemVm.ImageFile.Length > 0)
                        {
                            imageUrl = await _fileUploadService.UploadFileAsync(itemVm.ImageFile);
                            Console.WriteLine($"Uploaded RefundItem image: {imageUrl}");
                        }
                        else
                        {
                            Console.WriteLine($"No image uploaded for RefundItem: {itemVm.RefundItemName}");
                        }
                        var refundItem = new RefundItem
                        {
                            RefundItemName = itemVm.RefundItemName,
                            ImageUrl = imageUrl,
                            ComplainId = complain.ComplainId
                        };
                        _context.RefundItems.Add(refundItem);
                    }
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Saved all RefundItems");
                }
                else
                {
                    Console.WriteLine("Not saving RefundItems due to complain type or empty list");
                }

                if (viewModel.ComplainType == ComplainType.Lost_and_found && viewModel.LostOrFoundItems != null && viewModel.LostOrFoundItems.Any())
                {
                    foreach (var itemVm in viewModel.LostOrFoundItems)
                    {
                        string imageUrl = string.Empty;
                        if (itemVm.ImageFile != null && itemVm.ImageFile.Length > 0)
                        {
                            imageUrl = await _fileUploadService.UploadFileAsync(itemVm.ImageFile);
                            Console.WriteLine($"Uploaded LostOrFoundItem image: {imageUrl}");
                        }
                        else
                        {
                            Console.WriteLine($"No image uploaded for LostOrFoundItem: {itemVm.ItemDescription}");
                        }
                        var lostOrFoundItem = new LostOrFoundItem
                        {
                            ItemDescription = itemVm.ItemDescription,
                            ImageUrl = imageUrl,
                            ComplainId = complain.ComplainId
                        };
                        _context.LostOrFoundItems.Add(lostOrFoundItem);
                    }
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Saved all LostOrFoundItems");
                }
                else
                {
                    Console.WriteLine("Not saving LostOrFoundItems due to complain type or empty list");
                }

                TempData["Message"] = "You have successfully submitted your complain. This complain is under review.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Complain POST: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new
                {
                    success = false,
                    message = "Failed to submit complain: " + ex.Message,
                    errorDetails = new
                    {
                        ex.InnerException?.Message,
                        ex.StackTrace
                    }
                });
            }
        }


        private string GenerateRefundComplainReferenceNumber()
        {
            var now = DateTime.Now;
            const string prefix = "REFU";
            string datePart = now.ToString("yyMMdd");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Range(0, 4)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
            return $"{datePart}{randomPart}{prefix}";
        }

        private string GenerateLostOrFoundComplainReferenceNumber()
        {
            var now = DateTime.Now;
            const string prefix = "LFND";
            string datePart = now.ToString("yyMMdd");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
            return $"{prefix}{datePart}{randomPart}";
        }
    }
}