using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class UserFavouriteFood 
    {
        public int Id { get; set; }
        public int FoodId { get; set; }

        [ForeignKey("FoodId")]
        public Food Food { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
