using Microsoft.AspNetCore.Mvc.Rendering;
using StainSaver.Models;
using System.ComponentModel.DataAnnotations;

namespace StainSaver.Areas.Admin.Models
{
    public class AssignDriverViewModel
    {
        public int BookingId { get; set; }
        
        [Required(ErrorMessage = "Please select a driver")]
        [Display(Name = "Driver")]
        public string? DriverId { get; set; }
        
        // Navigation property
        public Booking? Booking { get; set; }
        
        // For dropdown
        public SelectList? Drivers { get; set; }
    }
    
    public class AssignStaffViewModel
    {
        public int BookingDetailId { get; set; }
        
        [Required(ErrorMessage = "Please select a staff member")]
        [Display(Name = "Staff Member")]
        public string? StaffId { get; set; }
        
        public BookingDetail? BookingDetail { get; set; }
        
        public SelectList? StaffMembers { get; set; }
    }
} 