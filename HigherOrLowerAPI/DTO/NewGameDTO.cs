using Newtonsoft.Json;

namespace HigherOrLowerAPI.DTO
{
    public class NewGameDTO
    {
        [JsonRequired]
        public string[]? Players { get; set; }
    }
}
