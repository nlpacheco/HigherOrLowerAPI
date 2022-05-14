using HigherOrLowerData.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Repository
{
    public class GameRepository: RepositoryEF<Game, uint>, IGameRepository
    {
        public GameRepository(DbContext context) : base(context)
        {
        }

    }
}
