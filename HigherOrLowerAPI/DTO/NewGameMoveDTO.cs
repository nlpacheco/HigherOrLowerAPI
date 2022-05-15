using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HigherOrLowerAPI.DTO
{
    public class NewGameMoveDTO
    {
        [JsonRequired]
        [Required]
        public string Player { get; set; }

        [JsonRequired]
        [Required]
        public bool IsHigher { get; set; }


    }
}
