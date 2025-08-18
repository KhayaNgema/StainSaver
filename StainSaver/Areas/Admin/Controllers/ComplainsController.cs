using Hangfire;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Admin.Models;
using StainSaver.Areas.Customer.Models;
using StainSaver.Areas.Driver.Models;
using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
namespace StainSaver.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ComplainsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FileUploadService _fileUploadService;
        private readonly BarcodeService _barcodeService;
        private readonly SendRefundSms _sendRefundSms;
        public ComplainsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            FileUploadService fileUploadService,
            BarcodeService barcodeService,
            SendRefundSms sendRefundSms)
        {
            _fileUploadService = fileUploadService;
            _context = context;
            _userManager = userManager;
            _barcodeService = barcodeService;
            _sendRefundSms = sendRefundSms;
        }
        [HttpGet]
        public IActionResult SuccesfullyApproved(int id)
        {
            ViewData["ComplainId"] = id;
            return View();
        }

        [HttpGet]
        public IActionResult SuccessfullyProcessed()
        {
            return View();
        }

        

        [HttpGet]
        public IActionResult SuccesfullyAssignedDriver(int id)
        {
            ViewData["ComplainId"] = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PackagedDeliveries()
        {
            var packages = await _context.Packages
                .Include(r => r.Complain)
                    .ThenInclude(c => c.Customer)
                .Include(r => r.Driver)
                .ToListAsync();
            return View(packages);
        }

        [HttpGet]
        public async Task<IActionResult> PackageDetails(int id)
        {

            var package = await _context.Packages
                .Include(p => p.Complain)
                .ThenInclude(p => p.Customer)
                .Include(p => p.Driver)
                .Where(p => p.PackageId == id)
                .FirstOrDefaultAsync();

            return View(package);
        }


        [HttpGet]
        public async Task<IActionResult> ProcessedRefunds()
        {
            var refunds = await _context.Refunds
                .Include(r => r.Complain)
                    .ThenInclude(c => c.Customer)
                .Where(r => r.Status == RefundStatus.Refunded)
                .ToListAsync();
            return View(refunds);
        }

        [HttpGet]
        public async Task<IActionResult> RefundComplains()
        {
            var complains = await _context.Complains
                                 .Where(c => c.ComplainType == ComplainType.Refund &&
                                    (c.Status == ComplainStatus.AwaitingCustomer ||
                                     c.Status == ComplainStatus.DriverAssigned ||
                                     c.Status == ComplainStatus.Approved ||
                                     c.Status == ComplainStatus.Review))
                                 .Include(c => c.Customer)
                                 .Include(c => c.Booking)
                                 .ToListAsync();

            return View(complains);
        }

        [HttpGet]
        public async Task<IActionResult> LostOrFoundComplains()
        {
            var complains = await _context.Complains
                                 .Where(c => c.ComplainType == ComplainType.Lost_and_found &&
                                    (c.Status == ComplainStatus.AwaitingCustomer ||
                                     c.Status == ComplainStatus.DriverAssigned ||
                                     c.Status == ComplainStatus.Approved ||
                                     c.Status == ComplainStatus.Review))
                                  .Include(c => c.Customer)
                                  .Include(c => c.Booking)
                                 .ToListAsync();
            return View(complains);
        }
        [HttpGet]
        public async Task<IActionResult> RefundDetails(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var refund = await _context.Refunds
                .Include(r => r.Complain)
                    .ThenInclude(c => c.Customer)
                .Include(r => r.RefundPolicyEntries)
                .Include(r => r.RefundValidationEntries)
                .FirstOrDefaultAsync(r => r.RefundId == id);
            if (refund == null)
            {
                return NotFound();
            }
            return View(refund);
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

        [HttpGet]
        public async Task<IActionResult> ConfirmItem(int pickupId)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ProcessRefund(int complainId)
        {
            var complain = await _context.Complains
                .Where(c => c.ComplainId == complainId)
                .Include(c => c.Customer)
                .Include(c => c.RefundItems)
                 .Include(c => c.LostOrFoundItems)
                .Include(c => c.Booking)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(b => b.LaundryService)
                .FirstOrDefaultAsync();
            if (complain == null)
            {
                return NotFound();
            }

            var refund = await _context.Refunds
                .Where(r => r.ComplainId == complainId)
                .FirstOrDefaultAsync();

            var items = complain.RefundItems?.Select(r => new RefundDisplayItemViewModel
            {
                RefundItemName = r.RefundItemName,
                ImageFile = r.ImageUrl
            }).ToList();

            var refundItem = complain.RefundItems?.FirstOrDefault();

            var model = new ProcessRefundViewModel
            {
                ComplainId = complain.ComplainId,
                RefundValidations = refund?.RefundValidationEntries
                    .Select(rv => rv.RefundValidation)
                    .ToList() ?? new List<RefundValidation>(),
                RefundPolicies = refund?.RefundPolicyEntries
                    .Select(rp => rp.RefundPolicy)
                    .ToList() ?? new List<RefundPolicy>(),
                CouponBonus = refund?.CouponBonus ?? 0m,
                RefundItems = items ?? new List<RefundDisplayItemViewModel>()
            };


            ViewData["Complain"] = complain;
            ViewData["Booking"] = complain.Booking;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessRefund(ProcessRefundViewModel model)
        {
            try
            {
                var complainFromDb = await _context.Complains
                    .Include(c => c.Booking)
                    .FirstOrDefaultAsync(c => c.ComplainId == model.ComplainId);
                if (complainFromDb == null)
                {
                    ModelState.AddModelError("", "Complain not found.");
                    return View(model);
                }
                var booking = complainFromDb.Booking;
                decimal bookingTotal = booking?.TotalAmount ?? 0m;
                decimal totalDeductionPercent = 0m;
                if (model.RefundPolicies != null)
                {
                    foreach (var policy in model.RefundPolicies)
                    {
                        if (RefundPolicyData.RefundPolicyPercentages.TryGetValue(policy, out decimal pct))
                            totalDeductionPercent += pct;
                    }
                    if (totalDeductionPercent > 1m) totalDeductionPercent = 1m;
                }
                decimal policyDeductionAmount = bookingTotal * totalDeductionPercent;
                decimal calculatedRefund = bookingTotal - policyDeductionAmount - model.CouponBonus;
                if (calculatedRefund < 0) calculatedRefund = 0;
                var refund = new Refund
                {
                    ComplainId = model.ComplainId,
                    CouponBonus = model.CouponBonus,
                    RefundedAmount = calculatedRefund,
                    Status = RefundStatus.Refunded,
                    RefundPolicyEntries = (model.RefundPolicies ?? new List<RefundPolicy>())
                        .Select(policy => new RefundPolicyEntry { RefundPolicy = policy })
                        .ToList(),
                    RefundValidationEntries = (model.RefundValidations ?? new List<RefundValidation>())
                        .Select(validation => new RefundValidationEntry { RefundValidation = validation })
                        .ToList()
                };
                complainFromDb.Status = ComplainStatus.Refunded;

                _context.Update(complainFromDb);
                _context.Refunds.Add(refund);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Refund processed successfully.";

                BackgroundJob.Enqueue(() =>_sendRefundSms.NotifyRefundCustomerContactAsync(model.ComplainId));

                return RedirectToAction(nameof(SuccessfullyProcessed));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to process refund: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComments(int complainId, string comments)
        {
            if (complainId <= 0)
            {
                return BadRequest("Invalid complain ID.");
            }
            var complain = await _context.Complains.FindAsync(complainId);
            if (complain == null)
            {
                return NotFound();
            }
            if (complain.Comments == null)
            {
                complain.Comments = new List<string?>();
            }
            if (!string.IsNullOrWhiteSpace(comments))
            {
                complain.Comments.Add(comments);
            }
            try
            {
                _context.Update(complain);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Comments updated successfully.";
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating comments.");
                return View("Details", complain);
            }
            return RedirectToAction("Details", new { id = complainId });
        }
        private string GeneratePickUpReferenceNumber()
        {
            var now = DateTime.Now;
            const string prefix = "PCK";
            string datePart = now.ToString("yyMMdd");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Range(0, 4)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
            return $"{datePart}{randomPart}{prefix}";
        }
        [HttpGet]
        public async Task<IActionResult> SchedulePickUp(int id)
        {
            var complain = await _context.Complains
                .Where(c => c.ComplainId == id)
                .FirstOrDefaultAsync();
            if (complain == null)
            {
                return NotFound();
            }
            var driversList = await _userManager.GetUsersInRoleAsync("Driver");
            var driverSelectList = driversList
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = d.Id,
                    Text = d.FullName
                })
                .ToList();
            ViewBag.Drivers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(driverSelectList, "Value", "Text");
            var viewModel = new SchedulePickUpViewModel
            {
                ComplainId = id,
                ComplainType = complain.ComplainType,
                PickUpDate = DateTime.Today
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SchedulePickUp(SchedulePickUpViewModel viewModel)
        {
            try
            {
                var complain = await _context.Complains
                    .Where(c => c.ComplainId == viewModel.ComplainId)
                    .Include(c => c.Customer)
                    .FirstOrDefaultAsync();
                if (complain == null)
                {
                    return NotFound();
                }
                var referenceNumber = GeneratePickUpReferenceNumber();
                var pickUp = new PickUp
                {
                    ReferenceNumber = referenceNumber,
                    ComplainId = complain.ComplainId,
                    DriverId = viewModel.DriverId,
                    IsPickedUp = false,
                    PickUpDate = viewModel.PickUpDate,
                    Status = PickUpStatus.DriverAssigned
                    
                };
                complain.Status = ComplainStatus.DriverAssigned;
                _context.Update(complain);
                _context.Add(pickUp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SuccesfullyAssignedDriver), new { id = viewModel.ComplainId });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to submit complain: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        ex.StackTrace
                    }
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> PackageItems(int complainId)
        {
            var complain = await _context.Complains.FirstOrDefaultAsync(c => c.ComplainId == complainId);
            if (complain == null)
                return NotFound();

            // Get all LostOrFoundItems related to complain
            var lostOrFoundItems = await _context.LostOrFoundItems
                .Where(l => l.ComplainId == complainId)
                .ToListAsync();

            var deliveryItems = await _context.DeliveryItems
                .Where(di => di.ComplainId == complainId)
                .ToListAsync();

            var viewModel = new PackageItemsViewModel
            {
                ComplainId = complainId,

                Items = lostOrFoundItems.Select(l =>
                {
                    var deliveryItem = deliveryItems.FirstOrDefault(di => di.LostOrFoundItemId == l.LostOrFoundItemId);
                    return new LostOrFoundDisplayItemViewModel
                    {
                        LostOrFoundItemId = l.LostOrFoundItemId,
                        ItemDescription = l.ItemDescription,
                        ImageFile = l.ImageUrl,
                        IsPackaged = deliveryItem?.IsPackaged ?? false  
                    };
                }).ToList()
            };

            return PartialView("_PackageDeliveryPartial", viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> PackageItem(int complainId, int deliveryItemId)
        {
            var lostOrFoundItem = await _context.LostOrFoundItems
                .Where(d => d.LostOrFoundItemId == deliveryItemId)
                .Include(d => d.Complain)
                .FirstOrDefaultAsync();

            if (lostOrFoundItem == null)
            {
                return Json(new { success = false, message = "Delivery item not found." });
            }

            var deliveryItem = new DeliveryItem
            {
                PackagedAt = DateTime.Now,
                IsPackaged = true,
                LostOrFoundItemId = deliveryItemId,
                IsCollected = false,
                IsMissing = false,
                ComplainId = lostOrFoundItem.Complain.ComplainId
            };

            _context.Add(deliveryItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> CompletePackaging(int complainId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var barcodeValue = new Random().Next(10000000, 99999999).ToString();
                string barcodeFileName = $"{barcodeValue}.png";

                var (barcodePath, imageBytes) = await _barcodeService.GenerateAndSaveBarcodeAsync(barcodeValue, barcodeFileName);

                var complain = await _context.Complains.FirstOrDefaultAsync(c => c.ComplainId == complainId);
                if (complain == null) return NotFound();

                var package = new Package
                {
                    CreatedAt = DateTime.Now,
                    BarcodeImage = imageBytes,       
                    ReferenceNumber = barcodeValue,
                    ComplainId = complain.ComplainId,
                    CreatedById = user.Id,
                    DriverId = null
                };

                _context.Packages.Add(package);
                await _context.SaveChangesAsync();

                return Ok(new { packageId = package.PackageId, barcodePath });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add new medication: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }


        // GET: Show schedule delivery form
        [HttpGet]
        public async Task<IActionResult> ScheduleDelivery(int packageId)
        {
            var package = await _context.Packages
                .Include(p => p.Complain)
                .Where(p => p.PackageId == packageId)
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return NotFound();
            }

            var driversList = await _userManager.GetUsersInRoleAsync("Driver");
            var driverSelectList = driversList
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = d.Id,
                    Text = d.FullName
                })
                .ToList();

            ViewBag.Drivers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(driverSelectList, "Value", "Text");

            var viewModel = new ScheduleDeliveryViewModel
            {
                PackageId = package.PackageId,
                ComplainId = package.Complain?.ComplainId ?? 0, 
                ReferenceNumber = package.ReferenceNumber,
                Barcode = package.BarcodeImage,
                DeliveryDate = DateTime.Today
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleDelivery(ScheduleDeliveryViewModel viewModel)
        {

            try
            {
                var complain = await _context.Complains
                    .Where(c => c.ComplainId == viewModel.ComplainId)
                    .Include(c => c.Customer)
                    .FirstOrDefaultAsync();

                if (complain == null)
                {
                    return NotFound();
                }

                var package = await _context.Packages.FirstOrDefaultAsync(p => p.PackageId == viewModel.PackageId);


                if (package == null)
                {
                    return NotFound();
                }

                // Update the package's DriverId
                package.DriverId = viewModel.DriverId;
                _context.Packages.Update(package);

                var delivery = new Delivery
                {
                    ComplainId = complain.ComplainId,
                    DriverId = viewModel.DriverId,
                    IsDelivered = false,
                    DeliveryDate = viewModel.DeliveryDate,
                    Status = DeliveryStatus.DriverAssigned,
                    PackageId = package.PackageId
                };

                complain.Status = ComplainStatus.DriverAssigned;

                _context.Complains.Update(complain);
                _context.Deliveries.Add(delivery);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(SuccesfullyAssignedDriver), new { id = viewModel.ComplainId });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = false,
                    message = "Failed to submit complain: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveComplain(int complainId)
        {
            if (complainId <= 0)
            {
                return BadRequest("Invalid complain ID.");
            }
            var complain = await _context.Complains.FindAsync(complainId);
            if (complain == null)
            {
                return NotFound();
            }
            complain.Status = ComplainStatus.Approved;
            try
            {
                _context.Update(complain);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while approving the complain.");
                return View("Details", complain);
            }
            return RedirectToAction("SuccesfullyApproved", new { id = complainId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectComplain(int complainId)
        {
            if (complainId <= 0)
            {
                return BadRequest("Invalid complain ID.");
            }
            var complain = await _context.Complains.FindAsync(complainId);
            if (complain == null)
            {
                return NotFound();
            }
            complain.Status = ComplainStatus.Rejected;
            try
            {
                _context.Update(complain);
                await _context.SaveChangesAsync();
                TempData["Message"] = "You have rejected this complaint. Notifications have been sent to the relevant parties.";
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while rejecting the complain.");
                return View("Details", complain);
            }
            return RedirectToAction("Details", new { id = complainId });
        }
    }
}
