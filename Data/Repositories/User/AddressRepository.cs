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
        public AddressQuery Query { get; private set; }
        public AddressRepository(EBankingContext context) : base (context) 
        {
            Query = new AddressQuery(context.Addresses.AsQueryable());
        }

        public class AddressQuery : CustomQuery<Address, AddressQuery>
        {
            public AddressQuery(IQueryable<Address> query) : base(query) { }
            public AddressQuery HasAddressId(int? addressId) => WhereCondition(a => a.AddressId == addressId);
            public AddressQuery HasBarangayId(int? barangayId) => WhereCondition(a => a.BarangayId == barangayId);
            public AddressQuery HasCityId(int? cityId) => WhereCondition(a => a.CityId == cityId);
            public AddressQuery HasProvinceId(int? provinceId) => WhereCondition(a => a.ProvinceId == provinceId);
            public AddressQuery HasRegionId(int? regionId) => WhereCondition(a => a.RegionId == regionId);
            public AddressQuery HasHouse(string? house) => WhereCondition(a => a.House == house);
            public AddressQuery HasStreet(string? street) => WhereCondition(a => a.Street == street);
            public AddressQuery HasPostalCode(int? postalCode) => WhereCondition(a => a.PostalCode == postalCode);
            public AddressQuery IncludeRegion(bool include = true) => include ? Include(a => a.Region) : this;
            public AddressQuery IncludeProvince(bool include = true) => include ? Include(a => a.Province) : this;
            public AddressQuery IncludeCity(bool include = true) => include ? Include(a => a.City) : this;
            public AddressQuery IncludeBarangay(bool include = true) => include ? Include(a => a.Barangay) : this;
        } 
    }

    /// <summary>
    /// Builder class for Address
    /// </summary>
    public class AddressBuilder
    {
        private string _house = string.Empty;
        private string _street = string.Empty;
        private int? _postalCode;
        private int? _barangayId;
        private int? _cityId;
        private int? _provinceId;
        private int? _regionId;

        private Barangay? _barangay;
        private City? _city;
        private Province? _province;
        private Region? _region;



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
        public AddressBuilder WithPostalCode(int postalCode)
        {
            _postalCode = postalCode;
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
        public AddressBuilder WithBarangay(Barangay barangay)
        {
            _barangay = barangay;
            return this;
        }
        public AddressBuilder WithCity(City city)
        {
            _city = city;
            return this;
        }
        public AddressBuilder WithProvince(Province province)
        {
            _province = province;
            return this;
        }
        public AddressBuilder WithRegion(Region region)
        {
            _region = region;
            return this;
        }
        #endregion Builder Methods

        /// <summary>
        /// Builds the Address object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Address Build()
        {
            Address address = new Address();
            address.House = _house;
            address.Street = _street;
            address.PostalCode = _postalCode;

            if (_barangayId is int barangayId)
                address.BarangayId = _barangayId;
            else if (_barangay is not null)
                address.Barangay = _barangay;

            if (_cityId is int cityId)
                address.CityId = _cityId;
            else if (_city is not null)
                address.City = _city;

            if (_provinceId is int provinceId)
                address.ProvinceId = _provinceId;
            else if (_province is not null)
                address.Province = _province;

            if (_regionId is int regionId)
                address.RegionId = _regionId;
            else if (_region is not null)
                address.Region = _region;

            return address;
        }
    }
}
