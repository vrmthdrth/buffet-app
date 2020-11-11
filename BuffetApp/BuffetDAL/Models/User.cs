using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Surname { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; }

        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<UserFavouriteFood> UserFavouriteFoods { get; set; }
        public ICollection<Reserve> Reserves { get; set; }

        public User()
        {
            this.UserFavouriteFoods = new List<UserFavouriteFood>();
        }
    }
}
