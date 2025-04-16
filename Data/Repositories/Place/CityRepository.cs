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
    public class CityRepository
    {
        private readonly EBankingContext _context;

        public CityRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the Cities table.
        /// </summary>
        /// <param name="address"></param>
        public void AddCitySync(City city)
        {
            _context.Set<City>().Add(city);
        }

        /// <summary>
        /// Adds a new entry to the Cities table.
        /// </summary>
        /// <param name="address"></param>
        public async Task AddCityAsync(City city)
        {
            await _context.Set<City>().AddAsync(city);
        }
    }

    /// <summary>
    /// Builder class for Address
    /// </summary>
    public class CityBuilder
    {
        private string _cityName = string.Empty;
        private int? _provinceId;

        public CityBuilder WithCityName(string cityName)
        {
            _cityName = cityName.Trim();
            return this;
        }

        public CityBuilder WithProvinceId(int provinceId)
        {
            _provinceId = provinceId;
            return this;
        }

        /// <summary>
        /// Builds the City object with the specified properties
        /// </summary>
        /// <returns></returns>
        public City Build()
        {
            return new City
            {
                CityName = _cityName,
                ProvinceId = _provinceId
            };
        }
    }
}
