using Exceptions;
using Data;
using Data.Repositories.User;


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


        public RegistrationService(EBankingContext context) : base(context)
        {
            _userInfoRepository = new UserInfoRepository(_context);
            _nameRepository = new NameRepository(_context);
            _userInfoBuilder = new UserInfoBuilder();
            _birthInfoRepository = new BirthInfoRepository(_context);
            _addressRepository = new AddressRepository(_context);
            _religionRepository = new ReligionRepository(_context);
        }


        public async Task RegisterAsync(string userFirstName, string? userMiddleName, string userLastName,
            DateTime birthDate, int birthCityId, int birthProvinceId, int birthRegionId,
            string house, string street, int barangayId, int cityId, int provinceId, int regionId, int postalCode,
            int age, string sex, string contactNumber, string civilStatus, int ReligonId
            )
        {
            Name UserName = await RegisterName("Lando", "Alon", "Bogart", null);
            _userInfoBuilder
                .WithUserNameId(UserName.NameId);

            Name MotherName = await RegisterName("Lily", "Pronda", "Bogart", null);
            _userInfoBuilder
                .WithUserNameId(UserName.NameId);

            Name FatherName = await RegisterName("Pando", "Pronda", "Bogart", null);
            _userInfoBuilder
                .WithUserNameId(UserName.NameId);


            BirthInfo UserBirthInfo = await RegisterBirthInfo(birthDate, birthCityId, birthProvinceId, birthRegionId);
            _userInfoBuilder
                .WithBirthInfoId(UserBirthInfo.BirthInfoId);

            Address UserAddress = await RegisterAddress("Blk 12", "Lot 12", barangayId, cityId, provinceId, regionId, postalCode);
            _userInfoBuilder
                .WithAddressId(UserAddress.AddressId);

            Religion UserReligion = await RegisterReligion("Catholic");
            _userInfoBuilder
                .WithReligion(UserReligion.ReligionId);

            UserInfo UserInfo = await RegisterUserInfo(
                UserName.NameId,
                MotherName.NameId,
                FatherName.NameId,
                UserBirthInfo.BirthInfoId,
                UserAddress.AddressId,
                UserReligion.ReligionId,
                age,
                sex,
                "09123414", ///hard coded Contact number
                "123456789", ///Hard coded Tax Identification Number
                civilStatus
                );

            UserInfo userInfo = _userInfoBuilder.Build();
            await _userInfoRepository.AddAsync(userInfo);
            await _userInfoRepository.SaveChangesAsync();

        }


        public async Task<Name> RegisterName(string firstName, string? middleName, string lastName, string? suffix)
        {

            if (string.IsNullOrWhiteSpace(firstName)) 
            {
                throw new FieldMissingException("First Name is required.");
            }

            if (string.IsNullOrWhiteSpace(lastName)) 
            {
                throw new FieldMissingException($"Last Name is required.");
            }

            var nameBuilder = new NameBuilder();
            nameBuilder
                .WithFirstName(firstName)
                .WithLastName(lastName);

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                nameBuilder.WithMiddleName(middleName);
            }

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                nameBuilder.WithSuffix(suffix);
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

        public async Task<Address> RegisterAddress(string house, string street, int barangayId, int cityId, int provinceId, int regionId, int postalCode) 
        {
            if (string.IsNullOrWhiteSpace(house)) 
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
                .WithHouse(house)
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
            string taxIdentificationNumber,
            string civilStatus) 
        {
            if (string.IsNullOrWhiteSpace(contactNumber)) 
            {
                throw new FieldMissingException("Contact number is required.");
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
                .WithUserNameId(userNameId)
                .WithMotherNameId(motherNameId)
                .WithFatherNameId(fatherNameId)
                .WithBirthInfoId(birthInfoId)
                .WithAddressId(addressId)
                .WithReligion(religionId)
                .WithAge(age)
                .WithSex(sex)
                .WithContactNumber(contactNumber)
                .WithTaxIdentificationNumber(taxIdentificationNumber)
                .WithCivilStatus(civilStatus);

            UserInfo UserInfo = _userInfoBuilder.Build();
            await _userInfoRepository.AddAsync(UserInfo);
            await _userInfoRepository.SaveChangesAsync();
            return UserInfo;
        }

    }
}