using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Business
{
    public interface IRandomNumbers
    {
        int NextRandom(int maxNumber);
    }
}
