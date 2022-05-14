using HigherOrLowerData;
using HigherOrLowerData.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.HigherOrLower.Mocking
{
    internal class MockUnitOfWork : IUnitOfWork
    {

        public bool SaveChangesCalled { get; private set; }

        public IGameRepository Game => _game;

        public IGamePlayerRepository GamePlayer => _gamePlayer;

        public IPlayerMoveRepository PlayerMove => _playerMove;

        IGameRepository _game;
        IGamePlayerRepository _gamePlayer;
        IPlayerMoveRepository _playerMove;


        public MockUnitOfWork(IGameRepository game, IGamePlayerRepository gamePlayer, IPlayerMoveRepository playerMove)
        {
            _game = game;
            _gamePlayer = gamePlayer;
            _playerMove = playerMove;
            SaveChangesCalled = false;
        }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalled = true;
            return Task.FromResult(1); ;
        }
    }
}
