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
    }
}
