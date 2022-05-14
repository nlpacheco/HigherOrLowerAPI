using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Responses
{
    public interface IGameStatusPlayerResponse
    {
        public string PlayerName { get; }
        public int PlayerScore { get; }

    }
}
