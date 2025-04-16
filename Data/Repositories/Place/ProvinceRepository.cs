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
    public class ProvinceRepository
    {
        private readonly EBankingContext _context;

        public ProvinceRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the Provinces table.
        /// </summary>
        /// <param name="province"></param>
        public void AddProvinceSync(Province province)
        {
            _context.Set<Province>().Add(province);
        }

        /// <summary>
        /// Adds a new entry to the Provinces table.
        /// </summary>
        /// <param name="province"></param>
        public async Task AddProvinceAsync(Province province)
        {
            await _context.Set<Province>().AddAsync(province);
        }
    }

    /// <summary>
    /// Builder class for Address
    /// </summary>
    public class ProvinceBuilder
    {
        private string _provinceName = string.Empty;
        private int? _regionId;

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
                ProvinceName = _provinceName,
                RegionId = _regionId
            };
        }
    }
}
