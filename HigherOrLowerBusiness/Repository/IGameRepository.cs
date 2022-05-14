using HigherOrLowerData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Repository
{
    public interface IGameRepository: IRepository<Game, uint>
    {
    }
}
