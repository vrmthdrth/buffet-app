using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetAuxiliaryLib.DTOs
{
    public class LoginDTO
    {
        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }
    }
}
