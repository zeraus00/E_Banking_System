using Data.Repositories.User;

namespace Data.Seeders.User
{
    public class UserInfoSeeder : Seeder
    {
        NameRepository _nameRepository;
        UserInfoRepository _userInfoRepository;
        public UserInfoSeeder(EBankingContext context) : base(context)
        {
            _nameRepository = new NameRepository(_context);
            _userInfoRepository = new UserInfoRepository(_context);
        }

        public async Task SeedUserInfos()
        {
            Name name;
            UserInfo userInfo;
            if (!await _context.Names.AnyAsync())
            {
                name = new NameBuilder()
                    .WithFirstName("Bogart")
                    .WithMiddleName("Mon")
                    .WithLastName("Dela Mon")
                    .Build();
                await _nameRepository.AddAsync(name);

                if (!await _context.UsersInfo.AnyAsync())
                {
                    userInfo = new UserInfoBuilder()
                        .WithUserNameId(name.NameId)
                        .WithAge(25)
                        .WithSex("Unicorn")
                        .WithFatherNameId(name.NameId)
                        .WithMotherNameId(name.NameId)
                        .WithContactNumber("09669696969")
                        .WithTaxIdentificationNumber("696969696969")
                        .WithCivilStatus("Oppressed")
                        .Build();

                    await _userInfoRepository.AddAsync(userInfo);

                    await _nameRepository.SaveChangesAsync();
                    await _userInfoRepository.SaveChangesAsync();
                }

            }

            
        }
    }
}
