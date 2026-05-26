using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuT_and_Trololo.Models
{
    public class Polzovatel
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsFrozen { get; set; }
        public string FreezeReason { get; set; }
        public string AvatarPath { get; set; }
        public string About { get; set; }
    }
}
