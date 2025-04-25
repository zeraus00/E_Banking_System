namespace Data.Repositories.User
{
    /// <summary>
    /// CRUD operations handler for Addresses table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    /// <param name="_context"></param>
    public class AddressRepository : Repository
    {
        public AddressRepository(EBankingContext context) : base (context) { }
    }

    /// <summary>
    /// Builder class for Address
    /// </summary>
    public class AddressBuilder
    {
        private string _house = string.Empty;
        private string _street = string.Empty;
        private int? _barangayId;
        private int? _cityId;
        private int? _provinceId;
        private int? _regionId;
        private int? _postalCode;

        #region Builder Methods
        public AddressBuilder WithHouse(string house)
        {
            _house = house;
            return this;
        }
        public AddressBuilder WithStreet(string street)
        {
            _street = street;
            return this;
        }

        public AddressBuilder WithBarangayId(int barangayId)
        {
            _barangayId = barangayId;
            return this;
        }

        public AddressBuilder WithCityId(int cityId)
        {
            _cityId = cityId;
            return this;
        }

        public AddressBuilder WithProvinceId(int provinceId)
        {
            _provinceId = provinceId;
            return this;
        }

        public AddressBuilder WithRegionId(int regionId)
        {
            _regionId = regionId;
            return this;
        }

        public AddressBuilder WithPostalCode(int postalCode)
        {
            _postalCode = postalCode;
            return this;
        }
        #endregion Builder Methods

        /// <summary>
        /// Builds the Address object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Address Build()
        {
            return new Address
            {
                House = _house,
                Street = _street,
                BarangayId = _barangayId,
                CityId = _cityId,
                ProvinceId = _provinceId,
                RegionId = _regionId,
                PostalCode = _postalCode
            };
        }
    }
}
