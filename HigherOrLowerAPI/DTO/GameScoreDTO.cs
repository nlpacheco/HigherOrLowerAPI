using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerAPI.DTO
{
    public class GameScoreDTO
    {
        public uint GameId { get; }
        public string CardFlag { get; }
        public IEnumerable<PlayerScoreDTO> Scores { get; }
        public string Message { get; }

        public GameScoreDTO(uint gameId, string cardFlag, IEnumerable<PlayerScoreDTO> scores, string message)
        {
            GameId = gameId;
            CardFlag = cardFlag;
            Scores = scores;
            Message = message;
        }
    }
}
