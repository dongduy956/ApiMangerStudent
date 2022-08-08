using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Models
{
    public class ChangePassword
    {
        [Required]
        public string password { get; set; }
        [Required]
        public string newPassword { get; set; }
        [Required]
        public string prePassword { get; set; }
    }
}
