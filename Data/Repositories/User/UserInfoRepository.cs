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
            public UserInfoQuery HasUserAuthId(int? userAuthId) => WhereCondition(ui => ui.UserAuthId == userAuthId);
            public UserInfoQuery HasUserInfoId(int userInfoId) => WhereCondition(ui => ui.UserInfoId == userInfoId);
            public UserInfoQuery IncludeUserAuth(bool include = true) => include ? Include(ui => ui.UserAuth) : this;
            public UserInfoQuery IncludeUserName(bool include = true) => include ? Include(ui => ui.UserName) : this;
            public UserInfoQuery IncludeBirthInfo(bool include = true) => include ? Include(ui => ui.BirthInfo) : this;
            public UserInfoQuery IncludeAddress(bool include = true) => include ? Include(ui => ui.Address) : this;
            public UserInfoQuery IncludeFatherName(bool include = true) => include ? Include(ui => ui.FatherName) : this;
            public UserInfoQuery IncludeMotherName(bool include = true) => include ? Include(ui => ui.MotherName) : this;
            public UserInfoQuery IncludeReligion(bool include = true) => include ? Include(ui => ui.Religion) : this;
            public UserInfoQuery IncludeUserInfoAccounts(bool include = true) => include ? Include(ui => ui.UserInfoAccounts) : this;
            public async Task<Name?> SelectUserName() => await Select<Name>(ui => ui.UserName);
        }

        #endregion Read Methods


    }

    /// <summary>
    /// Builder class for UserInfo
    /// </summary>
    public class UserInfoBuilder
    {
        private int? _userAuthId;
        private int? _userNameId;
        private byte[]? _profilePicture; //nullable for now
        private int _age;
        private string _sex = string.Empty;
        private int? _birthInfoId;
        private int? _addressId;
        private int? _fatherNameId;
        private int? _motherNameId;
        private string _contactNumber = string.Empty;
        private string _occupation = string.Empty;
        private decimal _grossAnnualIncome = 0.0m;
        private byte[]? _governmentId; //nullable for now
        private byte[]? _payslipPicture;
        private string _taxIdentificationNumber = string.Empty;
        private string _civilStatus = string.Empty;
        private int? _religionId;

        private UserAuth? _userAuth;
        private Name? _userName;
        private BirthInfo? _birthInfo;
        private Address? _address;
        private Name? _fatherName;
        private Name? _motherName;
        private Religion? _religion;

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
            if (profilePicture.Length > ImageSizes.ONE_MEGA_BYTE)
                throw new ArgumentException($"Image exceeded maximum size {ImageSizes.ONE_MEGA_BYTE}");

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
        public UserInfoBuilder WithGrossAnnualIncome(decimal grossAnnualIncome)
        {
            _grossAnnualIncome = grossAnnualIncome;
            return this;
        }
        public UserInfoBuilder WithGovernmentId(byte[] governmentid)
        {
            if (governmentid.Length > ImageSizes.ONE_MEGA_BYTE)
                throw new ArgumentException($"Image exceeded maximum size {ImageSizes.ONE_MEGA_BYTE}");

            _governmentId = governmentid;
            return this;
        }
        public UserInfoBuilder WithPayslipPicture(byte[] payslipPicture)
        {
            _payslipPicture = payslipPicture;
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
        public UserInfoBuilder WithReligionId(int religionId)
        {
            _religionId = religionId;
            return this;
        }
        public UserInfoBuilder WithUserAuth(UserAuth userAuth)
        {
            _userAuth = userAuth;
            return this;
        }
        public UserInfoBuilder WithUserName(Name userName)
        {
            _userName = userName;
            return this;
        }
        public UserInfoBuilder WithBirthInfo(BirthInfo birthInfo)
        {
            _birthInfo = birthInfo;
            return this;
        }
        public UserInfoBuilder WithAddress(Address address)
        {
            _address = address;
            return this;
        }
        public UserInfoBuilder WithFatherName(Name fatherName)
        {
            _fatherName = fatherName;
            return this;
        }
        public UserInfoBuilder WithMotherName(Name motherName)
        {
            _motherName = motherName;
            return this;
        }
        public UserInfoBuilder WithReligion(Religion religion)
        {
            _religion = religion;
            return this;
        }
        #endregion Builder Methods

        /// <summary>
        /// Builds the UserInfo object with the specified properties
        /// </summary>
        /// <returns></returns>
        public UserInfo Build()
        {
            UserInfo userInfo = new UserInfo 
            {
                ProfilePicture = _profilePicture,
                Age = _age,
                Sex = _sex,
                ContactNumber = _contactNumber,
                Occupation = _occupation,
                GrossAnnualIncome = _grossAnnualIncome,
                TaxIdentificationNumber = _taxIdentificationNumber,
                GovernmentId = _governmentId,
                PayslipPicture = _payslipPicture,
                CivilStatus = _civilStatus
            };

            if (_userAuthId is int userAuthid)
                userInfo.UserAuthId = userAuthid;
            else if (_userAuth is not null)
                userInfo.UserAuth = _userAuth;

            if (_userNameId is int userNameId)
                userInfo.UserNameId = userNameId;
            else if (_userName is not null)
                userInfo.UserName = _userName;

            if (_birthInfoId is int birthInfoId)
                userInfo.BirthInfoId = birthInfoId;
            else if (_birthInfo is not null)
                userInfo.BirthInfo = _birthInfo;

            if (_addressId is int addressId)
                userInfo.AddressId = addressId;
            else if (_address is not null)
                userInfo.Address = _address;

            if (_fatherNameId is int fatherNameId)
                userInfo.FatherNameId = fatherNameId;
            else if (_fatherName is not null)
                userInfo.FatherName = _fatherName;

            if (_motherNameId is int motherNameId)
                userInfo.MotherNameId = motherNameId;
            else if (_motherName is not null)
                userInfo.MotherName = _motherName;

            if (_religionId is int religiondId)
                userInfo.ReligionId = religiondId;
            else if (_religion is not null)
                userInfo.Religion = _religion;

            return userInfo;
        }
    }
}
