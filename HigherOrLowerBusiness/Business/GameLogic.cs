using HigherOrLowerData.Entities;
using HigherOrLowerData.Responses;
using Microsoft.Extensions.Logging;
using static HigherOrLowerData.Responses.IGameResponse;

namespace HigherOrLowerData.Business
{
    public class GameLogic
    {
        
        IUnitOfWork _unitOfWork;
        IRandomNumbers _randomNumbers;
        ILogger<GameLogic> _logger;
        //IRandomNumbers randomNumbers,  
        public GameLogic(IUnitOfWork unitOfWork, IRandomNumbers randomNumber, ILogger<GameLogic> xlogger)
        {
            _unitOfWork = unitOfWork;
            _randomNumbers = randomNumber; 
            _logger = xlogger;
        }

        public async Task<IEnumerable<Game>> GetAllGames()
        {
            return await _unitOfWork.Game.GetAllAsync();
        }


        public async Task<BusinessResult<IGameStatusResponse>> GetGameStatus(uint gameId)
        {

            var game = await _unitOfWork.Game.GetAsync(gameId);

            if (game == null)
                return new BusinessResult<IGameStatusResponse>(RESULT.Not_Found, RESULT_MESSAGE.INVALID_GAME_ID);

            return new BusinessResult<IGameStatusResponse>(
                        new GameStatusResponse(
                                    gameId, 
                                    CalculateCardFlag(game.CurrentRound), 
                                    await _unitOfWork.GamePlayer.findAllPlayerOfGame(gameId)));
        }


        public async Task<BusinessResult<IGameResponse>> GetGame(uint gameId)
        {

            var game = await _unitOfWork.Game.GetAsync(gameId);

            if (game == null)
                return new BusinessResult<IGameResponse>(RESULT.Not_Found, RESULT_MESSAGE.INVALID_GAME_ID);


            if (game.CurrentRound >= DeckOfCards.NumberOfCards - 1) {
                
                return new BusinessResult<IGameResponse>(
                       new GameResponse(
                           game.Id,
                           "",
                           game.CurrentRound,
                           IGameResponse.CARD_FLAG.END_OF_GAME
                        ));
            }


            DeckOfCards deckOfCards = new DeckOfCards(game.Cards);

            return new BusinessResult<IGameResponse>(
                   new GameResponse(
                       game.Id,
                       deckOfCards.getCard(game.CurrentRound).ToString(),
                       game.CurrentRound,
                       CalculateCardFlag(game.CurrentRound)
                    ));

        }


        public async Task<BusinessResult<IGameResponse>> NewGame(string[] players)
        {
            

            if (players == null || players.Length < 2 || !ValidateDuplicities(players))
                return new BusinessResult<IGameResponse>(RESULT.Bad_Parameters, RESULT_MESSAGE.INVALID_PLAYER_COUNT);

            DeckOfCards deckOfCards = new DeckOfCards(_randomNumbers);

            Game game = new Game(deckOfCards, players.Length);
            
            _unitOfWork.Game.Add(game);

           int roundPosition = 0;

            foreach (var player in players)
            {
                
                _unitOfWork.GamePlayer.Add(new GamePlayer()
                {
                    GameId = game.Id,
                    Game = game,
                    PlayerName = player,
                    RoundPosition = roundPosition++
                });
            }


            await _unitOfWork.SaveChangesAsync();



            return new BusinessResult<IGameResponse>(
                        new GameResponse(
                            game.Id,
                            deckOfCards.getCard(game.CurrentRound).ToString(),
                            game.CurrentRound,
                            IGameResponse.CARD_FLAG.FIRST_CARD
                         ));

        }


        public async Task<BusinessResult<IGameMoveResponse>>GetGameMove(uint gameId, uint moveId)
        {
            var game = await _unitOfWork.Game.GetAsync(gameId);

            if (game == null)
                return new BusinessResult<IGameMoveResponse>(RESULT.Not_Found, RESULT_MESSAGE.INVALID_GAME_ID);

            var playerMove = await _unitOfWork.PlayerMove.GetAsync(moveId);

            if (playerMove == null)
                return new BusinessResult<IGameMoveResponse>(RESULT.Not_Found, RESULT_MESSAGE.INVALID_GAME_ID);


            return new BusinessResult<IGameMoveResponse>(
                        new GameMoveResponse(
                            playerMove.Id,
                            playerMove.GameId,
                            playerMove.PlayerName,
                            playerMove.isHigher,
                            playerMove.IsCorrect,
                            playerMove.PreviousCard,
                            playerMove.NextCard,
                            playerMove.RoundCount,
                            CalculateCardFlag(playerMove.RoundCount)
                         ));


        }


