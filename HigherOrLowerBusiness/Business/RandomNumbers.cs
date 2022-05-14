using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Business
{
    public class RandomNumbers : IRandomNumbers
    {
        public int NextRandom(int maxNumber)
        {
            return Random.Shared.Next(maxNumber);
        }
    }
}
