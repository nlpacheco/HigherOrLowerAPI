using HigherOrLowerData.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData
{
    public class UnitOfWorkEF: IUnitOfWork  //, IDisposable
    {
        private IGameRepository? _gameRepository;
        private IGamePlayerRepository? _gamePlayerRepository;
        private IPlayerMoveRepository? _playerMoveRepository;

        //Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _currentTransaction;

        readonly ApplicationDbContext _context;



        public UnitOfWorkEF(ApplicationDbContext context)
        {
            _context = context;
            //_currentTransaction = null;
        }


        //public ApplicationDbContext ApplicationDbContext
        //{
        //    get { return _context; }
        //}

        public IGameRepository Game {  get { return _gameRepository ??= new GameRepository(_context);  } }
        public IGamePlayerRepository GamePlayer {  get { return _gamePlayerRepository ??= new GamePlayerRepository(_context);  } }
        public IPlayerMoveRepository PlayerMove {  get { return _playerMoveRepository ??= new PlayerMoveRepository(_context); } }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        //public async Task BeginTransactionAsync()
        //{
        //    if (_currentTransaction == null)
        //        _currentTransaction = await _context.Database.BeginTransactionAsync();
        //}

        //public async Task CommitTransactionAsync()
        //{
            
        //    await _context.Database.CommitTransactionAsync();
        //    _currentTransaction = null;
        //}

        //public async Task RollbackTransactionAsync()
        //{
        //    await _context.Database.RollbackTransactionAsync();
        //    _currentTransaction = null;
        //}

        //public void Dispose()
        //{
        //    if (_currentTransaction != null)
        //        _context.Database.RollbackTransaction();
        //    _context.Dispose();
        //}
    }
}
