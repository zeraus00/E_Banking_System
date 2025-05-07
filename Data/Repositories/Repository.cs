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
        public Repository(EBankingContext context) => _context = context;

        /// <summary>
        /// Save changes to database synchronously.
        /// </summary>
        public void SaveChangesSync() => _context.SaveChanges();

        /// <summary>
        /// Save changes to database asynchronously.
        /// </summary>
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        /// <summary>
        /// Add entity of class T to database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void AddSync<T>(T entity) where T : class => _context.Set<T>().Add(entity);

        /// <summary>
        /// Add entity of class T to database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public async Task AddAsync<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

        /// <summary>
        /// Retrieves an entity of class T from database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<T?> GetById<T>(int id, IQueryable<T>? query = null) where T : class
            => await _context.Set<T>().FindAsync(id);

        protected async Task<T?> GetByCompositeId<T>(int id1, int id2) where T : class
            => await _context.Set<T>().FindAsync(id1, id2);

        /// <summary>
        /// Retrieves an entity of class T from database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        protected async Task<T?> Get<T>(Expression<Func<T, bool>> condition, IQueryable<T>? query = null) where T : class
        {
            IQueryable<T> finalQuery = query ?? _context.Set<T>().AsQueryable();
            return await finalQuery.FirstOrDefaultAsync(condition);
        }

        public abstract class CustomQuery<TEntity, TSelf> where TEntity : class where TSelf : CustomQuery<TEntity, TSelf>
        {
            protected IQueryable<TEntity> _query;

            public CustomQuery(IQueryable<TEntity> query)
            {
                _query = query;
            }
            public IQueryable<TEntity> GetQuery() => _query;

            public TSelf NewQuery(IQueryable<TEntity> newQuery)
            {
                _query = newQuery;
                return (TSelf)this;
            }
            public TSelf SkipBy(int skipCount)
            {
                _query = _query.Skip(skipCount);
                return (TSelf)this;
            }
            public TSelf TakeWithCount(int takeCount)
            {
                _query = _query.Take(takeCount);
                return (TSelf)this;
            }
            public async Task<int> GetCountAsync()
            {
                return await _query.CountAsync();
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
            protected IQueryable<TProjection> Selectv2<TProjection>(Expression<Func<TEntity, TProjection>> projection)
            {
                return _query.Select(projection);
            }
            protected async Task<TProjection?> Select<TProjection>(Expression<Func<TEntity, TProjection>> projection)
            {
                return (TProjection?)await _query.Select(projection).FirstOrDefaultAsync();
            }

            protected async Task<List<TProjection>> SelectAsList<TProjection>(Expression<Func<TEntity, TProjection>> projection)
            {
                return await _query.Select(projection).ToListAsync();
            }

        }
    }
}


