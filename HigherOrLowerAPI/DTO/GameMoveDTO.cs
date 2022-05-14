using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerAPI.DTO
{
    public class GameMoveDTO
    {
        public uint MoveId { get; }
        public uint GameId { get; }
        public string CardFlag { get; }
        public string PlayerName { get; }
        public bool IsHigher { get; }
        public bool IsCorrect { get; }
        public string CurrentCard { get; }
        public string Message { get; }

        public GameMoveDTO(uint moveId, uint gameId, string playerName, bool isHigher, bool isCorrect, string currentCard, string cardFlag, string message)
        {
            MoveId = moveId;
            GameId = gameId;
            CardFlag = cardFlag;
            PlayerName = playerName;
            IsHigher = isHigher;
            IsCorrect = isCorrect;
            CurrentCard = currentCard;
            Message = message;
        }
    }
}
