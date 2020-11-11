using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetAdminMVC.Models
{
    public class ReserveModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool? IsAccepted { get; set; }
    }
}
