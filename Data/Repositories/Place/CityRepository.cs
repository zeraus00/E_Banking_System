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
    }

    /// <summary>
    /// Builder class for City
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
