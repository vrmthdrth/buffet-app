using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetAuxiliaryLib.DTOs
{
    public class CreateReserveModelDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Quantity")]
        public int Quantity { get; set; }
    }
}
