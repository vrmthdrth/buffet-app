using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class MenuFoodReserve
    {
        public int Id { get; set; }

        public int MenuFoodId { get; set; }
        [ForeignKey("MenuFoodId")]
        public MenuFood MenuFood { get; set; }

        public int ReserveId { get; set; }
        [ForeignKey("ReserveId")]
        public Reserve Reserve { get; set; }
        public int Amount { get; set; }
    }
}
