using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Responses
{
    internal class GameStatusPlayerResponse : IGameStatusPlayerResponse
    {
        public string PlayerName { get; }

        public int PlayerScore { get; }

        public GameStatusPlayerResponse(string playerName, int playerScore )
        {
            PlayerName = playerName;
            PlayerScore = playerScore;
        }


    }
}
