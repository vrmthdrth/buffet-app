using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetAuxiliaryLib.DTOs
{
    public class FeedbackDTO
    {
        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Anonymously")]
        public bool Anonymously { get; set; }
    }
}
