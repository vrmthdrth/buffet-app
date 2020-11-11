using Newtonsoft.Json;

namespace BuffetAuxiliaryLib.DTOs
{
    public class UserDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Surname")]
        public string Surname { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("RoleId")]
        public int RoleId { get; set; }
        
        [JsonProperty("RoleName")]
        public string RoleName { get; set; }
    }
}
