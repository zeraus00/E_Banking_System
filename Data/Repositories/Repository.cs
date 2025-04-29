using Microsoft.EntityFrameworkCore;
using static Data.Repositories.Finance.AccountRepository;
using System.Linq;

namespace Data.Repositories
{
    /// <summary>
    /// Abstract class for Repository classes
    /// /// CRUD operations handler for tables
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    public abstract class Repository
    {
        protected readonly EBankingContext _context;
        public Repository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Save changes to database synchronously.
        /// </summary>
        public void SaveChangesSync()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Save changes to database asynchronously.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Add entity of class T to database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void AddSync<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
        }

        /// <summary>
        /// Add entity of class T to database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public async Task AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public abstract class CustomQuery<TEntity, TSelf> where TEntity : class where TSelf : CustomQuery<TEntity, TSelf>
        {
            protected IQueryable<TEntity> _query;

            public CustomQuery(IQueryable<TEntity> query)
            {
                _query = query;
            }

            protected TSelf Include(Expression<Func<TEntity, object?>> navigationProperty) 
            {
                _query = _query.Include(navigationProperty);
                return (TSelf)this;
            }
            protected TSelf OrderBy(Expression<Func<TEntity, object?>> field)
            {
                _query = _query.OrderBy(field);
                return (TSelf)this;
            }
            protected TSelf OrderByDescending(Expression<Func<TEntity, object?>> field)
            {
                _query = _query.OrderByDescending(field);
                return (TSelf)this;
            }
            protected TSelf WhereCondition(Expression<Func<TEntity, bool>> condition)
            {
                _query = _query.Where(condition);
                return (TSelf)this;
            }
            public IQueryable<TEntity> GetQuery()
            {
                return _query;
            }
        }
    }
}


