using HigherOrLowerAPI.Controllers;
using HigherOrLowerAPI.DTO;
using HigherOrLowerData;
using HigherOrLowerData.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestProject.HigherOrLower
{
    internal class UnitTest_Controller_GameHigherOrLower
    {

        SqliteConnection? _connection;
        DbContextOptions<ApplicationDbContext>? _contextOptions;

        private readonly string Player1 = "John";
        private readonly string Player2 = "James";
        private readonly string Player3 = "Carl";

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
                .UseSqlite(_connection)
                .Options;

        }


        [Test]
        public async Task UnitTest_NewGame_ThreePlayers()
        {

            //ARRANGE
            

            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ARRANGE - Use case: three players 
            string[] players = new string[] { Player1, Player2, Player3 };


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);

            // ACT
            ActionResult<HigherOrLowerAPI.DTO.GameDTO>? result = await controller.PostNewGame(
                                        new HigherOrLowerAPI.DTO.NewGameDTO()
                                        {
                                            Players = players
                                        });

            

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result.Result;
            
            Assert.IsInstanceOf<GameDTO>(createdResult.Value);
            GameDTO gameDTO = (GameDTO)createdResult.Value;

            Assert.AreEqual(gameDTO.GameId, 1);
            Assert.AreEqual(gameDTO.CardFlag, "FIRST_CARD");
            Assert.AreEqual(gameDTO.CurrentCard, fakeCard0.ToString());
            Assert.AreEqual(gameDTO.RoundCounter, 1);
            Assert.AreEqual(gameDTO.Message, "The first card in Game1 is 3 of Spades.");

        }


        [Test]
        public async Task UnitTest_GetGame_NoMoves()
        {

            //ARRANGE


            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ARRANGE - Use case: get game with no moves
            string[] players = new string[] { Player1, Player2, Player3 };


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var resultNewGame = await gameLogic.NewGame(players);


            context.SaveChanges();

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);

            //  ACT
            var result = await controller.GetGameById(
                                        resultNewGame.GameResponse.GameId);



            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            OkObjectResult createdResult = (OkObjectResult)result.Result;

            Assert.IsInstanceOf<GameDTO>(createdResult.Value);
            GameDTO gameDTO = (GameDTO)createdResult.Value;

            Assert.AreEqual(gameDTO.GameId, 1);
            Assert.AreEqual(gameDTO.CardFlag, "FIRST_CARD");
            Assert.AreEqual(gameDTO.CurrentCard, fakeCard0.ToString());
            Assert.AreEqual(gameDTO.RoundCounter, 1);
            Assert.AreEqual(gameDTO.Message, "The first card in Game1 is 3 of Spades.");

        }


        [Test]
        public async Task UnitTest_GameMove_BadGameId()
        {

            //ARRANGE


            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ARRANGE - Use case: wrong game Id 
            string[] players = new string[] { Player1, Player2, Player3 };
            uint wrongGameId = 999;
            string currentPlayer = Player1;
            bool isHigher = true;


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var resultNewGame = await gameLogic.NewGame(players);
            

            context.SaveChanges();

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);

            // ACT
            ActionResult<HigherOrLowerAPI.DTO.GameMoveDTO>? result = await controller.PostGameMove(
                                        wrongGameId,
                                        new HigherOrLowerAPI.DTO.NewGameMoveDTO()
                                        {
                                            Player = currentPlayer,
                                            IsHigher = isHigher
                                        });



            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            NotFoundObjectResult notFound = (NotFoundObjectResult)result.Result;

            Assert.AreEqual(notFound.StatusCode, 404);
            
            Assert.AreEqual(notFound.Value, "INVALID_GAME_ID");

        }



        [Test]
        public async Task UnitTest_GameMove_WrongPlayer()
        {

            //ARRANGE

            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ARRANGE - Use case: wrong player
            string[] players = new string[] { Player1, Player2, Player3 };
            string wrongPlayer = Player3;
            bool isHigher = true;


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");


            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var resultNewGame = await gameLogic.NewGame(players);

            context.SaveChanges();

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);


            // ACT
            ActionResult<HigherOrLowerAPI.DTO.GameMoveDTO>? result = await controller.PostGameMove(
                                        resultNewGame.GameResponse.GameId,
                                        new HigherOrLowerAPI.DTO.NewGameMoveDTO()
                                        {
                                            Player = wrongPlayer,
                                            IsHigher = isHigher
                                        });



            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            NotFoundObjectResult notFound = (NotFoundObjectResult)result.Result;

            Assert.AreEqual(notFound.StatusCode, 404);

            Assert.AreEqual(notFound.Value, "WRONG_PLAYER");
        }




        [Test]
        public async Task UnitTest_GameMove_1stMov_Looses()
        {

            //ARRANGE


            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ----------------------------------------------------------------------
            // ARRANGE - Use case: 1st Move and looses
            string[] players = new string[] { Player1, Player2, Player3 };
            var isHigher = true;            // player's guess
            var currentPlayer = Player1;
            var isCorrect = false;


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var resultNewGame = await gameLogic.NewGame(players);
            context.SaveChanges();

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);


            // ACT
            var result = await controller.PostGameMove(
                                        resultNewGame.GameResponse.GameId,
                                        new HigherOrLowerAPI.DTO.NewGameMoveDTO()
                                        {
                                            Player = currentPlayer,
                                            IsHigher = isHigher
                                        });



            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result.Result;

            Assert.IsInstanceOf<GameMoveDTO>(createdResult.Value);
            GameMoveDTO gameMoveDTO = (GameMoveDTO)createdResult.Value;

            Assert.AreEqual(gameMoveDTO.GameId, resultNewGame.GameResponse.GameId);
            Assert.AreEqual(gameMoveDTO.CardFlag, "IN_THE_MIDDLE_CARD");
            Assert.AreEqual(gameMoveDTO.CurrentCard, fakeCard1.ToString());
            Assert.AreEqual(gameMoveDTO.PlayerName, currentPlayer);
            Assert.AreEqual(gameMoveDTO.IsCorrect, isCorrect);
            Assert.AreEqual(gameMoveDTO.Message, "The card in Game1 is 2 of Hearts. Incorrect.");

        }


        [Test]
        public async Task UnitTest_GameMove_2ndMov_Wins()
        {

            //ARRANGE


            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ----------------------------------------------------------------------
            // ARRANGE - Use case: 2nd Move and wins
            string[] players = new string[] { Player1, Player2, Player3 };
            var isHigher = true;            // player's guess
            var currentPlayer = Player2;
            var isCorrect = true;


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var resultNewGame = await gameLogic.NewGame(players);
            context.SaveChanges();
            var result1stMove = await gameLogic.NewGameMove(resultNewGame.GameResponse.GameId, Player1, isHigher);
            context.SaveChanges();

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);



            // ACT
            var result = await controller.PostGameMove(
                                        resultNewGame.GameResponse.GameId,
                                        new HigherOrLowerAPI.DTO.NewGameMoveDTO()
                                        {
                                            Player = currentPlayer,
                                            IsHigher = isHigher
                                        });



            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result.Result;

            Assert.IsInstanceOf<GameMoveDTO>(createdResult.Value);
            GameMoveDTO gameMoveDTO = (GameMoveDTO)createdResult.Value;

            Assert.AreEqual(gameMoveDTO.GameId, resultNewGame.GameResponse.GameId);
            Assert.AreEqual(gameMoveDTO.CardFlag, "IN_THE_MIDDLE_CARD");
            Assert.AreEqual(gameMoveDTO.CurrentCard, fakeCard2.ToString());
            Assert.AreEqual(gameMoveDTO.PlayerName, currentPlayer);
            Assert.AreEqual(gameMoveDTO.IsCorrect, isCorrect);
            Assert.AreEqual(gameMoveDTO.Message, "The card in Game1 is 10 of Diamonds. Correct.");

        }



        [Test]
        public async Task UnitTest_GameMove_3rdMov_Wins()
        {

            //ARRANGE


            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ----------------------------------------------------------------------
            // ARRANGE - Use case: 3rd Move and wins 
            string[] players = new string[] { Player1, Player2, Player3 };
            var isHigher = false;            // player's guess
            var currentPlayer = Player3;
            var isCorrect = true;


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var resultNewGame = await gameLogic.NewGame(players);
            context.SaveChanges();
            var result1stMove = await gameLogic.NewGameMove(resultNewGame.GameResponse.GameId,Player1,true);
            context.SaveChanges();
            var result2ndMove = await gameLogic.NewGameMove(resultNewGame.GameResponse.GameId,Player2,true);
            context.SaveChanges();


            var controller = new GameHigherOrLowerController(gameLogic, loggerController);


            // ACT
            var result = await controller.PostGameMove(
                                        resultNewGame.GameResponse.GameId,
                                        new HigherOrLowerAPI.DTO.NewGameMoveDTO()
                                        {
                                            Player = currentPlayer,
                                            IsHigher = isHigher
                                        });



            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result.Result;

            Assert.IsInstanceOf<GameMoveDTO>(createdResult.Value);
            GameMoveDTO gameMoveDTO = (GameMoveDTO)createdResult.Value;

            Assert.AreEqual(gameMoveDTO.GameId, resultNewGame.GameResponse.GameId);
            Assert.AreEqual(gameMoveDTO.CardFlag, "IN_THE_MIDDLE_CARD");
            Assert.AreEqual(gameMoveDTO.CurrentCard, fakeCard3.ToString());
            Assert.AreEqual(gameMoveDTO.PlayerName, currentPlayer);
            Assert.AreEqual(gameMoveDTO.IsCorrect, isCorrect);
            Assert.AreEqual(gameMoveDTO.Message, "The card in Game1 is 10 of Clubs. Correct.");

        }





        [Test]
        public async Task UnitTest_GameScore_After_3Moves()
        {

            //ARRANGE


            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ----------------------------------------------------------------------
            // ARRANGE - Use case: get game score with 3 players after 3 moves
            string[] players = new string[] { Player1, Player2, Player3 };
            int score1stPlayer = 0;
            int score2ndPlayer = 1;
            int score3rdPlayer = 1;


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);

            var resultNewGame = await gameLogic.NewGame(players);
            context.SaveChanges();
            var result1stMove = await gameLogic.NewGameMove(resultNewGame.GameResponse.GameId, Player1, true);
            context.SaveChanges();
            var result2ndMove = await gameLogic.NewGameMove(resultNewGame.GameResponse.GameId, Player2, true);
            context.SaveChanges();
            var result3rdMove = await gameLogic.NewGameMove(resultNewGame.GameResponse.GameId, Player3, false);
            context.SaveChanges();

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);



            // ACT
            var result = await controller.GetGameScore(resultNewGame.GameResponse.GameId);


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            OkObjectResult createdResult = (OkObjectResult)result.Result;

            Assert.IsInstanceOf<GameScoreDTO>(createdResult.Value);
            GameScoreDTO gameScoreDTO = (GameScoreDTO)createdResult.Value;

            Assert.AreEqual(gameScoreDTO.GameId, resultNewGame.GameResponse.GameId);
            Assert.AreEqual(gameScoreDTO.CardFlag, "IN_THE_MIDDLE_CARD");

            var listEnumerator = gameScoreDTO.Scores.GetEnumerator();

            listEnumerator.Reset();
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player1);
            Assert.AreEqual(listEnumerator.Current.Score, score1stPlayer);
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player2);
            Assert.AreEqual(listEnumerator.Current.Score, score2ndPlayer);
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player3);
            Assert.AreEqual(listEnumerator.Current.Score, score3rdPlayer);
            Assert.IsFalse(listEnumerator.MoveNext());


            Assert.AreEqual(gameScoreDTO.Message, "The current score for the Game1 is John:0, James:1 and Carl:1");

        }













        [Test]
        public async Task UnitTest_TwoPlayerScore_After_EndOfGame()
        {

            //ARRANGE
            var loggerGameLogic = _loggerfactory.CreateLogger<GameLogic>();
            var loggerController = _loggerfactory.CreateLogger<GameHigherOrLowerController>();

            // ARRANGE - Use case: get game scores after Game Over
            string[] players = new string[] { Player1, Player2 };
            
            int score1stPlayer = 15;
            int score2ndPlayer = 15;
            var isHigher = true;



            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), loggerGameLogic);
            var resultNewGame = await gameLogic.NewGame(players);
            context.SaveChanges();

            // Generate all moves
            // Important: Last card is not playable --> DeckOfCards.NumberOfCards - 1
            for (int i = 0; i < DeckOfCards.NumberOfCards - 1; i++)
            {
                var player = i % 2 == 0 ? Player1 : Player2;
                var resultTmp = await gameLogic.NewGameMove(resultNewGame.GameResponse.GameId, player, isHigher);
                context.SaveChanges();

            }

            var controller = new GameHigherOrLowerController(gameLogic, loggerController);



            // ACT
            var result = await controller.GetGameScore(resultNewGame.GameResponse.GameId);


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            OkObjectResult createdResult = (OkObjectResult)result.Result;

            Assert.IsInstanceOf<GameScoreDTO>(createdResult.Value);
            GameScoreDTO gameScoreDTO = (GameScoreDTO)createdResult.Value;

            Assert.AreEqual(gameScoreDTO.GameId, resultNewGame.GameResponse.GameId);
            Assert.AreEqual(gameScoreDTO.CardFlag, "LAST_CARD");

            // Check list of players
            var listEnumerator = gameScoreDTO.Scores.GetEnumerator();

            listEnumerator.Reset();
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player1);
            Assert.AreEqual(listEnumerator.Current.Score, score1stPlayer);
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player2);
            Assert.AreEqual(listEnumerator.Current.Score, score2ndPlayer);
            Assert.IsFalse(listEnumerator.MoveNext());


            Assert.AreEqual(gameScoreDTO.Message, "The final result  for the Game1 is John:15 and James:15");


        }


    }
}
