using StainSaver.Data;
using StainSaver.Models;
using StainSaver.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace StainSaver.Services
{
    public class SendRefundSms
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SmsService _smsService;

        public SendRefundSms(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SmsService smsService)
        {
            _context = context;
            _userManager = userManager;
            _smsService = smsService;
        }

        public async Task NotifyRefundCustomerContactAsync(int complainId)
        {
            var complain = await _context.Complains
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.ComplainId == complainId);

            if (complain == null || complain.Customer == null)
                return;

            var customer = complain.Customer;
            var phone = customer.PhoneNumber;
            var email = customer.Email;

            string refundMessage = $"Dear {customer.FullName}, your refund has been processed and you will receive funds in 24 hrs.";

            if (!string.IsNullOrEmpty(phone))
            {
                try
                {
                    await _smsService.SendSmsAsync(phone, refundMessage);
                }
                catch
                {
                }
            }
        }
    }
}
