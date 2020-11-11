using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetAdminMVC.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Name is required"), MaxLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required"), MaxLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        public string Email { get; set; }

        public string RoleName { get; set; }
    }
}
