using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace StainSaver.Services
{
    public class ReceiveItemsService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SmsService _smsService;

        public ReceiveItemsService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SmsService smsService)
        {
            _context = context;
            _userManager = userManager;
            _smsService = smsService;
        }

        public async Task NotifyDeliveryCustomerContactAsync(int deliveryId)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Complain)
                    .ThenInclude(c => c.Customer)
                .Include(d => d.Package)
                .FirstOrDefaultAsync(d => d.DeliveryId == deliveryId);

            if (delivery == null || delivery.Complain == null || delivery.Complain.Customer == null || delivery.Package == null)
                return;

            var customer = delivery.Complain.Customer;
            var phone = customer.PhoneNumber;
            var email = customer.Email;

            string baseUrl = "https://20.164.17.133:2005";
            string encodedPackageId = WebUtility.UrlEncode(delivery.Package.PackageId.ToString());
            string receiveDeliveryLink = $"{baseUrl}/Customer/Complains/ScanPackage?packageId={encodedPackageId}";

            string smsMessage = $"Dear {customer.FullName}, your delivery is on the way. Please track or receive it here: {receiveDeliveryLink}";

            string emailMessage = $@"
                Dear {customer.FullName}, your delivery is on the way.<br/>
                Please <a href=""{receiveDeliveryLink}"">track or receive your delivery here</a>.
            ";

            if (!string.IsNullOrEmpty(phone))
            {
                try
                {
                    await _smsService.SendSmsAsync(phone, smsMessage);
                }
                catch
                {

                }
            }
        }
    }
}
