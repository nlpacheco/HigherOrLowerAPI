using HigherOrLowerAPI.Tools;
using HigherOrLowerData.Business;
using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerAPI.DTO
{
    public class GameDTO
    {
        public uint GameId { get; }
        public string CurrentCard { get; }
        public int RoundCounter { get; }
        public string CardFlag { get; }

        public string Message { get; }

        public GameDTO(uint gameId, string currentCard, int roundCounter, string cardFlag, string message)
        {
            GameId = gameId;
            CurrentCard = currentCard;
            RoundCounter = roundCounter;
            CardFlag = cardFlag;
            Message = message;

        }

    }
}
