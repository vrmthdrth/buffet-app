using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class MenuFood
    {
        public int Id { get; set; }

        public int FoodId { get; set; }

        [ForeignKey("FoodId")]
        public Food Food { get; set; }

        public int MenuId { get; set; }

        [ForeignKey("MenuId")]
        public Menu Menu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int? BaseAmount { get; set; }
        public int? AvailableAmount { get; set; }
        public int? InsufficientAmount { get; set; }

        public ICollection<MenuFoodReserve> MenuFoodReserves { get; set; }
    
        public MenuFood()
        {
            this.MenuFoodReserves = new List<MenuFoodReserve>();
        }
    }
}
