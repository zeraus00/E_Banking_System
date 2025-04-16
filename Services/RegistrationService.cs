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


        public RegistrationService(EBankingContext context) : base(context)
        {
            _userInfoRepository = new UserInfoRepository(_context);
            _nameRepository = new NameRepository(_context);
            _userInfoBuilder = new UserInfoBuilder();
        }


        public async Task RegisterAsync(string userFirstName, string? userMiddleName, string userLastName)
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
        }


        public async Task<Name> RegisterName(string firstName, string? middleName, string lastName, string? suffix)
        {

            if (string.IsNullOrWhiteSpace(firstName)) 
            {
                throw new FieldMissingException($"{firstName}");
            }

            if (string.IsNullOrWhiteSpace(lastName)) 
            {
                throw new FieldMissingException($"{lastName}");
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

        public async Task<BirthInfo> RegisterBirthInfo(DateTime birthDate, int cityId, int provinceId, int regiodId) 
        {
            if (birthDate == default) 
            {
                throw new FieldMissingException();
            }

            if (cityId <= 0) 
            {
                throw new FieldMissingException();
            }

            if (provinceId <= 0) 
            {
                throw new FieldMissingException();
            }

            if (regiodId <= 0) 
            {
                throw new FieldMissingException();
            }

            var birthInfoBuilder = new BirthInfoBuilder()
            .WithBirthDate(birthDate)
            .WithCityId(cityId)
            .WithProvinceId(provinceId)
            .WithRegionId(regiodId);

            var birthInfo = birthInfoBuilder.Build();

            var birthInfoRepository = new BirthInfoRepository(_context);
            await birthInfoRepository.AddAsync(birthInfo);
            await birthInfoRepository.SaveChangesAsync();

            return birthInfo;
        }
    }
}