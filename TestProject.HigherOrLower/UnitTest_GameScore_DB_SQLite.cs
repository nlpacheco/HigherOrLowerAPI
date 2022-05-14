using HigherOrLowerData;
using HigherOrLowerData.Business;
using HigherOrLowerData.Responses;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.HigherOrLower
{
    internal class UnitTest_GameScore_DB_SQLite
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
                .UseSqlite(_connection)
                .Options;

        }


        [Test]
        public async Task UnitTest_ThreePlayerScore_After_3Moves()
        {

            //ARRANGE
            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: get game scores: 
            // Player1: looses
            // Player2: wins
            // Player3: wins
            string[] players = new string[] { Player1, Player2, Player3 };
            uint gameId = 1;
            var player1IsHigher = true;               // player's guess
            var player2IsHigher = true;               // player's guess
            var player3IsHigher = false;               // player's guess
            int score1stPlayer = 0;
            int score2ndPlayer = 1;
            int score3rdPlayer = 1;



            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);
            var resultNewGame = await gameLogic.NewGame(players);

            context.SaveChanges();


            var result1 = await gameLogic.NewGameMove(gameId, Player1, player1IsHigher);
            context.SaveChanges();

            var result2 = await gameLogic.NewGameMove(gameId, Player2, player2IsHigher);
            context.SaveChanges();

            var result3 = await gameLogic.NewGameMove(gameId, Player3, player3IsHigher);
            context.SaveChanges();

            // ACT

            var result = await gameLogic.GetGameStatus(gameId);

            //context.SaveChanges();


            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.That(result.GameResponse, Is.Not.Null);
            Assert.AreEqual(result.GameResponse.GameId, gameId);

            //TestContext.Out.WriteLine("PlayerCount: " + result.GameResponse.GameStatusPlayerResponses.Count());
            //foreach (var item in result.GameResponse.GameStatusPlayerResponses)
            //{
            //    TestContext.Out.WriteLine($"Player {item.PlayerName} Score {item.PlayerScore}");
            //}

            var listEnumerator = result.GameResponse.GamePlayers.GetEnumerator();
            
            listEnumerator.Reset();
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player1);
            Assert.AreEqual(listEnumerator.Current.Wins, score1stPlayer);
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player2);
            Assert.AreEqual(listEnumerator.Current.Wins, score2ndPlayer);
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player3);
            Assert.AreEqual(listEnumerator.Current.Wins, score3rdPlayer);
            Assert.IsFalse(listEnumerator.MoveNext());

        }










        
        [Test]
        public async Task UnitTest_TwoPlayerScore_After_EndOfGame()
        {

            //ARRANGE
            var logger = _loggerfactory.CreateLogger<GameLogic>();

            // ARRANGE - Use case: get game scores after Game Over
            string[] players = new string[] { Player1, Player2 };
            uint gameId = 1;
            int score1stPlayer = 15;
            int score2ndPlayer = 15;
            var isHigher = true;


            // Create the schema and seed some data
            using var context = new ApplicationDbContext(_contextOptions);

            if (!context.Database.EnsureCreated())
                throw new System.Exception("Database not created");

            IUnitOfWork unitOfWork = new UnitOfWorkEF(context);
            GameLogic gameLogic = new GameLogic(unitOfWork, new Mocking.FakeRandomNumbers(), logger);
            var resultNewGame = await gameLogic.NewGame(players);
            context.SaveChanges();

            // Last card is not playable
            for (int i = 0; i < DeckOfCards.NumberOfCards-1; i++)
            {
                var player = i % 2 == 0 ? Player1 : Player2;
                var resultTmp = await gameLogic.NewGameMove(gameId, player, isHigher);
                context.SaveChanges();
            }


            // ACT

            var result = await gameLogic.GetGameStatus(gameId);



            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Result, RESULT.SUCCESS);
            Assert.That(result.GameResponse, Is.Not.Null);
            Assert.AreEqual(result.GameResponse.GameId, gameId);

            var listEnumerator = result.GameResponse.GamePlayers.GetEnumerator();

            listEnumerator.Reset();
            listEnumerator.MoveNext();
            TestContext.Out.WriteLine($"Player: {listEnumerator.Current.PlayerName}   score: {listEnumerator.Current.Wins}");
            listEnumerator.MoveNext();
            TestContext.Out.WriteLine($"Player: {listEnumerator.Current.PlayerName}   score: {listEnumerator.Current.Wins}");

            listEnumerator.Reset();
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player1);
            Assert.AreEqual(listEnumerator.Current.Wins, score1stPlayer);
            Assert.IsTrue(listEnumerator.MoveNext());

            Assert.IsNotNull(listEnumerator.Current);
            Assert.AreEqual(listEnumerator.Current.PlayerName, Player2);
            Assert.AreEqual(listEnumerator.Current.Wins, score2ndPlayer);
            Assert.IsFalse(listEnumerator.MoveNext());

        }

    }
}
