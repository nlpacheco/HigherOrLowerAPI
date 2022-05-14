using HigherOrLowerData.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Repository
{
    public class GamePlayerRepository : RepositoryEF<GamePlayer, uint>, IGamePlayerRepository
    {
        public GamePlayerRepository(DbContext context) : base(context)
        {
        }

        public async Task<GamePlayer?> findGamePlayerAsync(uint gameId, int playerPosition)
        {
            return  (await FindAsync(   p => (p.GameId == gameId)
                                             &&
                                             (p.RoundPosition == playerPosition)
                                    )
                    ).ToList().FirstOrDefault();
        }

        public async Task<IEnumerable<GamePlayer>> findAllPlayerOfGame(uint gameId)
        {
            return await _entities.Where(p => p.GameId == gameId).OrderBy(p=> p.RoundPosition).ToListAsync();

            // return (await FindAsync(p => p.GameId == gameId)).ToList();
        }
    }
}
