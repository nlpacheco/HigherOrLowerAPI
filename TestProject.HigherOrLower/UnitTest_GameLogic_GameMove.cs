using HigherOrLowerData.Business;
using HigherOrLowerData.Entities;
using HigherOrLowerData.Repository;
using HigherOrLowerData.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.HigherOrLower.Mocking;

namespace TestProject.HigherOrLower
{
    internal class UnitTest_GameLogic_GameMove
    {
        private readonly string Player1 = "John";
        private readonly string Player2 = "James";
        private readonly string Player3 = "Carl";

        private readonly string fakeRandomCardAsText = "3 3,2 2,10 1,10 0,12 2,13 2,0 2,2 3,9 2,10 2,0 3,8 3,7 3,12 3,4 2,11 0,5 1,10 3,6 1,2 1,5 0,6 2,0 0,9 3,9 1,3 2,6 3,7 0,11 3,11 2,13 3,0 1,9 0,8 0,5 3,3 1,8 2,2 0,11 1,12 1,8 1,4 3,7 2,4 0,4 1,7 1,13 1,6 0,3 0,5 2,13 0,12 0";
        private readonly Card fakeCard0 = new Card(Card.CardValues.Three, Card.Suits.Spades);
        private readonly Card fakeCard1 = new Card(Card.CardValues.Two, Card.Suits.Hearts);
        private readonly Card fakeCard2 = new Card(Card.CardValues.Ten, Card.Suits.Diamonds);
        private readonly Card fakeCard3 = new Card(Card.CardValues.Ten, Card.Suits.Clubs);

        GameLogic? _gameLogic;
        ILoggerFactory? _loggerfactory;



        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                                        .AddLogging(builder => builder.AddConsole())
                                        .BuildServiceProvider();

            _loggerfactory = serviceProvider.GetService<ILoggerFactory>();

        }


