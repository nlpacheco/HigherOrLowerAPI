using HigherOrLowerData.Business;
using HigherOrLowerData.Entities;
using HigherOrLowerData.Responses;
using System.Text;
using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerAPI.Tools
{
    public class TextResponse
    {

        public static string BuildCardMessage(uint gameId, CARD_FLAG cardFlag, string card)
        {
            return $"The {BuildCardFlagMessage(cardFlag)}card in Game{gameId} is {card}.";
        }


        public static string BuildCardMessage(uint gameId, CARD_FLAG cardFlag, string card, bool isCorrect)
        {
            return $"The {BuildCardFlagMessage(cardFlag)}card in Game{gameId} is {card}. {BuildIsCorrectMessage(isCorrect)}." ;
        }

        private static string BuildIsCorrectMessage(bool isCorrect)
        {
            return (isCorrect ? "Correct" : "Incorrect");
        }


        public static string BuildGameStatusMessage(uint gameId, CARD_FLAG cardFlag, IEnumerable<GamePlayer> players)
        {

            return $"The {BuildCardFlagMessageForStatus(cardFlag)} for the Game{gameId} is " + BuidAllPlayerStatus(players);

        }


        private static string BuildCardFlagMessage(CARD_FLAG cardFlag)
        {
            return
                cardFlag switch
                {
                    CARD_FLAG.FIRST_CARD => "first ",
                    CARD_FLAG.LAST_CARD => "last ",
                    CARD_FLAG.END_OF_GAME => "",
                    _ => ""
                };
        }

        private static string BuildCardFlagMessageForStatus(CARD_FLAG cardFlag)
        {
            return
                cardFlag switch
                {

                    CARD_FLAG.LAST_CARD => "final result ",
                    CARD_FLAG.END_OF_GAME => "final result ",
                    _ => "current score"
                };
        }

        private static string BuidAllPlayerStatus(IEnumerable<GamePlayer> players)
        {
            int iPlayerCounter = players.Count();
            StringBuilder sb = new StringBuilder();
            foreach (var player in players)
            {
                sb.Append(player.PlayerName);
                sb.Append(":");
                sb.Append(player.Wins);
                sb.Append(iPlayerCounter switch
                {
                    2 => " and ",
                    1 => "",
                    _ => ", "
                });

                iPlayerCounter--;
            }
            return sb.ToString();
        }

    }
}
