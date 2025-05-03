using Exceptions;
using Data.Constants;

namespace Data.Repositories.User
{
    /// <summary>
    /// CRUD operations handler for UsersInfo table
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    public class UserInfoRepository : Repository
    {
        public UserInfoQuery Query { get; private set; }
        public UserInfoRepository(EBankingContext context) : base(context) 
        {
            Query = new UserInfoQuery(context.UsersInfo.AsQueryable());
        }

        #region Read Methods

        public async Task<UserInfo?> GetUserInfoByIdAsync(int userInfoId) => await GetById<UserInfo>(userInfoId);
        /// <summary>
        /// Retrieves a UserInfo entry by its primary key asynchronously.
        /// Optionally accepts a pre-composed IQueryable with desired includes.
        /// </summary>
        /// <param name="userInfoId">The primary key of the UserInfo entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserInfo entity if found or null if not.</returns>
        public async Task<UserInfo?> GetUserInfoByIdAsync(int userInfoId, IQueryable<UserInfo> query) => await Get<UserInfo>(u => u.UserInfoId == userInfoId, query);

        public class UserInfoQuery : CustomQuery<UserInfo, UserInfoQuery>
        {
            public UserInfoQuery(IQueryable<UserInfo> userInfo) : base (userInfo) { }
            public UserInfoQuery HasUserAuth(int? userAuthId) => WhereCondition(ui => ui.UserAuthId == userAuthId);
            public UserInfoQuery IncludeUserAuth(bool include = true) => include ? Include(ui => ui.UserAuth) : this;
            public UserInfoQuery IncludeUserName(bool include = true) => include ? Include(ui => ui.UserName) : this;
            public UserInfoQuery IncludeBirthInfo(bool include = true) => include ? Include(ui => ui.BirthInfo) : this;
            public UserInfoQuery IncludeAddress(bool include = true) => include ? Include(ui => ui.Address) : this;
            public UserInfoQuery IncludeFatherName(bool include = true) => include ? Include(ui => ui.FatherName) : this;
            public UserInfoQuery IncludeMotherName(bool include = true) => include ? Include(ui => ui.MotherName) : this;
            public UserInfoQuery IncludeReligion(bool include = true) => include ? Include(ui => ui.Religion) : this;
            public UserInfoQuery IncludeUserInfoAccounts(bool include = true) => include ? Include(ui => ui.UserInfoAccounts) : this;
        }

        #endregion Read Methods


    }

    /// <summary>
    /// Builder class for UserInfo
    /// </summary>
    public class UserInfoBuilder
    {
        private int _userNameId;
        private int _userAuthId;
        private byte[]? _profilePicture; //nullable for now
        private int _age;
        private string _sex = string.Empty;
        private int? _birthInfoId;
        private int? _addressId;
        private int _fatherNameId;
        private int _motherNameId;
        private string _contactNumber = string.Empty;
        private string _occupation = string.Empty;
        private byte[]? _governmentId; //nullable for now
        private string _taxIdentificationNumber = string.Empty;
        private string _civilStatus = string.Empty;
        private int? _religionId;

        #region Builder Methods
        public UserInfoBuilder WithUserNameId(int userNameId)
        {
            _userNameId = userNameId;
            return this;
        }
        public UserInfoBuilder WithUserAuthId(int userAuthId)
        {
            _userAuthId = userAuthId;
            return this;
        }
        public UserInfoBuilder WithProfilePicture(byte[] profilePicture)
        {
            if (profilePicture.Length > ImageSize.ONE_MEGA_BYTE)
                throw new ArgumentException($"Image exceeded maximum size {ImageSize.ONE_MEGA_BYTE}");

            _profilePicture = profilePicture;
            return this;
        }
        public UserInfoBuilder WithAge(int age)
        {
            _age = age;
            return this;
        }
        public UserInfoBuilder WithSex(string sex)
        {
            _sex = sex.Trim();
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
            _contactNumber = contactNumber.Trim();
            return this;
        }
        public UserInfoBuilder WithOccupation(string occupation)
        {
            _occupation = occupation.Trim();
            return this;
        }
        public UserInfoBuilder WithGovernmentId(byte[] governmentid)
        {
            if (governmentid.Length > ImageSize.ONE_MEGA_BYTE)
                throw new ArgumentException($"Image exceeded maximum size {ImageSize.ONE_MEGA_BYTE}");

            _governmentId = governmentid;
            return this;
        }
        public UserInfoBuilder WithTaxIdentificationNumber(string taxIdentificationNumber)
        {
            _taxIdentificationNumber = taxIdentificationNumber.Trim();
            return this;
        }
        public UserInfoBuilder WithCivilStatus(string civilStatus)
        {
            _civilStatus = civilStatus.Trim();
            return this;
        }
        public UserInfoBuilder WithReligion(int religionId)
        {
            _religionId = religionId;
            return this;
        }
        #endregion Builder Methods

        /// <summary>
        /// Builds the UserInfo object with the specified properties
        /// </summary>
        /// <returns></returns>
        public UserInfo Build()
        {
            return new UserInfo
            {
                UserAuthId = _userAuthId,
                UserNameId = _userNameId,
                ProfilePicture = _profilePicture,
                Age = _age,
                Sex = _sex,
                BirthInfoId = _birthInfoId,
                AddressId = _addressId,
                FatherNameId = _fatherNameId,
                MotherNameId = _motherNameId,
                ContactNumber = _contactNumber,
                Occupation = _occupation,
                TaxIdentificationNumber = _taxIdentificationNumber,
                GovernmentId = _governmentId,
                CivilStatus = _civilStatus,
                ReligionId = _religionId
            };
        }
    }
}
