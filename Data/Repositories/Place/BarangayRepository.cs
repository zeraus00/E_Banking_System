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
    public class BarangayRepository : Repository
    {
        public BarangayRepository(EBankingContext context) : base(context) { }

        public async Task<Barangay?> GetBarangayByIdAsync(int barangayId) => await GetById<Barangay>(barangayId);
        public async Task<Barangay?> GetBarangayByBarangayCodeAsync(string barangayCode) => await Get<Barangay>(b => b.BarangayCode == barangayCode);
    }

    /// <summary>
    /// Builder class for Barangay
    /// </summary>
    public class BarangayBuilder
    {
        private string _barangayCode = string.Empty;
        private string _barangayName = string.Empty;
        private int? _cityId;

        public BarangayBuilder WithBarangayCode(string barangayCode)
        {
            _barangayCode = barangayCode.Trim();
            return this;
        }
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
                BarangayCode = _barangayCode,
                BarangayName = _barangayName,
                CityId = _cityId
            };
        }
    }
}
