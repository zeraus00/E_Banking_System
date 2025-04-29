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
    public class CityRepository : Repository
    {
        public CityRepository(EBankingContext context) : base(context) { }

        public async Task<City?> GetCityByIdAsync(int cityId) => await GetById<City>(cityId);
        public async Task<City?> GetCityByIdAsync(int cityId, IQueryable<City> query) => await Get<City>(c => c.CityId == cityId, query);
        public async Task<City?> GetCityByCityCodeAsync(string cityCode, IQueryable<City>? query = null) => await Get<City>(c => c.CityCode == cityCode, query);
    }

    /// <summary>
    /// Builder class for City
    /// </summary>
    public class CityBuilder
    {
        private string _cityCode = string.Empty;
        private string _cityName = string.Empty;
        private int? _provinceId;

        public CityBuilder WithCityCode (string cityCode)
        {
            _cityCode = cityCode.Trim();
            return this;
        }
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
                CityCode = _cityCode,
                CityName = _cityName,
                ProvinceId = _provinceId
            };
        }
    }
}
