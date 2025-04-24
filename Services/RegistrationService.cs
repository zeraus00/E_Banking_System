using Exceptions;
using Data;
using Data.Enums;
using Data.Repositories.User;
using Data.Repositories.Auth;
using Data.Models.User;


namespace Services
{
    public class RegistrationService : Service
    {
        public RegistrationService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }


        public async Task RegisterAsync(string userFirstName, string? userMiddleName, string userLastName, string? userSuffix,
                string fatherFirstName, string? fatherMiddleName, string fatherLastName, string? fatherSuffix,
                string motherFirstName, string? motherMiddleName, string motherLastName, string? motherSuffix,
                string beneficiaryFirstName, string? beneficiaryMiddleName, string beneficiaryLastName, string? beneficiarySuffix,
                DateTime birthDate, int birthCityId, int birthProvinceId, int birthRegionId,
                string houseNo, string street, int barangayId, int cityId, int provinceId, int regionId, int postalCode,
                int age, string sex, string contactNumber, string Occupation, string taxIdentificationNumber, string civilStatus, string userReligion,
                string username, string email, string password
            )
        {
            Name UserName = await RegisterName(userFirstName, userMiddleName, userLastName, userSuffix);
            Name FatherName = await RegisterName(fatherFirstName, fatherMiddleName, fatherLastName, fatherSuffix);
            Name MotherName = await RegisterName(motherFirstName, motherMiddleName, motherLastName, motherSuffix);
            Name BeneficiaryName = await RegisterName(beneficiaryFirstName, beneficiaryMiddleName, beneficiaryLastName, beneficiarySuffix);

            UserAuth userAuth = await RegisterUserAuth(username, email, password);
            BirthInfo UserBirthInfo = await RegisterBirthInfo(birthDate, birthCityId, birthProvinceId, birthRegionId);
            Address UserAddress = await RegisterAddress(houseNo, street, barangayId, cityId, provinceId, regionId, postalCode);
            Religion UserReligion = await RegisterReligion(userReligion);



            UserInfo UserInfo = await RegisterUserInfo(
                userAuth.UserAuthId,
                UserName.NameId,
                MotherName.NameId,
                FatherName.NameId,
                UserBirthInfo.BirthInfoId,
                UserAddress.AddressId,
                UserReligion.ReligionId,
                age,
                sex,
                contactNumber,
                Occupation,
                taxIdentificationNumber, 
                civilStatus
                );
        }


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

            var UserAuthBuilder = new UserAuthBuilder()
               .WithRoleId((int)RoleTypes.User)
               .WithUserName(username)
               .WithPassword(password);

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
            string civilStatus
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
                .WithCivilStatus(civilStatus);

            UserInfo UserInfo = UserInfoBuilder.Build();


            


            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserInfoRepository userInfoRepo = new UserInfoRepository(dbContext);

                await userInfoRepo.AddAsync(UserInfo);
                await userInfoRepo.SaveChangesAsync();

                return UserInfo;
            }
        }

    }
}