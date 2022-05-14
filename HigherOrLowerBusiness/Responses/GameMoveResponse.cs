using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerData.Responses
{
    public class GameMoveResponse: IGameMoveResponse
    {
        public uint MoveId { get; }
        public uint GameId { get; }
        public string PlayerName { get; }
        public bool IsHigher { get; }
        public bool IsCorrect { get; }
        public string PreviousCard { get; }
        public string CurrentCard { get; }
        public int RoundCounter { get; }
        public CARD_FLAG CardFlag { get; }


        public GameMoveResponse(uint moveId, uint gameId, string playerName, bool isHigher, bool isCorrect, string previousCard, string currentCard, int roundCounter, CARD_FLAG cardFlag)
        {
            MoveId = moveId;
            GameId = gameId;
            PlayerName = playerName;
            IsHigher = isHigher;
            IsCorrect = isCorrect;
            PreviousCard = previousCard;
            CurrentCard = currentCard;
            RoundCounter = roundCounter;
            CardFlag = cardFlag;
            //Message = TextResponse.BuildCardMessage(gameId, roundCounter, card) + " " + TextResponse.BuildWinMessage(win);
        }



    }
}
