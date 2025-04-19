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

       
        UserInfo userInfo;
        public async Task SeedUserInfos()
        {
            Name name;
            Name fatherName;
            Name motherName;
            name = new NameBuilder()
                    .WithFirstName("Bogart")
                    .WithMiddleName("Mon")
                    .WithLastName("Dela Mon")
                    .WithSuffix("Jr.")
                    .Build();

            fatherName = new NameBuilder()
                .WithFirstName("Bogart")
                .WithMiddleName("Mon")
                .WithLastName("Dela Mon")
                .WithSuffix("Sr.")
                .Build();

            motherName = new NameBuilder()
                .WithFirstName("Marie")
                .WithMiddleName("Tess")
                .WithLastName("Mon")
                .Build();


            if (!await _context.Names.AnyAsync())
            {
                

                await _nameRepository.AddAsync(name);
                await _nameRepository.AddAsync(fatherName);
                await _nameRepository.AddAsync(motherName);

                await _nameRepository.SaveChangesAsync();
            }

            if (!await _context.UsersInfo.AnyAsync())
            {
                userInfo = new UserInfoBuilder()
                    .WithUserNameId(name.NameId)
                    .WithAge(25)
                    .WithSex("Unicorn")
                    .WithFatherNameId(fatherName.NameId)
                    .WithMotherNameId(motherName.NameId)
                    .WithContactNumber("09669696969")
                    .WithOccupation("Macho Dancer")
                    .WithTaxIdentificationNumber("696969696969")
                    .WithCivilStatus("Oppressed")
                    .Build();

                await _userInfoRepository.AddAsync(userInfo);

                await _userInfoRepository.SaveChangesAsync();
            }


        }
    }
}
