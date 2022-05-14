using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Repository
{
    public  interface IRepository<TEntity, PK> where TEntity : class
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        int Count();

        TEntity? Get(PK pk);
        Task<TEntity?> GetAsync(PK pk);
        IEnumerable<TEntity?> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate, string? includeClause);

    }
}