        [Test]
        public async Task UnitTest_GameMove_BadGameId()
        {
            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: wrong game Id
            uint wrongGameId = 999;
            string currentPlayer = Player1;
            bool isHigher = true;
            Game currentGame = new Game()
            {
                Cards = fakeRandomCardAsText,
                CurrentRound = 0,
                Id = wrongGameId,
                PlayerCount = 3
            };


            // ARRANGE - Repositories and success conditions

            var mockGameRepository = new Mock<IGameRepository>();
            mockGameRepository.Setup(repo => repo.GetAsync(wrongGameId)).Returns(Task.FromResult((Game)null));

            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();


            var unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);


            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);


            // ACT
            var result = await gameLogic.NewGameMove(wrongGameId, currentPlayer, isHigher);


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.Not_Found);
            Assert.AreEqual(result.ErrorMessage, RESULT_MESSAGE.INVALID_GAME_ID);
            mockGameRepository.VerifyAll();


        }






        [Test]
        public async Task UnitTest_GameMove_TestDeckOfCard_WrongPlayer()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: wrong player
            uint gameId = 1;
            var correctPlayer = Player1;
            var wrongPlayer = Player3;


            var currentPlayerPosition = 0;
            var isHigher = true;

            var currentGame = new Game()
            {
                Cards = fakeRandomCardAsText,
                CurrentRound = 0,
                Id = gameId,
                PlayerCount = 3
            };

            var currentGamePlayer = new GamePlayer()
            {
                GameId = gameId, 
                PlayerName = correctPlayer,
                RoundPosition = currentPlayerPosition
            };



            // ARRANGE - Repositories and success conditions

            var mockGameRepository = new Mock<IGameRepository>();
            mockGameRepository.Setup(repo => repo.GetAsync(gameId).Result)
                              .Returns(currentGame);

            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            mockGamePlayerRepository.Setup(repo => repo.findGamePlayerAsync(gameId, currentPlayerPosition).Result)
                                    .Returns(currentGamePlayer);

            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();


            var unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);


            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);


            // ACT
            var result = await gameLogic.NewGameMove(gameId, wrongPlayer, isHigher);

            TestContext.Out.WriteLine(result.ErrorMessage);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.Not_Found);
            Assert.AreEqual(result.ErrorMessage, RESULT_MESSAGE.WRONG_PLAYER);
            mockGameRepository.VerifyAll();

        }





        [Test]
        public async Task UnitTest_GameMove_TestDeckOfCard_1stMov_Looses()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ----------------------------------------------------------------------
            // ARRANGE - Use case: 1st Move and looses

            uint correctGameId = 1;
            var isHigher = true;            // player's guess
            
            var currentPlayer = Player1;
            var currentPlayerPosition = 0;  // Game player position


            var currentGame = new Game()
            {
                Cards = fakeRandomCardAsText,
                CurrentRound = 0,
                Id = correctGameId,
                PlayerCount = 3
            };

            var currentGamePlayer = new GamePlayer()
            {
                GameId = correctGameId, 
                PlayerName = currentPlayer, 
                RoundPosition = currentPlayerPosition
            };

            // ARRANGE - Repositories and success conditions

            var mockGameRepository = new Mock<IGameRepository>();
            mockGameRepository.Setup(repo => repo.GetAsync(correctGameId).Result)
                              .Returns(currentGame);

            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            mockGamePlayerRepository.Setup(repo => repo.findGamePlayerAsync(correctGameId, currentPlayerPosition).Result)
                                    .Returns(currentGamePlayer);

            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();


            var unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);


            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);


            // ACT
            var result = await gameLogic.NewGameMove(correctGameId, currentPlayer, isHigher);


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.AreEqual(result.GameResponse.GameId, correctGameId);
            Assert.AreEqual(currentGamePlayer.Wins, 0);
            Assert.AreEqual(result.GameResponse.CurrentCard, fakeCard1.ToString());
            mockGameRepository.VerifyAll();

        }




        [Test]
        public async Task UnitTest_GameMove_TestDeckOfCard_1stMov_Wins()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: 1st Move and wins
            uint gameId = 1;
            var isHigher = false;               // player's guess
            var currentPlayer = Player1;
            var currentPlayerPosition = 0;      // Game player position

            var currentGame = new Game()
            {
                Cards = fakeRandomCardAsText,
                CurrentRound = 0,
                Id = gameId,
                PlayerCount = 3
            };

            var currentGamePlayer = new GamePlayer()
            {
                GameId = gameId, 
                PlayerName = currentPlayer, 
                RoundPosition = currentPlayerPosition
            };


            // ARRANGE - Repositories and success conditions

            var mockGameRepository = new Mock<IGameRepository>();
            mockGameRepository.Setup(repo => repo.GetAsync(gameId).Result)
                              .Returns(currentGame);

            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            mockGamePlayerRepository.Setup(repo => repo.findGamePlayerAsync(gameId, currentPlayerPosition).Result)
                                    .Returns(currentGamePlayer);

            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();


            var unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);


            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);


            // ACT
            var result = await gameLogic.NewGameMove(gameId, currentPlayer, isHigher);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.AreEqual(result.GameResponse.GameId, gameId);
            Assert.AreEqual(currentGamePlayer.Wins, 1);
            Assert.AreEqual(result.GameResponse.CurrentCard, fakeCard1.ToString());
            mockGameRepository.VerifyAll();

        }






        [Test]
        public async Task UnitTest_GameMove_TestDeckOfCard_3rdMov_SameNumber_HigherWins()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: 3rd Move, next card will be the same number, user choose Higher and wins
            
            uint gameId = 1;            
            var isHigher = true;            // player's guess            
            var currentPlayer = Player3;
            var currentPlayerPosition = 2;  // Game player position



            
            var currentGame = new Game()
            {
                Cards = fakeRandomCardAsText,
                CurrentRound = 2,
                Id = gameId,
                PlayerCount = 3
            };

            var currentGamePlayer = new GamePlayer()
            {
                GameId = gameId, 
                PlayerName = currentPlayer,
                RoundPosition = currentPlayerPosition
            };

            // ARRANGE - Repositories and success conditions

            var mockGameRepository = new Mock<IGameRepository>();
            mockGameRepository.Setup(repo => repo.GetAsync(gameId).Result)
                              .Returns(currentGame);

            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            mockGamePlayerRepository.Setup(repo => repo.findGamePlayerAsync(gameId, currentPlayerPosition).Result)
                                    .Returns(currentGamePlayer);

            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();


            var unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);


            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);


            // ACT
            var result = await gameLogic.NewGameMove(gameId, currentPlayer, isHigher);




            // ASSERT
           
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.AreEqual(result.GameResponse.GameId, gameId);
            Assert.AreEqual(currentGamePlayer.Wins, 1);
            Assert.AreEqual(result.GameResponse.CurrentCard, fakeCard3.ToString());
            mockGameRepository.VerifyAll();
            
        }



        [Test]
        public async Task UnitTest_GameMove_TestDeckOfCard_3rdMov_SameNumber_LowerWins()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: 3rd Move, next card will be the same number, user choose Higher and wins

            uint gameId = 1;
            var isHigher = false;            // player's guess            
            var currentPlayer = Player3;
            var currentPlayerPosition = 2;  // Game player position




            var currentGame = new Game()
            {
                Cards = fakeRandomCardAsText,
                CurrentRound = 2,
                Id = gameId,
                PlayerCount = 3
            };

            var currentGamePlayer = new GamePlayer()
            {
                GameId = gameId, 
                PlayerName = currentPlayer, 
                RoundPosition = currentPlayerPosition
            };

            // ARRANGE - Repositories and success conditions

            var mockGameRepository = new Mock<IGameRepository>();
            mockGameRepository.Setup(repo => repo.GetAsync(gameId).Result)
                              .Returns(currentGame);

            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            mockGamePlayerRepository.Setup(repo => repo.findGamePlayerAsync(gameId, currentPlayerPosition).Result)
                                    .Returns(currentGamePlayer);
            

            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();


            var unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);


            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);


            // ACT
            var result = await gameLogic.NewGameMove(gameId, currentPlayer, isHigher);


            // ASSERT
            TestContext.Out.WriteLine("result.Result: " + result.Result);
            TestContext.Out.WriteLine("result.IsSuccess: " + result.IsSuccess);

          
            if (!result.IsSuccess)
                TestContext.Out.WriteLine("result.ErrorMessage: " + result.ErrorMessage);

            TestContext.Out.WriteLine("currentGamePlayer.Wins: " + currentGamePlayer.Wins);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.AreEqual(result.GameResponse.GameId, gameId);
            Assert.AreEqual(currentGamePlayer.Wins, 1);
            Assert.AreEqual(result.GameResponse.CurrentCard, fakeCard3.ToString());
            mockGameRepository.VerifyAll();
            
        }


    }
}
