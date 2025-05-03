using Exceptions;
using Data;
using Data.Constants;
using Data.Enums;
using Data.Repositories.User;
using Data.Repositories.Auth;
using Data.Repositories.Place;
using Data.Models.User;
using Data.Repositories.Finance;
using Data.Models.Finance;
using Data.Models.Place;
using Services.DataManagement;
using System.IO;
using ViewModels;


namespace Services
{
    public class RegistrationService : Service
    {
        public RegistrationService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }

        public async Task<Name> RegisterName(string userFirstName, string? userMiddleName, string userLastName, string? userSuffix)
        {

            if (string.IsNullOrWhiteSpace(userFirstName)) 
            {
                throw new FieldMissingException("First Name is required.");
            }

            if (string.IsNullOrWhiteSpace(userLastName)) 
            {
                throw new FieldMissingException($"Last Name is required.");
            }

            var nameBuilder = new NameBuilder();
            nameBuilder
                .WithFirstName(userFirstName)
                .WithLastName(userLastName);

            if (!string.IsNullOrWhiteSpace(userMiddleName))
            {
                nameBuilder.WithMiddleName(userMiddleName);
            }

            if (!string.IsNullOrWhiteSpace(userSuffix))
            {
                nameBuilder.WithSuffix(userSuffix);
            }

            Name UserName = nameBuilder.Build();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                NameRepository nameRepo = new NameRepository(dbContext);
                await nameRepo.AddAsync(UserName);
                await nameRepo.SaveChangesAsync();
                return UserName;
            }
        }

        public async Task<BirthInfo> RegisterBirthInfo(DateTime birthDate, int birthCityId, int birthProvinceId, int birthRegiodId) 
        {
            if (birthDate == default) 
            {
                throw new FieldMissingException($"Birth Date is required.");
            }

            if (birthCityId <= 0) 
            {
                throw new FieldMissingException("City is required.");
            }

            if (birthProvinceId <= 0) 
            {
                throw new FieldMissingException("Province is required.");
            }

            if (birthRegiodId <= 0) 
            {
                throw new FieldMissingException("Region is required.");
            }

            var birthInfoBuilder = new BirthInfoBuilder()
            .WithBirthDate(birthDate)
            .WithCityId(birthCityId)
            .WithProvinceId(birthProvinceId)
            .WithRegionId(birthRegiodId);

            BirthInfo UserBirthInfo = birthInfoBuilder.Build();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                BirthInfoRepository birthInfoRepo = new BirthInfoRepository(dbContext);
                await birthInfoRepo.AddAsync(UserBirthInfo);
                await birthInfoRepo.SaveChangesAsync();
                return UserBirthInfo;
            }
        }

        public async Task<Address> RegisterAddress(string houseNo, string street, int barangayId, int cityId, int provinceId, int regionId, int postalCode) 
        {
            if (string.IsNullOrWhiteSpace(houseNo)) 
            {
                throw new FieldMissingException("House is required.");
            }

            if (string.IsNullOrWhiteSpace(street)) 
            {
                throw new FieldMissingException("Street is required.");
            }

            if (barangayId <= 0) 
            {
                throw new FieldMissingException("Barangay is required.");
            }

            if (cityId <= 0) 
            {
                throw new FieldMissingException("City is required.");
            }

            if (provinceId <= 0) 
            {
                throw new FieldMissingException("Province is required.");
            }
            if (regionId <= 0) 
            {
                throw new FieldMissingException("Region is required.");
            }
            if (postalCode <= 0) 
            {
                throw new FieldMissingException("Postal code is required.");
            }

            var AddressBuilder = new AddressBuilder();
            AddressBuilder
                .WithHouse(houseNo)
                .WithStreet(street)
                .WithBarangayId(barangayId)
                .WithCityId(cityId)
                .WithProvinceId(provinceId)
                .WithRegionId(regionId)
                .WithPostalCode(postalCode);


            Address UserAddress = AddressBuilder.Build();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                AddressRepository addressRepo = new AddressRepository(dbContext);
                await addressRepo.AddAsync(UserAddress);
                await addressRepo.SaveChangesAsync();
                return UserAddress;
            }
        }
        public async Task<int> RegisterRegion(string selectedCode, List<RegionViewModel> regionList)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var regionRepo = new RegionRepository(dbContext);

                RegionViewModel selectedRegion = regionList
                    .FirstOrDefault(r => r.code.GetString() == selectedCode)!;

                Region? region = await regionRepo.GetRegionByCodeAsync(selectedCode);

                if (region is null)
                {
                    region = new();
                    region.RegionCode = selectedCode.Trim();
                    region.RegionName = selectedRegion.name.GetString()?.Trim() ?? FieldPlaceHolders.REGION_NAME_NOT_FOUND;

                    await regionRepo.AddAsync(region);
                    await regionRepo.SaveChangesAsync();
                }

                return region.RegionId;
            }
        }
        public async Task<int> RegisterProvince(string selectedCode, List<ProvinceViewModel> provinceList)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var provinceRepo = new ProvinceRepository(dbContext);

                ProvinceViewModel selectedProvince = provinceList
                    .FirstOrDefault(r => r.code.GetString() == selectedCode)!;

                Province? province = await provinceRepo.GetProvinceByCodeAsync(selectedCode);

                if (province is null)
                {
                    province = new();
                    province.ProvinceCode = selectedCode.Trim();
                    province.ProvinceName = selectedProvince.name.GetString()?.Trim() ?? FieldPlaceHolders.PROVINCE_NAME_NOT_FOUND;

                    await provinceRepo.AddAsync(province);
                    await provinceRepo.SaveChangesAsync();
                }

                return province.ProvinceId;
            }
        }

        public async Task<int> RegisterCity(string selectedCode, List<CityViewModel> cityList)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var cityRepo = new CityRepository(dbContext);

                CityViewModel selectedCity = cityList
                    .FirstOrDefault(r => r.code.GetString() == selectedCode)!;

                City? city = await cityRepo.GetCityByCityCodeAsync(selectedCode);

                if (city is null)
                {
                    city = new();
                    city.CityCode = selectedCode.Trim();
                    city.CityName = selectedCity.name.GetString()?.Trim() ?? FieldPlaceHolders.CITY_NAME_NOT_FOUND;

                    await cityRepo.AddAsync(city);
                    await cityRepo.SaveChangesAsync();
                }

                return city.CityId;
            }
        }

        public async Task<int> RegisterBarangay(string selectedCode, List<BarangayViewModel> barangayList)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var barangayRepo = new BarangayRepository(dbContext);

                BarangayViewModel selectedBarangay = barangayList
                    .FirstOrDefault(r => r.code.GetString() == selectedCode)!;

                Barangay? barangay = await barangayRepo.GetBarangayByBarangayCodeAsync(selectedCode);

                if (barangay is null)
                {
                    barangay = new();
                    barangay.BarangayCode = selectedCode.Trim();
                    barangay.BarangayName = selectedBarangay.name.GetString()?.Trim() ?? FieldPlaceHolders.CITY_NAME_NOT_FOUND;

                    await barangayRepo.AddAsync(barangay);
                    await barangayRepo.SaveChangesAsync();
                }

                return barangay.BarangayId;
            }
        }

        public async Task<Religion> RegisterReligion(string religionName) 
        {
            if (string.IsNullOrWhiteSpace(religionName)) 
            {
                throw new FieldMissingException("Religion is required.");
            }

            var ReligionBuilder = new ReligionBuilder();
            ReligionBuilder
                .WithReligionName(religionName);

            Religion UserReligion = ReligionBuilder.Build();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                ReligionRepository religionRepo = new ReligionRepository(dbContext);
                await religionRepo.AddAsync(UserReligion);
                await religionRepo.SaveChangesAsync();
                return UserReligion;
            }
        }

        public async Task<UserAuth> RegisterUserAuth(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new FieldMissingException("Username is required.");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new FieldMissingException("Email is required.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new FieldMissingException("Password is required.");
            }

            string hashedPassword = Helpers.BcryptHelper.HashPassword(password);

            var UserAuthBuilder = new UserAuthBuilder()
               .WithRoleId((int)RoleTypes.User)
               .WithUserName(username)
               .WithEmail(email)
               .WithPassword(hashedPassword);

            UserAuth userAuth = UserAuthBuilder.Build();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserAuthRepository userAuthRepo = new UserAuthRepository(dbContext);


                //  check for existing userauth
                var existingUserEmail = await userAuthRepo.GetUserAuthByUserNameOrEmailAsync(email);
                var existingUserUserName = await userAuthRepo.GetUserAuthByUserNameOrEmailAsync(username);

                if (existingUserEmail is not null || existingUserUserName is not null)
                {
                    throw new EmailAlreadyExistException(email ?? username);
                }


                await userAuthRepo.AddAsync(userAuth);
                await userAuthRepo.SaveChangesAsync();
                return userAuth;
            }
        }

        public async Task<UserInfo> RegisterUserInfo(
            int userAuthId,
            int userAccType,
            int userAccProductType,
            int userNameId,
            int motherNameId,
            int fatherNameId,
            int birthInfoId,
            int addressId,
            int religionId,
            int age,
            string sex,
            string contactNumber,
            string Occupation,
            string taxIdentificationNumber,
            string civilStatus,
            byte[] profilePicture,
            byte[] governmentId,
            int? beneficiaryAccountId = null
            ) 
        {
            if (string.IsNullOrWhiteSpace(contactNumber)) 
            {
                throw new FieldMissingException("Contact number is required.");
            }

            if (string.IsNullOrWhiteSpace(Occupation)) 
            {
                throw new FieldMissingException("Occupation is required.");
            }

            if (string.IsNullOrEmpty(taxIdentificationNumber)) 
            {
                throw new FieldMissingException("Tax Identification Number is required.");
            }
            if (string.IsNullOrWhiteSpace(civilStatus)) 
            {
                throw new FieldMissingException("Civil Status is required.");
            }


            var UserInfoBuilder = new UserInfoBuilder();
            UserInfoBuilder
                .WithUserAuthId(userAuthId)
                .WithUserNameId(userNameId)
                .WithMotherNameId(motherNameId)
                .WithFatherNameId(fatherNameId)
                .WithBirthInfoId(birthInfoId)
                .WithAddressId(addressId)
                .WithReligion(religionId)
                .WithAge(age)
                .WithSex(sex)
                .WithContactNumber(contactNumber)
                .WithOccupation(Occupation)
                .WithTaxIdentificationNumber(taxIdentificationNumber)
                .WithCivilStatus(civilStatus)
                .WithProfilePicture(profilePicture)
                .WithGovernmentId(governmentId);

            UserInfo UserInfo = UserInfoBuilder.Build();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserInfoRepository userInfoRepo = new UserInfoRepository(dbContext);

                await userInfoRepo.AddAsync(UserInfo);
                await userInfoRepo.SaveChangesAsync();
                return UserInfo;
            }
        }

        public async Task<Account> RegisterAccount(int accountTypeId,int accountProductTypeId) 
        {
            if (accountTypeId <= 0) 
            {
                throw new FieldMissingException("Account Type is required.");
            }

            if (accountProductTypeId <= 0) 
            {
                throw new FieldAccessException("Account product type is required.");
            }

            DateTime creationDate = DateTime.UtcNow.Date;
            string accountNumber = CredentialFactory.GenerateAccountNumber(creationDate, accountTypeId, accountProductTypeId);
            string atmNumber = CredentialFactory.GenerateAtmNumber(creationDate, accountTypeId, accountProductTypeId);
            string accountName = CredentialFactory.GenerateAccountName(accountTypeId, accountProductTypeId);

            var accountBuilder = new AccountBuilder();
            accountBuilder
                .WithAccountType(accountTypeId)
                .WithAccountProductTypeId(accountProductTypeId)
                .WithAccountNumber(accountNumber)
                .WithATMNumber(atmNumber)
                .WithAccountName(accountName)
                .WithAccountStatus((int)AccountStatusTypes.Pending)
                .WithBalance(0);

            Account userAccount = accountBuilder.Build();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                AccountRepository accountRepo = new AccountRepository(dbContext);
                await accountRepo.AddAsync(userAccount);
                await accountRepo.SaveChangesAsync(); 
                return userAccount;
            }
        }

        public async Task<Account?> GetExistingBeneficiaryAccountAsycn(string accountName, string accountNumber) 
        {
            accountName = accountName.Trim().ToUpper();
            accountNumber = accountNumber.Trim();

            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            AccountRepository accountRepo = new AccountRepository(dbContext);

            IQueryable<Account> Query = accountRepo.Query
                .HasAccountNumber(accountNumber)
                .HasAccountName(accountName)
                .GetQuery();

            return await Query.FirstOrDefaultAsync();
        }

        public async Task SyncUserAuthAndAccount(int userAuthId, int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var userAuthRepo = new UserAuthRepository(dbContext);
                var accountRepo = new AccountRepository(dbContext);

                var userAuthQuery = userAuthRepo.ComposeQuery(includeAccounts: true);
                var accountQuery = accountRepo.Query.IncludeUsersAuth().GetQuery();

                UserAuth userAuth = (await userAuthRepo.GetUserAuthByIdAsync(userAuthId, userAuthQuery))!;
                Account account = (await accountRepo.GetAccountByIdAsync(accountId, accountQuery))!;

                userAuth.Accounts.Add(account);
                account.UsersAuth.Add(userAuth);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task SyncUserInfoAndAccount(int userInfoId, int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var userInfoRepo = new UserInfoRepository(dbContext);

                var query = userInfoRepo.Query.IncludeUserInfoAccounts().GetQuery();

                var userInfo = await userInfoRepo.GetUserInfoByIdAsync(userInfoId, query);

                UserInfoAccount link = new UserInfoAccount
                {
                    UserInfoId = userInfoId,
                    AccessRoleId = (int)AccessRoles.PRIMARY_OWNER,
                    AccountId = accountId
                };

                userInfo!.UserInfoAccounts.Add(link);
            }
        }

    }
}