using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerData.Responses
{
    public  interface IGameMoveResponse
    {
        public uint MoveId { get; }
        public uint GameId { get; }
        public string PlayerName{ get; }
        public bool IsHigher { get; }
        public bool IsCorrect { get; }
        public string PreviousCard { get; }
        public string CurrentCard { get; }
        public int RoundCounter { get; }
        public CARD_FLAG CardFlag { get; }


    }
}
