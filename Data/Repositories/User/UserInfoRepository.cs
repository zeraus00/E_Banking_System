namespace Data.Repositories.User
{
    /// <summary>
    /// CRUD operations handler for UsersInfo table
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    /// <param name="_context"></param>
    public class UserInfoRepository
    {
        private readonly EBankingContext _context;

        public UserInfoRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the UsersInfo table.
        /// </summary>
        /// <param name="userInfo"></param>
        public void AddUserInfoSync(UserInfo userInfo)
        {
            _context.Set<UserInfo>().Add(userInfo);
        }

        /// <summary>
        /// Adds a new entry to the UsersInfo table.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public async Task AddUserInfoAsync(UserInfo userInfo)
        {
            await _context.Set<UserInfo>().AddAsync(userInfo);
        }

    }

    /// <summary>
    /// Builder class for UserInfo
    /// </summary>
    public class UserInfoBuilder
    {
        private int _userNameId;
        private byte[]? _profilePicture; //nullable for now
        private int _age;
        private string _sex = string.Empty;
        private int? _birthInfoId;
        private int? _addressId;
        private int _fatherNameId;
        private int _motherNameId;
        private string _contactNumber = string.Empty;
        private string _taxIdentificationNumber = string.Empty;
        private string _civilStatus = string.Empty;
        private int _religionId;

        
        public UserInfoBuilder WithUserNameId(int userNameId)
        {
            _userNameId = userNameId;
            return this;
        }

        public UserInfoBuilder WithProfilePicture(string filePath)
        {
            // add logic here for profile picture assignment
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

        public UserInfoBuilder WithFatherNameId(int fatherNameId)
        {
            _fatherNameId = fatherNameId;
            return this;
        }

        public UserInfoBuilder WithMotherNameId(int motherNameId)
        {
            _motherNameId = motherNameId;
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

        public UserInfoBuilder WithReligion(int religionId)
        {
            _religionId = religionId;
            return this;
        }

        /// <summary>
        /// Builds the UserInfo object with the specified properties
        /// </summary>
        /// <returns></returns>
        public UserInfo Build()
        {
            return new UserInfo
            {
                UserNameId = _userNameId,
                Age = _age,
                Sex = _sex,
                BirthInfoId = _birthInfoId,
                AddressId = _addressId,
                FatherNameId = _fatherNameId,
                MotherNameId = _motherNameId,
                ContactNumber = _contactNumber,
                TaxIdentificationNumber = _taxIdentificationNumber,
                CivilStatus = _civilStatus,
                ReligionId = _religionId
            };
        }
    }
}
