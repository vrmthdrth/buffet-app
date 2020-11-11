using BuffetDAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetDAL.AdditionalModels
{
    public class MenuRowModel
    {
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public int? BaseAmount { get; set; }
    }
}
