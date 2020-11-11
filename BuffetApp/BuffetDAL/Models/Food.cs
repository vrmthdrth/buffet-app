using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }

        [Required, MaxLength(255)]
        public string Description { get; set; }

        [Range(1, 4)]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public ICollection<UserFavouriteFood> UserFavouriteFoods { get; set; }
        public ICollection<MenuFood> MenuFoods { get; set; }   
        
        public Food()
        {
            this.MenuFoods = new List<MenuFood>();
        }
    }
}
