using HigherOrLowerData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Repository
{
    public interface IGamePlayerRepository: IRepository<GamePlayer, uint>
    {
        public Task<GamePlayer?> findGamePlayerAsync(uint gameId, int playerPosition);
        public Task<IEnumerable<GamePlayer>> findAllPlayerOfGame(uint gameId);
    }
}
