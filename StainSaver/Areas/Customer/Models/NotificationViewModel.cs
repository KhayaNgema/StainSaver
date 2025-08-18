using StainSaver.Models;
using System.Collections.Generic;

namespace StainSaver.Areas.Customer.Models
{
    public class NotificationViewModel
    {
        public List<CustomerNotification> Notifications { get; set; }
        public int UnreadCount { get; set; }
    }
} 