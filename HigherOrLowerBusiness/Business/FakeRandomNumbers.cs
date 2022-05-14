using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Business
{
    public class FakeRandomNumbers : IRandomNumbers
    {
        int[] numbers = new int[] { 41, 27, 22, 9, 34, 34, 24, 34, 30, 30, 31, 35, 34, 37, 25, 9, 15, 32, 15, 12, 4, 21, 0, 26, 14, 17, 23, 4, 22, 19, 21, 8, 5, 4, 17, 6, 14, 0, 8, 8, 7, 10, 9, 1, 4, 4, 4, 1, 0, 2, 1, 0 };
        int iLastSelected = -1;

        public int NextRandom(int maxNumber)
        {
            if (iLastSelected < numbers.Length - 1)
                return numbers[++iLastSelected];
            return Random.Shared.Next(maxNumber);

        }
    }
}
