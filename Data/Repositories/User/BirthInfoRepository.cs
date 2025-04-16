namespace Data.Repositories.User
{
    /// <summary>
    /// CRUD operations handler for BirthsInfo table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    /// <param name="_context"></param>
    public class BirthInfoRepository
    {
        private readonly EBankingContext _context;

        public BirthInfoRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the BirthsInfo table.
        /// </summary>
        /// <param name="birthInfo"></param>
        public void AddBirthInfoSync(BirthInfo birthInfo)
        {
            _context.Set<BirthInfo>().Add(birthInfo);
        }

        /// <summary>
        /// Adds a new entry to the BirthsInfo table.
        /// </summary>
        /// <param name="birthInfo"></param>
        /// <returns></returns>
        public async Task AddBirthInfoAsync(BirthInfo birthInfo)
        {
            await _context.Set<BirthInfo>().AddAsync(birthInfo);
        }
    }

    /// <summary>
    /// Builder class for BirthInfo
    /// </summary>
    public class BirthInfoBuilder
    {
        private DateTime _birthDate;
        private int _cityId;
        private int _provinceId;
        private int _regionId;

        public BirthInfoBuilder WithBirthDate(DateTime birthDate)
        {
            _birthDate = birthDate;
            return this;
        }

        public BirthInfoBuilder WithCityId(int cityId)
        {
            _cityId = cityId;
            return this;
        }

        public BirthInfoBuilder WithProvinceId(int provinceId)
        {
            _provinceId = provinceId;
            return this;
        }

        public BirthInfoBuilder WithRegionId(int regionId)
        {
            _regionId = regionId;
            return this;
        }

        /// <summary>
        /// Builds the BirthInfo object with the specified properties
        /// </summary>
        /// <returns></returns>
        public BirthInfo Build()
        {
            return new BirthInfo
            {
                BirthDate = _birthDate,
                CityId = _cityId,
                ProvinceId = _provinceId,
                RegionId = _regionId
            };
        }
    }
}
