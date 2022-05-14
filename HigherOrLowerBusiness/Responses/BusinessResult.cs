using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Responses
{

    public enum RESULT
    {
        SUCCESS,
        Not_Found,
        Bad_Parameters
    }

    public enum RESULT_MESSAGE
    {
        INVALID_GAME_ID,
        INVALID_PLAYER_COUNT,
        GAME_ALREADY_FINISHED,
        WRONG_PLAYER

    }

    public class BusinessResult<T> where T:class
    {

        public RESULT Result { get;}
        public T? GameResponse { get;}

        public RESULT_MESSAGE? ErrorMessage { get; }

        public bool IsSuccess {
            get { return Result == RESULT.SUCCESS; }
        }

        public BusinessResult(T gameResponse)
        {
            this.Result = RESULT.SUCCESS;
            GameResponse = gameResponse;
            ErrorMessage = null;
        }

        public BusinessResult(RESULT result, RESULT_MESSAGE errorMessage)
        {
            this.Result = result;
            ErrorMessage = errorMessage;
            GameResponse = null;
        }


    }
}
