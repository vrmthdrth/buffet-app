using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetDAL.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }

        public ICollection<Food> Foods { get; set; }
    }
}
