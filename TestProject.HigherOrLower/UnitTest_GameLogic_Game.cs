using HigherOrLowerData.Business;
using HigherOrLowerData.Entities;
using HigherOrLowerData.Repository;
using HigherOrLowerData.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TestProject.HigherOrLower.Mocking;



namespace TestProject.HigherOrLower
{
    internal class UnitTest_GameLogic_Game
    {
        private readonly string Player1 = "John";
        private readonly string Player2 = "James";
        private readonly string Player3 = "Carl";

        private readonly string fakeRandomCardAsText = "3 3,2 2,10 1,10 0,12 2,13 2,0 2,2 3,9 2,10 2,0 3,8 3,7 3,12 3,4 2,11 0,5 1,10 3,6 1,2 1,5 0,6 2,0 0,9 3,9 1,3 2,6 3,7 0,11 3,11 2,13 3,0 1,9 0,8 0,5 3,3 1,8 2,2 0,11 1,12 1,8 1,4 3,7 2,4 0,4 1,7 1,13 1,6 0,3 0,5 2,13 0,12 0";
        private readonly Card fakeCard0 = new Card(Card.CardValues.Three, Card.Suits.Spades);


        //GameLogic? _gameLogic;
        ILoggerFactory? _loggerfactory;
        Mocking.MockUnitOfWork? _unitOfWork;



        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                                        .AddLogging(builder => builder.AddConsole())
                                        .BuildServiceProvider();

            _loggerfactory = serviceProvider.GetService<ILoggerFactory>();



            var mockGameRepository = new Mock<IGameRepository>();
            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();

            _unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);

            //IUnitOfWork unitOfWork = new MockUnitOfWork();
            //_gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);

        }


        [Test]
        public async Task UnitTest_NewGame_WrongPlayerList_ZeroPlayers()
        {
            // ARRANGE
            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use Case : Zero players
            string[] players = new string[0];

            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(_unitOfWork, new Mocking.FakeRandomNumbers(), logger);

            // ACT
            var result = await gameLogic.NewGame(players);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.Bad_Parameters);
            Assert.AreEqual(result.ErrorMessage, RESULT_MESSAGE.INVALID_PLAYER_COUNT);
        }




        [Test]
        public async Task UnitTest_NewGame_WrongPlayerList_OnePlayer()
        {

            // ARRANGE
            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use Case : One player
            string[] players = new string[] { "John" };

            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(_unitOfWork, new Mocking.FakeRandomNumbers(), logger);

            // ACT
            var result = await gameLogic.NewGame(players);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.Bad_Parameters);
            Assert.AreEqual(result.ErrorMessage, RESULT_MESSAGE.INVALID_PLAYER_COUNT);
        }


        [Test]
        public async Task UnitTest_NewGame_WrongPlayerList_DuplicatedPlayer()
        {
            // ARRANGE - Use Case : One player
            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use Case : Duplicated player names
            string[] players = new string[] { Player1, Player2, Player1 };

            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(_unitOfWork, new Mocking.FakeRandomNumbers(), logger);

            // ACT
            var result = await gameLogic.NewGame(players);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.Bad_Parameters);
            Assert.AreEqual(result.ErrorMessage, RESULT_MESSAGE.INVALID_PLAYER_COUNT);
        }




        [Test]
        public async Task UnitTest_NewGame_With_TestDeckOfCard()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: three players 
            string[] players = new string[] { Player1, Player2, Player3 };


            // ARRANGE - Repositories and success conditions

            var mockGameRepository = new Mock<IGameRepository>();
            mockGameRepository.Setup(
                repo => repo.Add(It.Is<Game>(
                    g => g.Id == 0
                        &&
                        g.PlayerCount == players.Length
                        &&
                        g.CurrentRound == 0
                        &&
                        g.Cards == fakeRandomCardAsText)));


            var mockGamePlayerRepository = new Mock<IGamePlayerRepository>();
            mockGamePlayerRepository.Setup(
                repo => repo.Add(It.Is<GamePlayer>(
                    p => p.PlayerName == players[0]
                        &&
                        p.Wins == 0
                        &&
                        p.Loses == 0)));

            mockGamePlayerRepository.Setup(
                repo => repo.Add(It.Is<GamePlayer>(
                    p => p.PlayerName == players[1]
                        &&
                        p.Wins == 0
                        &&
                        p.Loses == 0)));

            mockGamePlayerRepository.Setup(
                repo => repo.Add(It.Is<GamePlayer>(
                    p => p.PlayerName == players[2]
                        &&
                        p.Wins == 0
                        &&
                        p.Loses == 0)));


            var mockPlayerMoveRepository = new Mock<IPlayerMoveRepository>();


            var unitOfWork = new MockUnitOfWork(mockGameRepository.Object,
                                             mockGamePlayerRepository.Object,
                                             mockPlayerMoveRepository.Object);


            // ARRANGE - SUT
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);


            // ACT
            var result = await gameLogic.NewGame(players);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.That(result.GameResponse, Is.Not.Null);
            Assert.AreEqual(result.GameResponse.GameId, 0);
            Assert.AreEqual(result.GameResponse.RoundCounter, 0);
            Assert.AreEqual(result.GameResponse.CurrentCard.ToString(), fakeCard0.ToString());
            // Assert.AreEqual(result.GameResponse.Message, "" );
            Assert.IsTrue(unitOfWork.SaveChangesCalled);
            mockGameRepository.VerifyAll();
            mockGamePlayerRepository.VerifyAll();

        }


    }
}
