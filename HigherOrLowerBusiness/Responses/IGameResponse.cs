using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Responses
{
    public interface IGameResponse
    {

        public enum CARD_FLAG
        {
            FIRST_CARD,
            LAST_CARD,
            IN_THE_MIDDLE_CARD,
            END_OF_GAME
        }


        public uint GameId { get; }
        public string CurrentCard { get; }
        public int RoundCounter { get; }
        public CARD_FLAG CardFlag { get; }

        //public string Message { get; }
    }
}
