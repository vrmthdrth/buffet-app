using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetAuxiliaryLib.DTOs
{
    public class MenuFoodDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("FoodId")]
        public int FoodId { get; set; }
        [JsonProperty("MenuId")]
        public int MenuId { get; set; }


        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Weight")]
        public decimal Weight { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("CategoryId")]
        public int CategoryId { get; set; }
        [JsonProperty("CategoryName")]
        public string CategoryName { get; set; }


        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("BaseAmount")]
        public int? BaseAmount { get; set; }
        [JsonProperty("AvailableAmount")]
        public int? AvailableAmount { get; set; }
        [JsonProperty("InsufficientAmount")]
        public int? InsufficientAmount { get; set; }
    }
}




