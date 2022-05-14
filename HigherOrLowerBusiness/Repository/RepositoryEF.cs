using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Repository
{
    public class RepositoryEF<TEntity, PK> : IRepository<TEntity, PK> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entities;


        public RepositoryEF(DbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }
        public void Update(TEntity entity)
        {
            _entities.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _entities.Remove(entity);
        }


        public int Count()
        {
            return _entities.Count();
        }


        public virtual TEntity? Get(PK pk)
        {
            return _entities.Find(pk);
        }

        public virtual async Task<TEntity?> GetAsync(PK pk)
        {

            return await _entities.FindAsync(pk);
        }


        public IEnumerable<TEntity> GetAll()
        {
            return _entities.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }




        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate)
        {
            return await ExecFindAsync(predicate, null, null, null);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate, string? includeClause)
        {
            return await ExecFindAsync(predicate, null, null, includeClause);
        }


        // ------------------------------------------------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------------------------------------------------- //


        private async Task<IEnumerable<TEntity>> ExecFindAsync( Expression<Func<TEntity, bool>>? predicate, 
                                                                Expression<Func<TEntity, bool>>[]? predicates, 
                                                                string[]? includeClauses, 
                                                                string? includeClause
                                                                //, string orderByPropertyName, string orderByDescendingPropertyName
                                                                )
        {

            IQueryable<TEntity> collectionBeforeReturn = BuildQueryable(predicate, 
                                                                        predicates,
                                                                        includeClauses, 
                                                                        includeClause
                                                                        //,orderByPropertyName, orderByDescendingPropertyName
                                                                        );

            return await collectionBeforeReturn.ToListAsync();

        }

        protected virtual IQueryable<TEntity> BuildQueryable(
                                                            Expression<Func<TEntity, bool>>? predicate,
                                                            Expression<Func<TEntity, bool>>[]? predicates,
                                                            string[]? includeClauses, 
                                                            string? includeClause
                                                            
                                                            // ,Expression<Func<TEntity, PK>> orderBySelector
                                                            //,string orderByPropertyName, string orderByDescendingPropertyName
                                                            )
        {
            IQueryable<TEntity> queryable = _entities.AsQueryable();


            if (!String.IsNullOrWhiteSpace(includeClause))
                queryable = queryable.Include(includeClause);

            if (includeClauses != null)
            {
                foreach (var inc in includeClauses)
                {
                    queryable = queryable.Include(inc);
                }
            }


            if (predicate != null)
                queryable = queryable.Where(predicate);

            if (predicates != null)
                foreach (var predic in predicates)
                {
                    if (predic != null)
                        queryable = queryable.Where(predic);
                }

           
            // TEST queryable.OrderBy(orderBySelector);
            //if (!String.IsNullOrWhiteSpace(orderByPropertyName))
            //    queryable = queryable.OrderBy(orderByPropertyName);
            //else if (!String.IsNullOrWhiteSpace(orderByDescendingPropertyName))
            //    queryable = queryable.OrderByDescending(orderByDescendingPropertyName);

            return queryable;
        }




    }
}
