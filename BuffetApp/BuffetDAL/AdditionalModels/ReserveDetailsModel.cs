using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetDAL.Models //AdditionalModels
{
    public class ReserveDetailsModel
    {
        public string FoodName { get; set; }
        public string FoodDescription { get; set; }
        public int FoodAmount { get; set; }
        public decimal FoodPrice { get; set; }
        public decimal FoodSum { get; set; }
    }
}
