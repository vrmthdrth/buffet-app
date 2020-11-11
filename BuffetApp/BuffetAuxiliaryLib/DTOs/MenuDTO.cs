using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetAuxiliaryLib.DTOs
{
    public class MenuDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("Date")]
        public DateTime Date { get; set; }
    }
}
