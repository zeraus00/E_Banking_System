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
        public UserInfoRepository(EBankingContext context) : base(context) { }


        /// <summary>
        /// Retrieves a UserInfo entry by its primary key.
        /// Optionally accepts a pre-composed IQueryable with desired includes.
        /// </summary>
        /// <param name="userInfoId">The primary key of the UserInfo entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserInfo entity if found or null if not.</returns>
        public UserInfo? GetUserInfoByIdSync(int userInfoId, IQueryable<UserInfo>? query = null)
        {
            UserInfo? userInfo;
            if (query != null)
            {
                userInfo = query.FirstOrDefault(ui => ui.UserInfoId == userInfoId);
            }
            else
            {
                userInfo = _context
                    .UsersInfo
                    .Find(userInfoId);
            }
            return userInfo;
        }

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
        public async Task<UserInfo?> GetUserInfoByIdAsync(int userInfoId, IQueryable<UserInfo>? query = null)
        {
            UserInfo? userInfo;
            if (query != null)
            {
                userInfo = await query.FirstOrDefaultAsync(ui => ui.UserInfoId == userInfoId);
            }
            else
            {
                userInfo = await _context
                    .UsersInfo
                    .FindAsync(userInfoId);
            }
            return userInfo;
        }

        /// <summary>
        /// Builds an IQueryable for querying the UsersInfo table that includes all related entities.
        /// </summary>
        /// <returns>An IQueryable of UserInfo with all includes.</returns>
        public IQueryable<UserInfo> QueryIncludeAll()
        {
            return this.ComposeQuery(
                includeName: true,
                includeBirthInfo: true,
                includeFatherName: true,
                includeMotherName: true,
                includeReligion: true
                );
        }

        /// <summary>
        /// Builds an IQueryable for querying the UsersAuth table with optional related entities.
        /// </summary>
        /// <param name="includeName">Whether to include the related Name entity.</param>
        /// <param name="includeBirthInfo">Whether to include the related BirthInfo entity.</param>
        /// <param name="includeFatherName">Whether to include the related FatherName entity.</param>
        /// <param name="includeMotherName">Whether to include the related MotherName entity.</param>
        /// <param name="includeReligion">Whether to include the related Religion entity.</param>
        /// <returns>An IQueryable of UserInfo with optional includes.</returns>
        public IQueryable<UserInfo> ComposeQuery(
            bool includeName = false,
            bool includeBirthInfo = false,
            bool includeFatherName = false,
            bool includeMotherName = false,
            bool includeReligion = false
            )
        {
            var query = _context
                .UsersInfo
                .AsQueryable();

            if (includeName) { query = query.Include(ui => ui.UserName); }
            if (includeBirthInfo) { query = query.Include(ui => ui.BirthInfo); }
            if (includeFatherName) { query = query.Include(ui => ui.FatherName); }
            if (includeMotherName) { query = query.Include(ui => ui.MotherName); }
            if (includeReligion) { query = query.Include(ui => ui.Religion); }

            return query;
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
        private string _occupation = string.Empty;
        private byte[]? _governmentId; //nullable for now
        private string _taxIdentificationNumber = string.Empty;
        private string _civilStatus = string.Empty;
        private int? _religionId;

        public UserInfoBuilder WithUserNameId(int userNameId)
        {
            _userNameId = userNameId;
            return this;
        }

        public UserInfoBuilder WithProfilePicture(byte[] profilePicture)
        {
            if (profilePicture.Length > ImageSize.OneMegaByte)
                throw new ArgumentException($"Image exceeded maximum size {ImageSize.OneMegaByte}");

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
            if (governmentid.Length > ImageSize.OneMegaByte)
                throw new ArgumentException($"Image exceeded maximum size {ImageSize.OneMegaByte}");

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

        /// <summary>
        /// Builds the UserInfo object with the specified properties
        /// </summary>
        /// <returns></returns>
        public UserInfo Build()
        {
            return new UserInfo
            {
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
