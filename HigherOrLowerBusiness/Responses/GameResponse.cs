using HigherOrLowerData.Business;
using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerData.Responses
{
    internal class GameResponse: IGameResponse
    {
        public uint GameId { get; }
        public string CurrentCard { get; }
        public int RoundCounter { get; }
        public CARD_FLAG CardFlag { get; }

//        public string Message { get; }



        public GameResponse(uint gameId, string currentCard, int roundCounter, CARD_FLAG cardFlag)
        {
            GameId = gameId;
            CurrentCard = currentCard;
            RoundCounter = roundCounter;
            CardFlag = cardFlag;

        }
       

    }
}
