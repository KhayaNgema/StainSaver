using StainSaver.Models;
using System.Collections.Generic;

namespace StainSaver.Areas.Admin.Models
{
    public class UsersViewModel
    {
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
    }

    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
} 