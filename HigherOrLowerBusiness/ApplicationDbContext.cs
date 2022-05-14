using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Entities.Game> Games { get; set; } 
        public DbSet<Entities.GamePlayer> GamePlayers{ get; set; }
        public DbSet<Entities.PlayerMove> PlayerMoves { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

    }
}
