namespace Database.Builder
{
    public class UserService
    {
        private readonly DbContext _context;
        public UserService(DbContext context)
        {
            _context = context;
        }

        public void AddUserInfo(UserInfo userInfo)
        {
            _context.Set<UserInfo>().Add(userInfo);
            _context.SaveChanges();
        }
    }

    class UserInfoBuilder
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
}
