namespace HigherOrLowerAPI.DTO
{
    public class PlayerScoreDTO
    {
        public string PlayerName { get; }
        public int Score { get; }

        public PlayerScoreDTO(string playerName, int score)
        {
            PlayerName = playerName;
            Score = score;
        }
    }
}