        public async Task<BusinessResult<IGameMoveResponse>> NewGameMove(uint gameId, string player, bool isHigher)
        {
            _logger.LogInformation("NewGameMove: gameId {gameId} player {player}", gameId, player);            


            Game? game = await _unitOfWork.Game.GetAsync(gameId);

            if (game == null)
                return new BusinessResult<IGameMoveResponse>(RESULT.Not_Found, RESULT_MESSAGE.INVALID_GAME_ID);

            if (game.CurrentRound >= DeckOfCards.NumberOfCards-1)
                return new BusinessResult<IGameMoveResponse>(RESULT.Not_Found, RESULT_MESSAGE.GAME_ALREADY_FINISHED);


            int playerPosition = CalculateNextPlayerPosition(game.CurrentRound, game.PlayerCount);
            var gamePlayer = await CheckNextPlayerMatchsName(game.Id, player, playerPosition);
            _logger.LogInformation("GameMove: gamePlayer is null: {isNull}", gamePlayer == null);

            if (gamePlayer == null)
                return new BusinessResult<IGameMoveResponse>(RESULT.Not_Found, RESULT_MESSAGE.WRONG_PLAYER);



            DeckOfCards deckOfCards = new DeckOfCards(game.Cards );

            var prevousCard = deckOfCards.getCard(game.CurrentRound);
            game.CurrentRound++;
            var nextCard = deckOfCards.getCard(game.CurrentRound);


            bool win = CheckPlayerGuestIsCorrect(prevousCard, nextCard, isHigher);

            gamePlayer.Wins += (win ? 1 : 0);
            gamePlayer.Loses += (win ? 0 : 1);

            var playerMove = new PlayerMove(
                                    gameId, 
                                    gamePlayer.Id, 
                                    gamePlayer.PlayerName, 
                                    prevousCard.ToString(), 
                                    isHigher , 
                                    nextCard.ToString(), 
                                    win, 
                                    game.CurrentRound);

            _unitOfWork.PlayerMove.Add(playerMove);

            await _unitOfWork.SaveChangesAsync();

            return new BusinessResult<IGameMoveResponse>(
                        new GameMoveResponse(
                            playerMove.Id,
                            playerMove.GameId,
                            playerMove.PlayerName, 
                            playerMove.isHigher,
                            playerMove.IsCorrect,
                            playerMove.PreviousCard,
                            playerMove.NextCard,
                            playerMove.RoundCount,
                            CalculateCardFlag(playerMove.RoundCount)
                         ));

        }


        private  int CalculateNextPlayerPosition(int currentRound, int playerCount)
        {
            _logger.LogInformation("CalculateNextPlayerPosition   currentRound: {currentRound}   playerCount: {playerCount}   Position: {position} ", currentRound, playerCount, currentRound % playerCount);
            return currentRound % playerCount;
        }


        private static bool CheckPlayerGuestIsCorrect(Card previousCard, Card nextCard, bool isHigher)
        {
            
            return
                // Guest where both Cards have igual face value always wins
                nextCard.Value == previousCard.Value ||
                // Higher and guest is higher  or   lower and gest is not higher
                ((nextCard.Value > previousCard.Value) == isHigher);

        }


        /// <summary>
        /// Search database for the next play of the game. Player Name should match.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerName"></param>
        /// <param name="playerPosition"></param>
        /// <returns>GamePlayer if playerName is the playerPositionTH player of the game </returns>
        private async Task<GamePlayer?> CheckNextPlayerMatchsName(uint gameId,  string playerName, int playerPosition)
        {
            _logger.LogInformation("CheckNextPlayerMatchsName " +
                " getNextPlayer Game: {gameId} playerPosition: {playerPosition} player {player}", 
                gameId, playerPosition, playerName);

            var gamePlayer = await _unitOfWork.GamePlayer.findGamePlayerAsync(gameId, playerPosition);

            if (gamePlayer != null)
            {
                _logger.LogInformation("CheckNextPlayerMatchsName - player {player}", gamePlayer.PlayerName);
                if (gamePlayer.PlayerName != playerName)
                    return null;
            }

            return gamePlayer;
        }




        /// <summary>
        /// Check the list does not any duplicated names
        /// </summary>
        /// <param name="players"></param>
        /// <returns>True: no duplicities   False: there is at least one duplicated name</returns>
        private static bool ValidateDuplicities(string[] players)
        {
            List<string> listOfNames = new List<string>();

            foreach (var player in players)
            {
                if (listOfNames.Contains(player)) return false;
                listOfNames.Add(player);

            }
            return true;
        }


        private CARD_FLAG CalculateCardFlag(int round)
        {
            return round switch
            {
                0 => CARD_FLAG.FIRST_CARD,
                < DeckOfCards.NumberOfCards - 1 => CARD_FLAG.IN_THE_MIDDLE_CARD,
                DeckOfCards.NumberOfCards - 1 => CARD_FLAG.LAST_CARD,
                DeckOfCards.NumberOfCards => CARD_FLAG.END_OF_GAME,
                _ => CARD_FLAG.END_OF_GAME

            };
        }
    }
}
