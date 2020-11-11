using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuffetAuxiliaryLib.DTOs;

namespace BuffetClientMVC.Models
{
    public class MenuViewModel
    {
        public MenuDTO Menu { get; set; }
        public List<MenuFoodDTO> MenuFoods { get; set; }
    }
}
