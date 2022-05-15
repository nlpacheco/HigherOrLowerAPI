using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HigherOrLowerAPI.DTO
{
    public class NewGameDTO
    {
        [JsonRequired]
        [Required]
        public string[]? Players { get; set; }
    }
}
