using Data.Enums;
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

            Name adminName = new NameBuilder()
                .WithFirstName("Hee Hee")
                .WithMiddleName("Mon")
                .WithLastName("Dela Mon")
                .WithSuffix("VII")
                .Build();

            Name adminFatherName = new NameBuilder()
                .WithFirstName("Hee Hee")
                .WithMiddleName("Mon")
                .WithLastName("Dela Mon")
                .WithSuffix("VII")
                .Build();

            Name adminMotherName = new NameBuilder()
                .WithFirstName("Po")
                .WithMiddleName("Key")
                .WithLastName("Mon")
                .Build();

            if (!await _context.Names.AnyAsync())
            {
                

                await _nameRepository.AddAsync(name);
                await _nameRepository.AddAsync(fatherName);
                await _nameRepository.AddAsync(motherName);
                await _nameRepository.AddAsync(adminName);
                await _nameRepository.AddAsync(adminFatherName);
                await _nameRepository.AddAsync(adminMotherName);

                await _nameRepository.SaveChangesAsync();
            }

            if (!await _context.UsersInfo.AnyAsync())
            {
                UserInfo userInfo = new UserInfoBuilder()
                    .WithUserAuthId(2)
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

                UserInfo adminInfo = new UserInfoBuilder()
                    .WithUserAuthId(1)
                    .WithUserNameId(adminName.NameId)
                    .WithAge(56)
                    .WithSex("Frying Pansexual")
                    .WithFatherNameId(adminFatherName.NameId)
                    .WithMotherNameId(adminMotherName.NameId)
                    .WithContactNumber("09GAYGAYGAY")
                    .WithOccupation("Bank Administrator")
                    .WithTaxIdentificationNumber("123456789012")
                    .WithCivilStatus("Committed to Relapsing")
                    .Build();

                await _userInfoRepository.AddAsync(userInfo);
                await _userInfoRepository.AddAsync(adminInfo);

                await _userInfoRepository.SaveChangesAsync();

                List<UserInfo> usersInfo = await _context
                    .UsersInfo
                    .Include(ui => ui.UserAuth)
                    .ThenInclude(ua => ua.Accounts)
                    .ToListAsync();

                foreach (var ui in usersInfo)
                {
                    List<Account> accounts = ui.UserAuth.Accounts.ToList();

                    foreach (var account in accounts)
                    {
                        var link = new UserInfoAccount
                        {
                            UserInfoId = ui.UserInfoId,
                            AccessRoleId = (int)AccessRoles.PRIMARY_OWNER,
                            AccountId = account.AccountId
                        };

                        await _context.UsersInfoAccounts.AddAsync(link);
                    }

                }
            }


        }
    }
}
