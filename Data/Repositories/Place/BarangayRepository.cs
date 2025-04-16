namespace Data.Repositories.Place
{
    /// <summary>
    /// CRUD operations handler for Addresses table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    /// <param name="_context"></param>
    public class BarangayRepository
    {
        private readonly EBankingContext _context;

        public BarangayRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the Barangays table.
        /// </summary>
        /// <param name="barangay"></param>
        public void AddBarangaySync(Barangay barangay)
        {
            _context.Set<Barangay>().Add(barangay);
        }

        /// <summary>
        /// Adds a new entry to the Barangays table.
        /// </summary>
        /// <param name="barangay"></param>
        public async Task AddBarangayAsync(Barangay barangay)
        {
            await _context.Set<Barangay>().AddAsync(barangay);
        }
        
    }

    /// <summary>
    /// Builder class for Barangay
    /// </summary>
    public class BarangayBuilder
    {
        private string _barangayName = string.Empty;
        private int? _cityId;

        public BarangayBuilder WithBarangayName(string barangayName)
        {
            _barangayName = barangayName.Trim();
            return this;
        }

        public BarangayBuilder WithCityId(int cityId)
        {
            _cityId = cityId;
            return this;
        }

        /// <summary>
        /// Builds the Address object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Barangay Build()
        {
            return new Barangay
            {
                BarangayName = _barangayName,
                CityId = _cityId
            };
        }
    }
}
