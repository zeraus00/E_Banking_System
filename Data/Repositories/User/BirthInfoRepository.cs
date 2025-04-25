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
    public class BirthInfoRepository : Repository
    {
        public BirthInfoRepository(EBankingContext context) : base(context) { }

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

        #region Builder Methods
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
        #endregion Builder Methods

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
