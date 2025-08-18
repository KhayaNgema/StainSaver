using Hangfire;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StainSaver.Areas.Customer.Models;
using StainSaver.Areas.Driver.Models;
using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
namespace StainSaver.Areas.Driver.Controllers
{
    [Area("Driver")]
    [Authorize(Roles = "Driver")]
    public class ComplainsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FileUploadService _fileUploadService;
        private readonly PickUpSmsService _pickUpSmsService;
        private readonly ReceiveItemsService _receiveItemsService;
        public ComplainsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            FileUploadService fileUploadService,
            PickUpSmsService pickUpSmsService,
            ReceiveItemsService receiveItemsService)
        {
            _fileUploadService = fileUploadService;
            _context = context;
            _userManager = userManager;
            _pickUpSmsService = pickUpSmsService;
            _receiveItemsService = receiveItemsService;
        }


        [HttpGet]
        public async Task<IActionResult> LoadPickUpPartial(int pickUpId)
        {
            var pickUp = await _context.PickUps
                                      .Include(p => p.Complain)
                                          .ThenInclude(c => c.LostOrFoundItems)
                                      .FirstOrDefaultAsync(p => p.PickUpId == pickUpId);
            if (pickUp == null)
                return NotFound();

            var items = pickUp.Complain.LostOrFoundItems?.Select(l => new PickUpItemViewModel
            {
                ItemName = l.ItemDescription,
                ItemImage = l.ImageUrl
            }).ToList();

            var lostOrFoundItem = pickUp.Complain.LostOrFoundItems?.FirstOrDefault();

            var otp = GenerateOTP();

            pickUp.OTP = otp;

            _context.Update(pickUp);
            await _context.SaveChangesAsync();

            BackgroundJob.Enqueue(() => _pickUpSmsService.NotifyPickUpFromCustomerContactAsync(pickUpId));

            var model = new PickUpConfirmViewModel
            {
                PickUpId = pickUp.PickUpId,
                ItemDescription = lostOrFoundItem?.ItemDescription,
                ItemImage = lostOrFoundItem?.ImageUrl,
                Items = items ?? new List<PickUpItemViewModel>()
            };

            return PartialView("_PickUpPartial", model);
        }

        public async Task<IActionResult> Deliveries()
        {
            var user = await _userManager.GetUserAsync(User);

            var deliveries = await _context.Deliveries
                .Where(pu => pu.DriverId == user.Id)
                .Include(pu => pu.Complain)
                    .ThenInclude(c => c.Customer)
                .Include(pu => pu.Complain)
                    .ThenInclude(c => c.LostOrFoundItems)
                .Include(pu => pu.Package)
                .ToListAsync();

            return View(deliveries);
        }

        public async Task<IActionResult> PickUps()
        {
            var user = await _userManager.GetUserAsync(User);
            var pickUps = await _context.PickUps
                .Where(pu => pu.DriverId == user.Id)
                .Include(pu => pu.Complain)
                    .ThenInclude(c => c.Customer)
                .Include(pu => pu.Complain)
                    .ThenInclude(c => c.LostOrFoundItems)
                .ToListAsync();
            return View(pickUps);
        }

        [HttpGet]
        public async Task<IActionResult> Complain()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Complain(ComplainViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var complain = new Complain
                {
                    CustomerId = user.Id,
                    ComplainType = viewModel.ComplainType,
                    BankAccountNumber = viewModel.BankAccountNumber,
                    BankAccountType = viewModel.BankAccountType,
                    IsFound = viewModel.IsFound,
                    IsLost = viewModel.IsLost,
                    LostOrFoundDate = viewModel.LostOrFoundDate,
                    Bank = viewModel.Bank,
                    Comments = null,
                    Status = ComplainStatus.Review,
                    ReasonForRefund = viewModel.ReasonForRefund
                };
                if (viewModel.ComplainType == ComplainType.Refund)
                {
                    complain.Description = viewModel.Description;
                }
                else if (viewModel.ComplainType == ComplainType.Lost_and_found)
                {
                    complain.Description = viewModel.LostFoundDescription;
                }
                if (viewModel.ProofOfPayment != null && viewModel.ProofOfPayment.Length > 0)
                {
                    var proofOfPaymentPath = await _fileUploadService.UploadFileAsync(viewModel.ProofOfPayment);
                    complain.ProofOfPayment = proofOfPaymentPath;
                }
                if (viewModel.LostOrFoundItems != null && viewModel.LostOrFoundItems.Count > 0)
                {
                    complain.LostOrFoundItems = new List<LostOrFoundItem>();

                    foreach (var item in viewModel.LostOrFoundItems)
                    {
                        var imageUrl = item.ImageFile?.Length > 0
                            ? await _fileUploadService.UploadFileAsync(item.ImageFile)
                            : null;
                        complain.LostOrFoundItems.Add(new LostOrFoundItem
                        {
                            ItemDescription = item.ItemDescription,
                            ImageUrl = imageUrl
                        });
                    }
                }
                _context.Add(complain);
                await _context.SaveChangesAsync();
                TempData["Message"] = "You have successfully submitted your complain. This complain is under review.";
                return Redirect(nameof(Index));
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PickUp(int pickupId)
        {
            var pickUp = await _context.PickUps
                .FirstOrDefaultAsync(p => p.PickUpId == pickupId);
            if (pickUp == null)
            {
                return NotFound(new { success = false, message = "Pick-up not found" });
            }
            pickUp.Status = PickUpStatus.PickingUp;
            _context.Update(pickUp);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPickUp(int pickupId, string Comments)
        {

            var pickUp = await _context.PickUps
                .Include(p => p.Complain)
                .Where(p => p.PickUpId == pickupId)
                .FirstOrDefaultAsync();
         

            if (pickUp == null)
            {
                return NotFound(new { success = false, message = "Pick-Up not found" });
            }

            var complain = pickUp.Complain;

            complain.Status = ComplainStatus.Completed;

            pickUp.Status = PickUpStatus.Completed;
            pickUp.Comments = Comments;

            _context.Update(complain);
            _context.Update(pickUp);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


        // Controller action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartDelivery(int deliveryId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var delivery = await _context.Deliveries
                    .Where(d => d.DeliveryId == deliveryId)
                    .FirstOrDefaultAsync();

                if (delivery == null)
                {
                    return NotFound();
                }

                delivery.Status = DeliveryStatus.Delivering;
                _context.Update(delivery);
                await _context.SaveChangesAsync();

                BackgroundJob.Enqueue(() => _receiveItemsService.NotifyDeliveryCustomerContactAsync(deliveryId));

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to start delivery: " + ex.Message,
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
        public async Task<IActionResult> VerifyOtp(int pickupId, string otpCode)
        {
            if (pickupId <= 0 || string.IsNullOrEmpty(otpCode))
            {
                return Json(new { success = false, message = "Invalid input parameters." });
            }

            var pickUp = await _context.PickUps.FindAsync(pickupId);
            if (pickUp == null)
            {
                return Json(new { success = false, message = "Pick-Up not found." });
            }

            bool isValidOtp = !string.IsNullOrEmpty(pickUp.OTP) && pickUp.OTP == otpCode;

            if (isValidOtp)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Incorrect OTP code." });
            }
        }



        private string GenerateOTP()
        {
            var random = new Random();
            return string.Concat(Enumerable.Range(0, 6).Select(_ => random.Next(0, 10).ToString()));
        }

    }

}
