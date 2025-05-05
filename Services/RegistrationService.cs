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
using ViewModels.Places;


namespace Services
{
    public class RegistrationService : Service
    {
        public RegistrationService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }

        /*      Main Registration       */
        public async Task InitiateRegistraiton(
            Name userName, 
            Name fatherName, 
            Name motherName,
            BirthInfo birthInfo,
            Address address,
            Religion religion,
            Account account,
            UserAuth userAuth,
            UserInfo userInfo)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                await using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        
                        await dbContext.Names.AddRangeAsync(userName, fatherName, motherName);

                        Console.WriteLine("Attempting to register birth info.");

                        birthInfo = await TryGetExistingBirthInfo(birthInfo) ?? birthInfo;

                        BirthInfo? existingBirthInfo = await TryGetExistingBirthInfo(birthInfo);
                        if (existingBirthInfo is null)
                            await dbContext.BirthInfos.AddAsync(birthInfo);
                        else
                        {
                            birthInfo = existingBirthInfo;
                            dbContext.Attach(birthInfo);
                        }
                        Console.WriteLine("Birth info registered.");

                        Console.WriteLine("Attempting to register address.");
                        Address? existingAddress = await TryGetExistingAddress(address);
                        if (existingAddress is null)
                            await dbContext.Addresses.AddAsync(address);
                        else
                        {
                            address = existingAddress;
                            dbContext.Attach(address);
                        }

                        Console.WriteLine("Address registered.");

                        userAuth.Accounts.Add(account);

                        await dbContext.UsersAuth.AddAsync(userAuth);

                        await dbContext.Accounts.AddAsync(account);

                        await dbContext.SaveChangesAsync();

                        userInfo.UserAuth = userAuth;
                        userInfo.UserName = userName;
                        userInfo.FatherName = fatherName;
                        userInfo.MotherName = motherName;
                        userInfo.BirthInfo = birthInfo;
                        userInfo.Address = address;

                        dbContext.Attach(religion);

                        userInfo.Religion = religion;
                        userInfo.UserInfoAccounts.Add(
                            new UserInfoAccount
                            {
                                UserInfo = userInfo,
                                AccessRoleId = (int)AccessRoles.PRIMARY_OWNER,
                                Account = account
                            });

                        await dbContext.UsersInfo.AddAsync(userInfo);
                        await dbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch(Exception)
                    {
                        //  Log error
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
        /*      Name Registration       */

        public Name CreateName(string userFirstName, string? userMiddleName, string userLastName, string? userSuffix)
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

            return nameBuilder.Build();
        }

        /*      BirthInfo Registration      */

        public BirthInfo CreateBirthInfo(DateTime birthDate, City birthCity, Province? birthProvince, Region birthRegion) 
        {
            if (birthDate == default) 
            {
                throw new FieldMissingException($"Birth Date is required.");
            }

            var birthInfoBuilder = new BirthInfoBuilder()
            .WithBirthDate(birthDate)
            .WithCityId(birthCity.CityId)
            .WithRegionId(birthRegion.RegionId);

            if (birthProvince is not null)
                birthInfoBuilder.WithProvinceId(birthProvince.ProvinceId);

            BirthInfo userBirthInfo = birthInfoBuilder.Build();

            return userBirthInfo;

        }
        

        /*      Address Registration        */
        public Address CreateAddress(string houseNo, string street, Barangay barangay, City city, Province? province, Region region, int postalCode) 
        {
            if (string.IsNullOrWhiteSpace(houseNo)) 
            {
                throw new FieldMissingException("House is required.");
            }

            if (string.IsNullOrWhiteSpace(street)) 
            {
                throw new FieldMissingException("Street is required.");
            }
            if (postalCode <= 0) 
            {
                throw new FieldMissingException("Postal code is required.");
            }

            var AddressBuilder = new AddressBuilder();
            AddressBuilder
                .WithHouse(houseNo)
                .WithStreet(street)
                .WithBarangayId(barangay.BarangayId)
                .WithCityId(city.CityId)
                .WithRegionId(region.RegionId);
            if (province is not null)
                AddressBuilder.WithProvinceId(province.ProvinceId);


            Address userAddress = AddressBuilder.Build();

            return userAddress;
        }

        

        public async Task<Region> GetOrRegisterRegion(string selectedCode, List<RegionViewModel> regionList)
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

