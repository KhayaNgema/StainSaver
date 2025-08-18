using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HospitalManagement.Services
{
    public class PickUpSmsService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly SmsService _smsService;

        public PickUpSmsService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SmsService smsService)
        {
            _context = context;
            _userManager = userManager;
            _smsService = smsService;
        }

        public async Task NotifyPickUpFromCustomerContactAsync(int pickUpId)
        {
            Console.WriteLine($"[NotifyPickUp] Started processing pickUpId: {pickUpId} at {DateTime.UtcNow}");

            var pickUp = await _context.PickUps
                .Include(p => p.Complain)
                    .ThenInclude(c => c.Customer)
                .FirstOrDefaultAsync(p => p.PickUpId == pickUpId);

            if (pickUp == null)
            {
                Console.WriteLine($"[NotifyPickUp] PickUp not found for Id: {pickUpId}");
                return;
            }

            if (pickUp.Complain == null)
            {
                Console.WriteLine($"[NotifyPickUp] PickUp {pickUpId} has null Complain");
                return;
            }

            if (pickUp.Complain.Customer == null)
            {
                Console.WriteLine($"[NotifyPickUp] Complain for PickUp {pickUpId} has null Customer");
                return;
            }

            var customer = pickUp.Complain.Customer;
            var phone = customer.PhoneNumber;

            if (string.IsNullOrEmpty(phone))
            {
                Console.WriteLine($"[NotifyPickUp] Customer {customer.Id} has no phone number");
                return;
            }

            string smsMessage = $"Dear {customer.FullName}, please use the following OTP code {pickUp.OTP} to verify that you have handed over the items to the pickup driver. Do not share this secret code with anyone.";

            try
            {
                Console.WriteLine($"[NotifyPickUp] Sending SMS to {phone} with message: {smsMessage}");
                await _smsService.SendSmsAsync(phone, smsMessage);
                Console.WriteLine($"[NotifyPickUp] SMS sent successfully to {phone}");
            }
            catch (Exception smsEx)
            {
                Console.WriteLine($"[NotifyPickUp] Failed to send SMS to {phone}: {smsEx}");
            }

            Console.WriteLine($"[NotifyPickUp] Completed processing pickUpId: {pickUpId} at {DateTime.UtcNow}");
        }

    }
}
