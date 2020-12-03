using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class Menu
    {
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        //public string Info { get; set; }

        public ICollection<MenuFood> MenuFoods { get; set; }

        public Menu()
        {
            this.MenuFoods = new List<MenuFood>();
        }
    }
}
