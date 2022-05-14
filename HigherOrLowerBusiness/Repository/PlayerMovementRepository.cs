using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HigherOrLowerData.Entities;
using Microsoft.EntityFrameworkCore;

namespace HigherOrLowerData.Repository
{
    public class PlayerMoveRepository: RepositoryEF<PlayerMove, uint>, IPlayerMoveRepository
    {
        public PlayerMoveRepository(DbContext context) : base(context)
        {
        }

    }
}