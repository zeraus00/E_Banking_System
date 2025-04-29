namespace Data.Repositories.Place
{
    /// <summary>
    /// CRUD operations handler for Provinces table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    /// <param name="_context"></param>
    public class ProvinceRepository : Repository
    {
        public ProvinceRepository(EBankingContext context) : base(context) { }

        public async Task<Province?> GetProvinceByIdAsync(int provinceId) => await GetById<Province>(provinceId);
        public async Task<Province?> GetProvinceByCodeAsync(string provinceCode) => await Get<Province>(p => p.ProvinceCode == provinceCode);
    }

    /// <summary>
    /// Builder class for Province
    /// </summary>
    public class ProvinceBuilder
    {
        private string _provinceCode = string.Empty;
        private string _provinceName = string.Empty;
        private int? _regionId;

        public ProvinceBuilder WithProvinceCode(string provinceCode)
        {
            _provinceCode = provinceCode.Trim();
            return this;
        }
        public ProvinceBuilder WithProvinceName(string provinceName)
        {
            _provinceName = provinceName.Trim();
            return this;
        }

        public ProvinceBuilder WithRegionId(int regionId)
        {
            _regionId = regionId;
            return this;
        }
        /// <summary>
        /// Builds the Address object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Province Build()
        {
            return new Province
            {
                ProvinceCode = _provinceCode,
                ProvinceName = _provinceName,
                RegionId = _regionId
            };
        }
    }
}
