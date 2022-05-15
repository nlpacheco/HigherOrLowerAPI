using HigherOrLowerAPI.DTO;
using HigherOrLowerAPI.Tools;
using HigherOrLowerData.Business;
using HigherOrLowerData.Entities;
using HigherOrLowerData.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HigherOrLowerAPI.Controllers
{
    [ApiController]
    [Route("gameHigherOrLower")]
    [Produces("application/json")]
    public class GameHigherOrLowerController : ControllerBase
    {
        //private readonly IUnitOfWork _repository;
        private readonly GameLogic _gameLogic;
        private readonly ILogger<GameHigherOrLowerController> _logger;

        public GameHigherOrLowerController(GameLogic gameLogic, ILogger<GameHigherOrLowerController> logger)
        {
            _gameLogic = gameLogic;
            _logger = logger;

        }



        [HttpGet("", Name = "GetGames")]
        public async Task<ActionResult> GetGames()
        {
            return Ok(await _gameLogic.GetAllGames());
        }

        /// <summary>
        /// Get a HigherOrLower Game.
        /// </summary>
        /// <param name="gameId">Identificaton of a game that exists.</param>
        /// <returns>IGameStatusResponse</returns>
        /// <remarks>
        /// Sample Request
        ///     GET /1
        ///     
        /// Sample response:
        ///     {
        ///        "gameId": 1,
        ///        "currentCard": "2 of hearts",
        ///        "roundCounter": 1,
        ///        "CardFlag": "FIRST_CARD"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the game</response>
        /// <response code="404">If the game is not found</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{gameId}", Name = "GetGameById")]
        public async Task<ActionResult<GameDTO>> GetGameById(uint gameId)
        {
            var response = await _gameLogic.GetGame(gameId);

            if (!response.IsSuccess || response.GameResponse == null)
                return BuildErrorResult(response);


            return Ok( new GameDTO( 
                            response.GameResponse.GameId, 
                            response.GameResponse.CurrentCard, 
                            response.GameResponse.RoundCounter+1,
                            response.GameResponse.CardFlag.ToString(),
                            TextResponse.BuildCardMessage(
                                            gameId,
                                            response.GameResponse.CardFlag,
                                            response.GameResponse.CurrentCard)));

        }

        /// <summary>
        /// Create a new HiherOrLower game. 
        /// The list of players has to have at least 2 names. Duplicated names are not allowed.
        /// </summary>
        /// <param name="newGameDTO">Object with a list of players. The list has to have at least 2 players. Duplicated names are not allowed.</param>
        /// <returns>GameDTO</returns>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST
        ///     {
        ///        "players": [
        ///        "John", "James", "Carl"
        ///        ]
        ///     }
        ///     
        /// Sample response:
        ///     {
        ///        "gameId": 1,
        ///        "currentCard": "2 of hearts",
        ///        "roundCounter": 1,
        ///        "CardFlag": "FIRST_CARD"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the new game</response>
        /// <response code="400">Bad parameters (eg, wrong number of players)</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("", Name = "PostNewGame")]
        public async Task<ActionResult<GameDTO>> PostNewGame([FromBody] NewGameDTO newGameDTO)
        {
            if (newGameDTO == null || newGameDTO.Players == null )
                return BadRequest();

            var response = await _gameLogic.NewGame(newGameDTO.Players);

            if (!response.IsSuccess || response.GameResponse == null)
                return BuildErrorResult(response);

            _logger.LogInformation("PostNewGame - {gameId}", response.GameResponse.GameId);

            var actionName = nameof(GetGameById);
            

            //return Created(actionName, dto);
            return CreatedAtAction( actionName,
                                    new { gameId = response.GameResponse.GameId } ,
                                    new GameDTO(
                                            response.GameResponse.GameId,
                                            response.GameResponse.CurrentCard,
                                            response.GameResponse.RoundCounter + 1,
                                            response.GameResponse.CardFlag.ToString(),
                                            TextResponse.BuildCardMessage(
                                                response.GameResponse.GameId,
                                                response.GameResponse.CardFlag,
                                                response.GameResponse.CurrentCard)));
        }


        /// <summary>
        /// Game Update is not Allowed.
        /// </summary>
        /// <param name="gameId">Game identification</param>
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{gameId}", Name = "PutGame")]
        public ActionResult PutGameMove(uint gameId)
        {
            return BadRequest("NOT ALLOWED");
        }


        /// <summary>
        /// Game delete is not Allowed.
        /// </summary>
        /// <param name="gameId">Game identification</param>
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{gameId}", Name = "DeleteGame")]
        public ActionResult DeleteGame(uint gameId)
        {
            return BadRequest("NOT ALLOWED");
        }





        // ###############################################################################
        // ###############################################################################



        /// <summary>
        /// Get the data from a specific move.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="moveId"></param>
        /// <returns>Info about the move such as player name, guess, previous e next cards, etc</returns>
        /// <response code="200">Returns the game move details </response>
        /// <response code="404">If the game or move is not found</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        /// 
        [HttpGet("{gameId}/move/{moveId}", Name = "GetGameMoveById")]
        public async Task<ActionResult<GameMoveDTO>> GetGameMoveById(uint gameId, uint moveId)
        {
            var response = await _gameLogic.GetGameMove(gameId, moveId);

            if (!response.IsSuccess || response.GameResponse == null)
                return BuildErrorResult(response);


            return Ok(new GameMoveDTO(
                            response.GameResponse.MoveId,
                            response.GameResponse.GameId,
                            response.GameResponse.PlayerName,
                            response.GameResponse.IsHigher,
                            response.GameResponse.IsCorrect,
                            response.GameResponse.CurrentCard,
                            response.GameResponse.CardFlag.ToString(),
                            TextResponse.BuildCardMessage(
                                            response.GameResponse.GameId,
                                            response.GameResponse.CardFlag,
                                            response.GameResponse.CurrentCard, 
                                            response.GameResponse.IsCorrect )));


            
        }

        /// <summary>
        /// Accept a new move from a player. The player should provide her or his guess about the next card.
        /// IsHigher should be true if the player guess next card has a bigger number than previous card.
        /// IsHigher should be false if the player guess next card has a smaller number.
        /// The sequence of players has to follow the list provided during the game creation.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="gameMoveDTO"></param>
        /// <returns>Message with result of player's guess and the next card.</returns>
        /// <response code="201">Returns the game response after the move </response>
        /// <response code="400">If the game has already finished</response>
        /// <response code="404">If the game is not found</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{gameId}/move", Name = "PostGameMove")]
        public async Task<ActionResult<GameMoveDTO>> PostGameMove(uint gameId, [FromBody] NewGameMoveDTO gameMoveDTO)
        {
            if (gameMoveDTO == null || gameMoveDTO.Player == null)
                return BadRequest();


            var response = await _gameLogic.NewGameMove(gameId, gameMoveDTO.Player, gameMoveDTO.IsHigher);

            if (!response.IsSuccess || response.GameResponse == null)
                return BuildErrorResult(response);


            return CreatedAtAction("GetGameMoveById", 
                                        new { gameId = response.GameResponse.GameId, moveId = response.GameResponse.MoveId},
                                        new GameMoveDTO(
                                                response.GameResponse.MoveId,
                                                response.GameResponse.GameId,
                                                response.GameResponse.PlayerName,
                                                response.GameResponse.IsHigher,
                                                response.GameResponse.IsCorrect,
                                                response.GameResponse.CurrentCard,
                                                response.GameResponse.CardFlag.ToString(),
                                                TextResponse.BuildCardMessage(
                                                                response.GameResponse.GameId,
                                                                response.GameResponse.CardFlag,
                                                                response.GameResponse.CurrentCard,
                                                                response.GameResponse.IsCorrect)));
   
        }




        // ###############################################################################
        // ###############################################################################



        /// <summary>
        /// Get a HigherOrLower Game score of all players.
        /// </summary>
        /// <param name="gameId">Identificaton of a game that exists.</param>
        /// <returns>IGameStatusResponse</returns>
        /// <remarks>
        /// Sample Request
        ///     GET /1/score
        ///     
        /// Sample response:
        ///     {
        ///        "gameId": 1,
        ///        "currentCard": "2 of hearts",
        ///        "roundCounter": 1,
        ///        "CardFlag": "FIRST_CARD"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the game</response>
        /// <response code="400">If the game has already finished</response>
        /// <response code="404">If the game is not found</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{gameId}/score", Name = "GetGameScore")]
        public async Task<ActionResult<GameScoreDTO>> GetGameScore(uint gameId)
        {
            var response = await _gameLogic.GetGameStatus(gameId);

            if (!response.IsSuccess || response.GameResponse == null)
                return BuildErrorResult<IGameStatusResponse>(response);

            return Ok(new GameScoreDTO(
                                response.GameResponse.GameId,
                                response.GameResponse.CardFlag.ToString(),
                                BuildPlayerResponseList(response.GameResponse.GamePlayers),
                                TextResponse.BuildGameStatusMessage(
                                                    response.GameResponse.GameId, 
                                                    response.GameResponse.CardFlag, 
                                                    response.GameResponse.GamePlayers)));
                
        }



        // ###############################################################################
        // ###############################################################################


        private ActionResult BuildErrorResult<T>(BusinessResult<T> businessResult) where T:class
        {
            return businessResult.Result switch
            {
                RESULT.Bad_Parameters => BadRequest(businessResult.ErrorMessage.ToString()),
                RESULT.Not_Found => NotFound(businessResult.ErrorMessage.ToString()),
                //RESULT.Success =>   
                //                    successCode switch {
                //                        200 => Ok(businessResult.GameResponse),
                //                        201 => CreatedAtAction
                
                _ => throw new NotImplementedException("BuildActionResult - Invalid Business Result" )
                    
            };
        }

        private static List<PlayerScoreDTO> BuildPlayerResponseList(IEnumerable<GamePlayer> players)
        {
            List<PlayerScoreDTO> playerResponses = new List<PlayerScoreDTO>();

            foreach (var player in players)
            {
                playerResponses.Add(new PlayerScoreDTO(player.PlayerName, player.Wins));
            }

            return playerResponses;
        }

    }
}