                return region;
            }
        }
        public async Task<Province?> GetOrRegisterProvince(string selectedCode, int regionId, List<ProvinceViewModel> provinceList)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                if (selectedCode.Equals(FieldPlaceHolders.PROVINCE_CODE_NOT_FOUND))
                    return null;

                var provinceRepo = new ProvinceRepository(dbContext);

                ProvinceViewModel selectedProvince = provinceList
                    .FirstOrDefault(r => r.code.GetString() == selectedCode)!;

                Province? province = await provinceRepo.GetProvinceByCodeAsync(selectedCode);

                if (province is null)
                {
                    province = new();
                    province.ProvinceCode = selectedCode.Trim();
                    province.ProvinceName = selectedProvince.name.GetString()?.Trim() ?? FieldPlaceHolders.PROVINCE_NAME_NOT_FOUND;
                    province.RegionId = regionId;

                    await provinceRepo.AddAsync(province);
                    await provinceRepo.SaveChangesAsync();
                }

                return province;
            }
        }

        public async Task<City> GetOrRegisterCity(string selectedCode, int? provinceId, int? regionId, List<CityViewModel> cityList)
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

                    if (provinceId is int provId)
                        city.ProvinceId = provId;

                    if (regionId is int regId)
                        city.RegionId = regId;

                    await cityRepo.AddAsync(city);
                    await cityRepo.SaveChangesAsync();
                }

                return city;
            }
        }

        public async Task<Barangay> GetOrRegisterBarangay(string selectedCode, int cityId, List<BarangayViewModel> barangayList)
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
                    barangay.CityId = cityId;
                    await barangayRepo.AddAsync(barangay);
                    await barangayRepo.SaveChangesAsync();
                }

                return barangay;
            }
        }

        /*      Religion Registration       */

        public async Task<Religion> GetOrRegisterReligion(string religionName) 
        {
            if (string.IsNullOrWhiteSpace(religionName)) 
            {
                throw new FieldMissingException("Religion is required.");
            }

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                ReligionRepository religionRepo = new ReligionRepository(dbContext);
                Religion? religion = await religionRepo.GetReligionByName(religionName);

                if (religion is null)
                {
                    religion = new ReligionBuilder()
                        .WithReligionName(religionName)
                        .Build();

                    await religionRepo.AddAsync(religion);
                    await religionRepo.SaveChangesAsync();
                }

                return religion;
            }
        }


        /*      UserAuth Registration       */

        public async Task<UserAuth> CreateUserAuth(string username, string email, string password)
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
                var existingUserEmail = (await userAuthRepo.GetUserAuthByUserNameOrEmailAsync(email))?.Email;
                var existingUserUserName = (await userAuthRepo.GetUserAuthByUserNameOrEmailAsync(username))?.UserName;

                if (existingUserEmail is not null || existingUserUserName is not null)
                {
                    throw new EmailAlreadyExistException(email ?? username);
                }

                return userAuth;
            }
        }

        public UserInfo CreateUserInfo(
            int age,
            string sex,
            string contactNumber,
            string Occupation,
            string taxIdentificationNumber,
            string civilStatus,
            byte[] profilePicture,
            byte[] governmentId
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
                .WithAge(age)
                .WithSex(sex)
                .WithContactNumber(contactNumber)
                .WithOccupation(Occupation)
                .WithTaxIdentificationNumber(taxIdentificationNumber)
                .WithCivilStatus(civilStatus)
                .WithProfilePicture(profilePicture)
                .WithGovernmentId(governmentId);

            UserInfo userInfo = UserInfoBuilder.Build();

            return userInfo;
        }

        public Account CreateAccount(int accountTypeId,int accountProductTypeId, string contactNumber, int? linkedBeneficiaryId = null) 
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
                .WithBalance(0)
                .WithAccountContactNo(contactNumber);

            if (linkedBeneficiaryId is int beneficiaryId)
                accountBuilder.WithLinkedBeneficiaryId(beneficiaryId);

            Account userAccount = accountBuilder.Build();

            return userAccount;
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

                await dbContext.SaveChangesAsync();
            }
        }
        private async Task<BirthInfo?> TryGetExistingBirthInfo(BirthInfo birthInfo)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                BirthInfoRepository birthInfoRepo = new BirthInfoRepository(dbContext);
                return await birthInfoRepo
                    .Query
                    .HasBirthDate(birthInfo.BirthDate)
                    .HasRegionId(birthInfo.RegionId)
                    .HasProvinceId(birthInfo.ProvinceId)
                    .HasCityId(birthInfo.CityId)
                    .GetQuery()
                    .FirstOrDefaultAsync();
            }
        }
        private async Task<Address?> TryGetExistingAddress(Address address)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                AddressRepository addressRepo = new AddressRepository(dbContext);
                return await addressRepo
                    .Query
                    .HasRegionId(address.RegionId)
                    .HasProvinceId(address.ProvinceId)
                    .HasCityId(address.CityId)
                    .HasBarangayId(address.BarangayId)
                    .HasStreet(address.Street)
                    .HasHouse(address.House)
                    .HasPostalCode(address.PostalCode)
                    .GetQuery()
                    .FirstOrDefaultAsync();
            }
        }
    }
}