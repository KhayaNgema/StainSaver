using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace StainSaver.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }
        
        [PersonalData]
        [Display(Name = "ID Number")]
        public string? IdNumber { get; set; }
        
        [PersonalData]
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }
        
        [PersonalData]
        [Display(Name = "Suburb")]
        public string? Suburb { get; set; }
        
        [PersonalData]
        [Display(Name = "City")]
        public string? City { get; set; }
        
        [PersonalData]
        [Display(Name = "Province")]
        public string? Province { get; set; }
        
        [PersonalData]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }
        
        [PersonalData]
        [Display(Name = "Alternative Contact Number")]
        public string? AlternativeContactNumber { get; set; }
        
        public string Address
        {
            get
            {
                var addressParts = new List<string>();
                
                if (!string.IsNullOrEmpty(StreetAddress))
                    addressParts.Add(StreetAddress);
                
                if (!string.IsNullOrEmpty(Suburb))
                    addressParts.Add(Suburb);
                
                if (!string.IsNullOrEmpty(City))
                    addressParts.Add(City);
                
                if (!string.IsNullOrEmpty(Province))
                    addressParts.Add(Province);
                
                if (!string.IsNullOrEmpty(PostalCode))
                    addressParts.Add(PostalCode);
                
                return addressParts.Count > 0 ? string.Join(", ", addressParts) : "No address provided";
            }
        }
    }
} 