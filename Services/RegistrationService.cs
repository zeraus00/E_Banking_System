using Exceptions;
using Data;
using Data.Repositories.User;
using Data.Repositories.Auth;
using Data.Models.User;


namespace Services
{
    public class RegistrationService : Service
    {
        NameRepository _nameRepository;
        UserInfoRepository _userInfoRepository;
        UserInfoBuilder _userInfoBuilder;
        BirthInfoRepository _birthInfoRepository;
        AddressRepository _addressRepository;
        ReligionRepository _religionRepository;
        UserAuthRepository _userAuthRepository;


        public RegistrationService(EBankingContext context) : base(context)
        {
            _userInfoRepository = new UserInfoRepository(_context);
            _nameRepository = new NameRepository(_context);
            _userInfoBuilder = new UserInfoBuilder();
            _birthInfoRepository = new BirthInfoRepository(_context);
            _addressRepository = new AddressRepository(_context);
            _religionRepository = new ReligionRepository(_context);
            _userAuthRepository = new UserAuthRepository(_context);
        }


        public async Task RegisterAsync(string userFirstName, string? userMiddleName, string userLastName, string? userSuffix,
                string fatherFirstName, string? fatherMiddleName, string fatherLastName, string? fatherSuffix,
                string motherFirstName, string? motherMiddleName, string motherLastName, string? motherSuffix,
                string beneficiaryFirstName, string? beneficiaryMiddleName, string beneficiaryLastName, string? beneficiarySuffix,
                DateTime birthDate, int birthCityId, int birthProvinceId, int birthRegionId,
                string houseNo, string street, int barangayId, int cityId, int provinceId, int regionId, int postalCode,
                int age, string sex, string contactNumber, string Occupation, string taxIdentificationNumber, string civilStatus, string userReligion,
                string userName, string email, string password
            )
        {
            Name UserName = await RegisterName(userFirstName, userMiddleName, userLastName, userSuffix);
            Name FatherName = await RegisterName(fatherFirstName, fatherMiddleName, fatherLastName, fatherSuffix);
            Name MotherName = await RegisterName(motherFirstName, motherMiddleName, motherLastName, motherSuffix);
            Name BeneficiaryName = await RegisterName(beneficiaryFirstName, beneficiaryMiddleName, beneficiaryLastName, beneficiarySuffix);

            BirthInfo UserBirthInfo = await RegisterBirthInfo(birthDate, birthCityId, birthProvinceId, birthRegionId);
            Address UserAddress = await RegisterAddress(houseNo, street, barangayId, cityId, provinceId, regionId, postalCode);
            Religion UserReligion = await RegisterReligion(userReligion);



            UserInfo UserInfo = await RegisterUserInfo(
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
                civilStatus,
                userName,
                email,
                password
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
            await _nameRepository.AddAsync(UserName);
            await _nameRepository.SaveChangesAsync();
            return UserName;
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
            await _birthInfoRepository.AddAsync(UserBirthInfo);
            await _birthInfoRepository.SaveChangesAsync();
            return UserBirthInfo;
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
            await _addressRepository.AddAsync(UserAddress);
            await _addressRepository.SaveChangesAsync();
            return UserAddress;
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
            await _religionRepository.AddAsync(UserReligion);
            await _religionRepository.SaveChangesAsync();
            return UserReligion;
        }

        public async Task<UserInfo> RegisterUserInfo(
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
            string userName,
            string email,
            string password
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

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new FieldMissingException("Username is Required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new FieldMissingException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new FieldMissingException("Password is required.");
            }

            //check for existing user by username
            var existingUser = await _userAuthRepository.GetUserAuthByUserNameOrEmailAsync(userName);
            if (existingUser != null)
            {
                throw new DuplicateWaitObjectException("Username is already exists.");
            }

            var UserInfoBuilder = new UserInfoBuilder();
            UserInfoBuilder
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
            await _userInfoRepository.AddAsync(UserInfo);
            await _userInfoRepository.SaveChangesAsync();

            var UserAuthBuilder = new UserAuthBuilder()
                .WithRoleId(2)
               .WithUserInfoId(UserInfo.UserInfoId)
               .WithUserName(userName)
               .WithPassword(password);

            UserAuth userAuth = UserAuthBuilder.Build();
            await _userAuthRepository.AddAsync(userAuth);
            await _userAuthRepository.SaveChangesAsync();

            return UserInfo;
        }

    }
}