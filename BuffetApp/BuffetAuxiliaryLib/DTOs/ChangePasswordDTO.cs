using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetAuxiliaryLib.DTOs
{
    public class ChangePasswordDTO
    {

        [JsonProperty("OldPassword")]
        public string OldPassword { get; set; }

        [JsonProperty("NewPassword")]
        public string NewPassword { get; set; }

        [JsonProperty("PasswordConfirm")]
        public string PasswordConfirm { get; set; }
    }
}
