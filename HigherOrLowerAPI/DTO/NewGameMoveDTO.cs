using Newtonsoft.Json;

namespace HigherOrLowerAPI.DTO
{
    public class NewGameMoveDTO
    {
        [JsonRequired] 
        public string Player { get; set; }

        [JsonRequired]
        public bool IsHigher { get; set; }


    }
}
