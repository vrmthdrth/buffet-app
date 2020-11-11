using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class Reserve
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateTime { get; set; }

        public bool? IsAccepted { get; set; }
        public ICollection<MenuFoodReserve> MenuFoodReserves { get; set; }
    
        public Reserve()
        {
            this.MenuFoodReserves = new List<MenuFoodReserve>();
        }
    }
}
