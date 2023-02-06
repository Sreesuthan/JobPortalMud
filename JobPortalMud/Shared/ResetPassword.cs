using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortalMud.Shared
{
    public class ResetPassword
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(UserPassword), ErrorMessage = "Password and Confirm password Should be same")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
