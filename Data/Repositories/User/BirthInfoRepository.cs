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
        public BirthInfoQuery Query { get; private set; }

        public BirthInfoRepository(EBankingContext context) : base(context)
        {
            Query = new BirthInfoQuery(context.BirthInfos.AsQueryable());
        }

        public class BirthInfoQuery : CustomQuery<BirthInfo, BirthInfoQuery>
        {
            public BirthInfoQuery(IQueryable<BirthInfo> query) : base (query) { }

            public BirthInfoQuery HasBirthInfoId(int? birthInfoId) => WhereCondition(bi => bi.BirthInfoId == birthInfoId);
            public BirthInfoQuery HasBirthDate(DateTime birthDate) => WhereCondition(bi => bi.BirthDate == birthDate.Date);
            public BirthInfoQuery HasCityId(int? cityId) => WhereCondition(bi => bi.CityId == cityId);
            public BirthInfoQuery HasProvinceId(int? provinceId) => WhereCondition(bi => bi.ProvinceId == provinceId);
            public BirthInfoQuery HasRegionId(int? regionId) => WhereCondition(bi => bi.RegionId == regionId);
            public BirthInfoQuery IncludeRegion(bool include = true) => include ? Include(bi => bi.Region) : this;
            public BirthInfoQuery IncludeProvince(bool include = true) => include ? Include(bi => bi.Province) : this;
            public BirthInfoQuery IncludeCity(bool include = true) => include ? Include(bi => bi.City) : this;
        }
    }

    /// <summary>
    /// Builder class for BirthInfo
    /// </summary>
    public class BirthInfoBuilder
    {
        private DateTime _birthDate;
        private int? _cityId;
        private int? _provinceId;
        private int? _regionId;

        private City? _city;
        private Province? _province;
        private Region? _region;

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
        public BirthInfoBuilder WithCity(City city)
        {
            _city = city;
            return this;
        }
        public BirthInfoBuilder WithProvince(Province province)
        {
            _province = province;
            return this;
        }
        public BirthInfoBuilder WithRegion(Region region)
        {
            _region = region;
            return this;
        }
        #endregion Builder Methods

        /// <summary>
        /// Builds the BirthInfo object with the specified properties
        /// </summary>
        /// <returns></returns>
        public BirthInfo Build()
        {
            BirthInfo birthInfo = new BirthInfo();

            birthInfo.BirthDate = _birthDate;

            if (_regionId is int regionId)
                birthInfo.RegionId = regionId;
            else if (_region is not null)
                birthInfo.Region = _region;

            if (_provinceId is int provinceId)
                birthInfo.ProvinceId = provinceId;
            else if (_province is not null)
                birthInfo.Province = _province;

            if (_cityId is int cityId)
                birthInfo.CityId = cityId;
            else if (_city is not null)
                birthInfo.City = _city;


            return birthInfo;
        }
    }
}
