using HigherOrLowerData.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData
{
    public interface IUnitOfWork
    {
        //ApplicationDbContext ApplicationDbContext { get; }

        IGameRepository Game { get; }
        IGamePlayerRepository GamePlayer { get; }
        IPlayerMoveRepository PlayerMove { get; } 


        //int SaveChanges();
        Task<int> SaveChangesAsync();
        //public Task BeginTransactionAsync();
        //public Task CommitTransactionAsync();
        //public Task RollbackTransactionAsync();

    }
}
