namespace Data.Models.User
{
    // UsersInfo Table
    public class UserInfo
    {
        public int UserInfoId { get; set; }                                 // Primary Key
        public int UserAuthId { get; set; }                                 // Foreign Key to UserAuth
        public int UserNameId { get; set; }                                 // Foreign Key to Names
        public byte[]? ProfilePicture { get; set; }                         // User image
        public int Age { get; set; }                                        // Required
        public string Sex { get; set; } = string.Empty;                     // Required; Max Length : 10
        public int? BirthInfoId { get; set; }                               // Foreign Key to BirthInfo
        public int? AddressId { get; set; }                                 // Foreign Key to Address
        public int FatherNameId { get; set; }                               // Foreign Key to Names
        public int MotherNameId { get; set; }                               // Foreign Key to Names
        public string ContactNumber { get; set; } = string.Empty;           // Required; Field Length : 11
        public string Occupation { get; set; } = string.Empty;              // Required; Field Length: 50
        public byte[]? GovernmentId { get; set; }                           // GovermentId Image
        public string TaxIdentificationNumber { get; set; } = string.Empty; // Required; Max Length : 12
        public string CivilStatus { get; set; } = string.Empty;             // Required; Max Length : 20
        public int? ReligionId { get; set; }                                // Foreign Key to Religions


        /*  Navigation Properties   */
        public UserAuth UserAuth { get; set; } = null!;
        public Name UserName { get; set; } = null!;
        public Name FatherName { get; set; } = null!;
        public Name MotherName { get; set; } = null!;
        public BirthInfo? BirthInfo { get; set; } 
        public Address? Address { get; set; }
        public Religion Religion { get; set; } = null!;
        public ICollection<UserInfoAccount> UserInfoAccounts { get; set; } = new List<UserInfoAccount>();
    }
}
