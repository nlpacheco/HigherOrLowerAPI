using HigherOrLowerData;
using HigherOrLowerData.Business;
using HigherOrLowerData.Responses;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

using System.Threading.Tasks;

namespace TestProject.HigherOrLower
{
    internal class UnitTest_SQLite_GameLogic
    {
        SqliteConnection? _connection;
        DbContextOptions<ApplicationDbContext>? _contextOptions;

        private readonly string Player1 = "John";
        private readonly string Player2 = "James";
        private readonly string Player3 = "Carl";

        private readonly string fakeRandomCardAsText = "3 3,2 2,10 1,10 0,12 2,13 2,0 2,2 3,9 2,10 2,0 3,8 3,7 3,12 3,4 2,11 0,5 1,10 3,6 1,2 1,5 0,6 2,0 0,9 3,9 1,3 2,6 3,7 0,11 3,11 2,13 3,0 1,9 0,8 0,5 3,3 1,8 2,2 0,11 1,12 1,8 1,4 3,7 2,4 0,4 1,7 1,13 1,6 0,3 0,5 2,13 0,12 0";
        private readonly Card fakeCard0 = new Card(Card.CardValues.Three, Card.Suits.Spades);
        private readonly Card fakeCard1 = new Card(Card.CardValues.Two, Card.Suits.Hearts);
        private readonly Card fakeCard2 = new Card(Card.CardValues.Ten, Card.Suits.Diamonds);
        private readonly Card fakeCard3 = new Card(Card.CardValues.Ten, Card.Suits.Clubs);


        //GameLogic? _gameLogic;
        ILoggerFactory? _loggerfactory;



        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                                        .AddLogging(builder => builder.AddConsole())
                                        .BuildServiceProvider();

            _loggerfactory = serviceProvider.GetService<ILoggerFactory>();

            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection )
                .Options;

        }


        [Test]
        public async Task UnitTest_NewGame_ThreePlayers()
        {

            //ARRANGE
            BusinessResult<IGameResponse>? result = null;

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: three players 
            string[] players = new string[] { Player1, Player2, Player3 };
            

            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);

            // ACT
            result = await gameLogic.NewGame(players);

            context.SaveChanges();


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.That(result.GameResponse, Is.Not.Null);
            Assert.AreEqual(result.GameResponse.GameId, 1);
            Assert.AreEqual(result.GameResponse.RoundCounter, 0);
            Assert.AreEqual(result.GameResponse.CurrentCard.ToString(), fakeCard0.ToString());
        }





        [Test]
        public async Task UnitTest_GameMove_BadGameId()
        {
            //ARRANGE
            
            var logger = _loggerfactory.CreateLogger<GameLogic>();


            // ARRANGE - Use case: wrong game Id
            string[] players = new string[] { Player1, Player2, Player3 };
            uint wrongGameId = 999;
            string currentPlayer = Player1;
            bool isHigher = true;



            // ARRANGE - Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");
            
            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);
           
            var resultNewGame = await gameLogic.NewGame(players);

            context.SaveChanges();


            // ACT
            var result = await gameLogic.NewGameMove(wrongGameId, currentPlayer, isHigher);


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.Not_Found);
            Assert.AreEqual(result.ErrorMessage, RESULT_MESSAGE.INVALID_GAME_ID);


        }






        [Test]
        public async Task UnitTest_GameMove_WrongPlayer()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();


            // ARRANGE - Use case: wrong player
            string[] players = new string[] { Player1, Player2, Player3 };
            uint gameId = 1;
            var correctPlayer = Player1;
            var wrongPlayer = Player3;
            var isHigher = true;



            // ARRANGE - Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);

            var resultNewGame = await gameLogic.NewGame(players);

            context.SaveChanges();



            // ACT
            var result = await gameLogic.NewGameMove(gameId, wrongPlayer, isHigher);

            TestContext.Out.WriteLine(result.ErrorMessage);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.Not_Found);
            Assert.AreEqual(result.ErrorMessage, RESULT_MESSAGE.WRONG_PLAYER);

        }





        [Test]
        public async Task UnitTest_GameMove_1stMov_Looses()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ----------------------------------------------------------------------
            // ARRANGE - Use case: 1st Move and looses
            string[] players = new string[] { Player1, Player2, Player3 };
            uint correctGameId = 1;
            var isHigher = true;            // player's guess
            var currentPlayer = Player1;
            var currentPlayerPosition = 0;



            // ARRANGE - Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);

            var resultNewGame = await gameLogic.NewGame(players);

            context.SaveChanges();


            // ACT
            var result = await gameLogic.NewGameMove(correctGameId, currentPlayer, isHigher);

            // Get player score
            var currentGamePlayer = await unitOfWork.GamePlayer.findGamePlayerAsync(correctGameId, currentPlayerPosition);


                        // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.AreEqual(result.GameResponse.GameId, correctGameId);
            Assert.AreEqual(currentGamePlayer.Wins, 0);
            Assert.AreEqual(result.GameResponse.CurrentCard, fakeCard1.ToString());
            //Assert.AreEqual(result.GameResponse.Message, "The card in Game1 is 2 of Hearts. Incorrect");
            

        }




        [Test]
        public async Task UnitTest_GameMove_1stMov_Wins()
        {

            //ARRANGE

            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: 1st Move and wins
            string[] players = new string[] { Player1, Player2, Player3 };
            uint gameId = 1;
            var isHigher = false;               // player's guess
            var currentPlayer = Player1;
            var currentPlayerPosition = 0;      // Game player position
            

            // ARRANGE - Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);

            var resultNewGame = await gameLogic.NewGame(players);

            context.SaveChanges();


            // ACT
            var result = await gameLogic.NewGameMove(gameId, currentPlayer, isHigher);

            // Get player score
            var currentGamePlayer = await unitOfWork.GamePlayer.findGamePlayerAsync(gameId, currentPlayerPosition);


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.AreEqual(result.GameResponse.GameId, gameId);
            Assert.AreEqual(currentGamePlayer.Wins, 1);
            Assert.AreEqual(result.GameResponse.CurrentCard, fakeCard1.ToString());
            //Assert.AreEqual(result.GameResponse.Message, "The card in Game1 is 2 of Hearts. Correct");

        }



    }



}
