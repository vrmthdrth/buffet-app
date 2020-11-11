using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetDAL.AdditionalModels
{
    public class MenuUpdateModel
    {
        public int Id { get; set; }
        public string FoodName { get; set; }
        public decimal Weight { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int? Base { get; set; }
        public int? Available { get; set; }
        public int? Insufficient { get; set; }
    }
}
