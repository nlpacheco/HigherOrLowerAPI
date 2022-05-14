﻿using HigherOrLowerData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerData.Responses
{
    internal class GameStatusResponse: IGameStatusResponse
    {
        public uint GameId { get; }
        public CARD_FLAG CardFlag { get; }
        public IEnumerable<GamePlayer> GamePlayers { get; }

        public GameStatusResponse(uint gameId, CARD_FLAG cardFlag, IEnumerable<GamePlayer> gamePlayers)
        {
            GameId = gameId;
            CardFlag = cardFlag;
            GamePlayers = gamePlayers;

        }

    }
}
