namespace Database.Repository
{
    public class UserRepository(DbContext context)
    {
        private readonly DbContext _context = context;

        public async Task AddUserInfo(UserInfo userInfo)
        {
            await _context.Set<UserInfo>().AddAsync(userInfo);
            await _context.SaveChangesAsync();
        }

        public async Task AddBirthInfo(BirthInfo birthInfo)
        {
            await _context.Set<BirthInfo>().AddAsync(birthInfo);
            await _context.SaveChangesAsync();
        }

        public async Task AddAddress(Address address)
        {
            await _context.Set<Address>().AddAsync(address);
            await _context.SaveChangesAsync();
        }
    }

    public class UserInfoBuilder
    {
        private string _firstName = string.Empty;
        private string? _middleName;
        private string _lastName = string.Empty;
        private string? _suffix;
        private int _age;
        private string _sex = string.Empty;
        private int? _birthInfoId;
        private int? _addressId;
        private string _contactNumber = string.Empty;
        private string _taxIdentificationNumber = string.Empty;
        private string _civilStatus = string.Empty;
        private string _religion = string.Empty;

        public UserInfoBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public UserInfoBuilder WithMiddleName(string? middleName)
        {
            _middleName = middleName;
            return this;
        }

        public UserInfoBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public UserInfoBuilder WithSuffix(string? suffix)
        {
            _suffix = suffix;
            return this;
        }

        public UserInfoBuilder WithAge(int age)
        {
            _age = age;
            return this;
        }

        public UserInfoBuilder WithSex(string sex)
        {
            _sex = sex;
            return this;
        }

        public UserInfoBuilder WithBirthInfoId(int birthInfoId)
        {
            _birthInfoId = birthInfoId;
            return this;
        }

        public UserInfoBuilder WithAddressId(int addressId)
        {
            _addressId = addressId;
            return this;
        }

        public UserInfoBuilder WithContactNumber(string contactNumber)
        {
            _contactNumber = contactNumber;
            return this;
        }

        public UserInfoBuilder WithTaxIdentificationNumber(string taxIdentificationNumber)
        {
            _taxIdentificationNumber = taxIdentificationNumber;
            return this;
        }

        public UserInfoBuilder WithCivilStatus(string civilStatus)
        {
            _civilStatus = civilStatus;
            return this;
        }

        public UserInfoBuilder WithReligion(string religion)
        {
            _religion = religion;
            return this;
        }

        public UserInfo Build()
        {
            return new UserInfo
            {
                FirstName = _firstName,
                MiddleName = _middleName,
                LastName = _lastName,
                Suffix = _suffix,
                Age = _age,
                Sex = _sex,
                BirthInfoId = _birthInfoId,
                AddressId = _addressId,
                ContactNumber = _contactNumber,
                TaxIdentificationNumber = _taxIdentificationNumber,
                CivilStatus = _civilStatus,
                Religion = _religion
            };
        }
    }

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

    public class AddressBuilder
    {
        private string _house = string.Empty;
        private string _street = string.Empty;
        private int _barangayId;
        private int _cityId;
        private int _provinceId;
        private int _regionId;
        private int _postalCode;

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
