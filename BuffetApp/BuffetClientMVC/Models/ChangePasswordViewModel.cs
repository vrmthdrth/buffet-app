using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetClientMVC.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Enter old password")]
        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Enter new password"), MinLength(8), MaxLength(64)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Enter confirmation password"), MinLength(8), MaxLength(64)]
        [Compare("NewPassword", ErrorMessage = "Passwords are not same")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string PasswordConfirm { get; set; }
    }
}
